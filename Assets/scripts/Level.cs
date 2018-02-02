using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Item  
{  
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
}

[Serializable]
public class Level  
{  
	public List<Item> players;
	public List<Item> floors;
	public List<Item> coins;
	public List<Item> goals;


	public Level() {
	}

	public static void Load(TextAsset textAsset, GameObject player, GameObject floor, GameObject coin, GameObject goal)
	{
		Level level = JsonUtility.FromJson<Level>(textAsset.text);
		Item playerItem = level.players[0];
		player.transform.position = playerItem.position;
		player.transform.rotation = playerItem.rotation;

		level.DestroyGameObjectsByTag ("Floor");
		level.DestroyGameObjectsByTag ("Coin");
		level.DestroyGameObjectsByTag ("Goal");
		level.PlaceGameObjects (floor, level.floors);
		level.PlaceGameObjects (coin, level.coins);
		level.PlaceGameObjects (goal, level.goals);
	}

	public static void Save(string levelName) {
		Level level = new Level ();
		level.players = level.ItemObjectsByTag ("Player");
		level.floors = level.ItemObjectsByTag ("Floor");
		level.coins = level.ItemObjectsByTag ("Coin");
		level.goals = level.ItemObjectsByTag ("Goal");
		level.Write (levelName, JsonUtility.ToJson (level));
	}

	void PlaceGameObject(GameObject oldObject, Item item) 
	{
		GameObject newObject = GameObject.Instantiate(oldObject, item.position, item.rotation);
		newObject.transform.localScale = item.scale;
	}

	void PlaceGameObjects(GameObject oldTransform, List<Item> items) 
	{
		
		foreach (Item item in items) {
			PlaceGameObject (oldTransform, item);
		}
	}

	Item ItemObjectByGameObject(GameObject gameObject) {
		Item item = new Item ();
		Transform itemTransform = gameObject.transform;
		item.position = itemTransform.position;
		item.rotation = itemTransform.rotation;
		item.scale = itemTransform.localScale;
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
			GameObject.Destroy(gameObject);
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

