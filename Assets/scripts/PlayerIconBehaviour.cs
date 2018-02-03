using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconBehaviour : MonoBehaviour {

	public Vector3 scale;
	public Quaternion rotation;
	public Vector3 position;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.localScale = scale;
		transform.rotation = rotation;
	}
}
