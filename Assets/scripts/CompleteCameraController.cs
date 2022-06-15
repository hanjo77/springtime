using UnityEngine;
using System.Collections;
using System;

public class CompleteCameraController : MonoBehaviour {

	public GameObject player;       //Public variable to store a reference to the player game object
	public GameBehaviour gameManager;
	public float distance = 10;
	public float heightOffset = 5;
	public float focusHeight = 2;
	public float sluggishness = .1f;
	public int mobileFieldOfView = 100;
	public int desktopFieldOfView = 60;

	private Vector3 _focusOffset;
	private PlayerBehaviour _playerBehaviour;

	// Use this for initialization
	void Start () 
	{
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		_playerBehaviour = player.GetComponent<PlayerBehaviour> ();
		_focusOffset = new Vector3 (0, focusHeight, 0);
	}

	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
		if (Screen.width < 900) {
			GetComponent<Camera>().fieldOfView = mobileFieldOfView;
		} else {
			GetComponent<Camera>().fieldOfView = desktopFieldOfView;
		}
		if (_playerBehaviour) {
			// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
			// transform.position = player.transform.position + offset;
			float jumpHeight = _playerBehaviour.GetJumpHeight();
			Vector3 distanceVector = player.transform.forward;
			if (jumpHeight < 0) {
				jumpHeight = 0;
			}

			if (gameManager.IsFrontView ()) {
				distanceVector *= -1;
			}
			Vector3 targetPos = player.transform.position + distanceVector * 10;
			targetPos.y = player.transform.position.y + heightOffset + Math.Abs(_playerBehaviour.GetJumpHeight());
			if (_playerBehaviour.isPlaying || gameManager.IsFrontView ()) {
				transform.position = Vector3.Lerp (transform.position, targetPos, sluggishness);
			} else {
				transform.position = targetPos;
				_playerBehaviour.isPlaying = true;
			}
			transform.LookAt (player.transform.position + _focusOffset);
			// _camera.fieldOfView = (20000 / Screen.width) + 25;
		}
	}
}
