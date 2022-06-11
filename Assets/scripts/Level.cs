using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Transition
{
	public Vector3 endPosition;
	public int moveSpeed;
}

[Serializable]
public class Rotation
{
	public Vector3 endRotation;
	public int rotationSpeed;
	public bool isInfinite;
}

[Serializable]
public class Item  
{  
	public int id;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
	public bool hasTransition;
	public bool hasRotation;
	public Transition transition;
	public Rotation rotator;
	public int parentId;
}

[Serializable]
public class Level  
{  
	public List<Item> players;
	public List<Item> floors;
	public List<Item> corners;
	public List<Item> otherCorners;
	public List<Item> coins;
	public List<Item> goals;

	public GameObject player;
	public GameObject floor;
	public GameObject corner;
	public GameObject otherCorner;
	public GameObject coin;
	public GameObject goal;

	public Level() {
	}

	public static void Load(TextAsset textAsset, GameObject player, GameObject floor, GameObject corner, GameObject otherCorner, GameObject coin, GameObject goal)
	{
		Level level = JsonUtility.FromJson<Level>(textAsset.text);
		Item playerItem = level.players[0];
		PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour> ();
		RaycastHit hit;
		Vector3 down = new Vector3(0, -1, 0);
		Vector3 playerPosition = new Vector3(playerItem.position.x, playerItem.position.y, playerItem.position.z);
		if (Physics.Raycast (playerPosition, down, out hit, 10)) {
			playerPosition.y = hit.point.y + playerBehaviour.startHeight;
		}

		level.player = player;
		level.floor = floor;
		level.corner = corner;
		level.otherCorner = otherCorner;
		level.coin = coin;
		level.goal = goal;

		player.transform.position = playerPosition;
		player.transform.rotation = playerItem.rotation;
		playerBehaviour.isPlaying = false;

		level.DestroyGameObjectsByTag ("Floor");
		level.DestroyGameObjectsByTag ("Corner");
		level.DestroyGameObjectsByTag ("OtherCorner");
		level.DestroyGameObjectsByTag ("Coin");
		level.DestroyGameObjectsByTag ("Goal");

		level.PlaceGameObjects (floor, level.floors.AsEnumerable().Where(s => s.parentId == 0));
		level.PlaceGameObjects (corner, level.corners.AsEnumerable().Where(s => s.parentId == 0));
		level.PlaceGameObjects (otherCorner, level.otherCorners.AsEnumerable().Where(s => s.parentId == 0));
		level.PlaceGameObjects (coin, level.coins.AsEnumerable().Where(s => s.parentId == 0));
		level.PlaceGameObjects (goal, level.goals.AsEnumerable().Where(s => s.parentId == 0));
	}

	public static void Save(string levelName) {
		Level level = new Level ();
		level.players = level.ItemObjectsByTag ("Player");
		level.floors = level.ItemObjectsByTag ("Floor");
		level.corners = level.ItemObjectsByTag ("Corner");
		level.otherCorners = level.ItemObjectsByTag ("OtherCorner");
		level.coins = level.ItemObjectsByTag ("Coin");
		level.goals = level.ItemObjectsByTag ("Goal");
		level.Write (levelName, JsonUtility.ToJson (level));
	}

	void PlaceGameObject(GameObject oldObject, Item item, GameObject parent = null) 
	{
		GameObject newObject = GameObject.Instantiate(oldObject, item.position, item.rotation);
		FloorBehaviour floorBehaviour = newObject.GetComponent<FloorBehaviour> ();
		newObject.transform.localScale = item.scale;
		if (floorBehaviour != null) {
			newObject.transform.localScale = Vector3.one;
			floorBehaviour.GetComponentInChildren<TextureResize>().transform.localScale = item.scale;

			if (item.id == 0) {
				item.id = newObject.GetInstanceID ();
			}
			floorBehaviour.id = item.id;

			if (item.hasTransition) {
				floorBehaviour.targetPosition = item.transition.endPosition;
				floorBehaviour.moveFrames = item.transition.moveSpeed;
				floorBehaviour.doTranslate = true;
			} 
			if (item.hasRotation) {
				floorBehaviour.targetRotation = item.rotator.endRotation;
				floorBehaviour.rotationFrames = item.rotator.rotationSpeed;
				floorBehaviour.isInfiniteRotation = item.rotator.isInfinite;
				floorBehaviour.doRotate = true;
			} 

			PlaceGameObjects (floor, floors.AsEnumerable().Where(s => s.parentId == floorBehaviour.id), newObject);
			PlaceGameObjects (corner, corners.AsEnumerable().Where(s => s.parentId == floorBehaviour.id), newObject);
			PlaceGameObjects (otherCorner, otherCorners.AsEnumerable().Where(s => s.parentId == floorBehaviour.id), newObject);
			PlaceGameObjects (coin, coins.AsEnumerable().Where(s => s.parentId == floorBehaviour.id), newObject);
			PlaceGameObjects (goal, goals.AsEnumerable().Where(s => s.parentId == floorBehaviour.id), newObject);
		}
		if (parent != null) {
			newObject.transform.parent = parent.transform;
		}
	}

	void PlaceGameObjects(GameObject oldTransform, IEnumerable<Item> items, GameObject parent = null) 
	{
		foreach (Item item in items) {
			PlaceGameObject (oldTransform, item, parent);
		}
	}

	Item ItemObjectByGameObject(GameObject gameObject) {
		FloorBehaviour floor = gameObject.GetComponent<FloorBehaviour> ();
		Item item = new Item ();
		Transform itemTransform = gameObject.transform;
		item.position = itemTransform.localPosition;
		item.rotation = itemTransform.localRotation;
		item.scale = itemTransform.localScale;
		if (floor != null) {
			item.scale = floor.floorTile.transform.localScale;
			if (floor.doTranslate) {
				Transition transition = new Transition ();
				item.position = floor.startPosition;
				transition.endPosition = floor.targetPosition;
				transition.moveSpeed = floor.moveFrames;
				item.hasTransition = true;
				item.transition = transition;
			}
			if (floor != null && floor.doRotate) {
				Rotation rotation = new Rotation ();
				item.rotation = Quaternion.Euler (floor.startRotation);
				rotation.endRotation = floor.targetRotation;
				rotation.rotationSpeed = floor.rotationFrames;
				rotation.isInfinite = floor.isInfiniteRotation;
				item.hasRotation = true;
				item.rotator = rotation;
			}
			item.id = floor.id;
		}
		if (gameObject.transform.parent != null && gameObject.transform.parent.gameObject.tag.Equals ("Floor")) {
			FloorBehaviour parentFloor = gameObject.transform.parent.gameObject.GetComponent<FloorBehaviour> ();
			item.parentId = parentFloor.id;
		}
		return item;
	}

	List<Item> ItemObjectsByTag(string tag) {
		List<Item> items = new List<Item> ();
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
		foreach (GameObject gameObject in gameObjects)
		{
			items.Add (ItemObjectByGameObject(gameObject));
		}
		return items;
	}

	void DestroyGameObjectsByTag(string tag) {
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag (tag);
		foreach (GameObject gameObject in gameObjects)
		{
			GameObject.DestroyImmediate(gameObject, true);
		}
	}

	void Write(string title, string content)
	{
		#if UNITY_EDITOR
		string path = "Assets/Levels/" + title + ".json";

		AssetDatabase.DeleteAsset (path); 

		StreamWriter writer = new StreamWriter (path, true);
		writer.WriteLine (content);
		writer.Close ();

		AssetDatabase.ImportAsset (path); 
		#endif
	}
}

