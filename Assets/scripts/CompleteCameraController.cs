using UnityEngine;
using System.Collections;
using System;

public class CompleteCameraController : MonoBehaviour {

	public GameObject player;       //Public variable to store a reference to the player game object
	public float distance = 10;
	public float heightOffset = 5;
	public float focusHeight = 2;
	public float sluggishness = .1f;
	public AudioClip goalTune;

	private PlayerBehaviour _playerBehaviour;
	private Vector3 _focusOffset;
	private AudioSource _audioSource;

	// Use this for initialization
	void Start () 
	{
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		_playerBehaviour = player.GetComponent<PlayerBehaviour> ();
		_focusOffset = new Vector3 (0, focusHeight, 0);
		_audioSource = GetComponent<AudioSource>();
	}

	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		// transform.position = player.transform.position + offset;
		float jumpHeight = _playerBehaviour.GetJumpHeight();
		Vector3 distanceVector = player.transform.forward;
		if (jumpHeight < 0) {
			jumpHeight = 0;
		}

		if (_playerBehaviour.IsFrontView ()) {
			distanceVector *= -1;
			if (_audioSource.loop) {
				_audioSource.clip = goalTune;
				_audioSource.loop = false;
				_audioSource.Play ();
			}
		}
		Vector3 targetPos = player.transform.position + distanceVector * 10;
		targetPos.y = player.transform.position.y + heightOffset + Math.Abs(_playerBehaviour.GetJumpHeight());
		transform.position = Vector3.Lerp (transform.position, targetPos, sluggishness);
		transform.LookAt (player.transform.position + _focusOffset);
		// _camera.fieldOfView = (20000 / Screen.width) + 25;
	}
}
