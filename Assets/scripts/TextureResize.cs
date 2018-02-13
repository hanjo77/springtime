using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TextureResize : MonoBehaviour 
{
	public float scaleFactor = 5.0f;
	public List<GameObject> surfaces;
	public List<GameObject> faces;
	public List<GameObject> sides;

	private List<Material> surfaceMats;
	private List<Material> faceMats;
	private List<Material> sideMats;
	// Use this for initialization
	void Start () 
	{
		Debug.Log("Start");
		surfaceMats = new List<Material> ();
		faceMats = new List<Material> ();
		sideMats = new List<Material> ();

		if (sides.Count > 0) {
			foreach (GameObject gameObject in sides) {
				sideMats.Add(gameObject.GetComponent<Renderer> ().material);
			}
			if (sideMats.Count > 0) {
				foreach (Material side in sideMats) {
					ResizeMat (side, true, false);
				}
			}
		}
		if (surfaces.Count > 0) {
			foreach (GameObject gameObject in surfaces) {
				surfaceMats.Add (gameObject.GetComponent<Renderer> ().material);
			}
			if (surfaceMats.Count > 0) {
				foreach (Material surface in surfaceMats) {
					ResizeMat (surface, false, false);
				}
			}
		}
		if (faces.Count > 0) {
			foreach (GameObject gameObject in faces) {
				faceMats.Add (gameObject.GetComponent<Renderer> ().material);
			}
			if (faceMats.Count > 0) {
				foreach (Material face in faceMats) {
					ResizeMat (face, false, true);
				}
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (transform.hasChanged && Application.isEditor && !Application.isPlaying) 
		{
			Debug.Log("The transform has changed!");
			transform.hasChanged = false;
			if (sideMats.Count > 0) {
				foreach (Material side in sideMats) {
					ResizeMat (side, true, false);
				}
			}
			if (surfaceMats.Count > 0) {
				foreach (Material surface in surfaceMats) {
					ResizeMat (surface, false, false);
				}
			}
			if (faceMats.Count > 0) {
				foreach (Material face in faceMats) {
					ResizeMat (face, false, true);
				}
			}
		} 
	}

	void ResizeMat(Material material, bool isSide, bool isFace) {
		if (isSide) {
			material.mainTextureScale = new Vector2 (transform.localScale.x / scaleFactor, transform.localScale.y / scaleFactor);
		} else if (isFace) {
			material.mainTextureScale = new Vector2 (transform.localScale.y / scaleFactor, transform.localScale.z / scaleFactor);
		} else {
			material.mainTextureScale = new Vector2 (transform.localScale.x / scaleFactor, transform.localScale.z / scaleFactor);
		}
	}
}
