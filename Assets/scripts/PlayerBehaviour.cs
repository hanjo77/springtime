using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerBehaviour : MonoBehaviour {

	public GameBehaviour gameManager;

	public float motionForce = 2;
	public float maxSpeed = 5;
	public float acceleratorForce = 5;
	public float rotationSpeed = 1;
	public float acceleratorRotationSpeed = 5;
	public float jumpForce = 3;
	public GameObject lightSource;
	public float lightHeight = 3;
	public float lightDistance = 10;
	public float sluggishness = .1f;
	public float slowDown = .2f;
	public float gravity = -10;

	public AudioClip jumpSound;
	public AudioClip coinSound;

	private Vector2 _motion = new Vector2 (0, 0);
	private Rigidbody _rigidbody;
	private GameObject _otherLight;
	private Quaternion _playerRotation;
	private Vector3 _startPosition;
	private bool _doJump;
	private long _lastJumpTime;
	private float _speed = 0;
	private float _floorHeight = 0;
	private Vector3 _targetPosition = Vector3.zero;
	private Vector3 _defaultAcceleration;

	// Use this for initialization
	void Start () {
		if (lightSource) {
			_rigidbody = GetComponent<Rigidbody> ();
			_otherLight = Instantiate(lightSource, new Vector3(0, .5f, 0), Quaternion.identity);
			_startPosition = transform.position;
		}
		_defaultAcceleration = Input.acceleration;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (lightSource && gameManager != null && !gameManager.IsFrontView ()) {
			lightSource.transform.position = new Vector3 (
				transform.position.x,
				transform.position.y + 10,
				transform.position.z
			);

			if (_doJump) {
				_rigidbody.AddForce (0, jumpForce, 0, ForceMode.Impulse);
				_doJump = false;
				gameManager.PlayAudioClip (jumpSound);
			}

			_rigidbody.AddForce (_motion.x, 0, _motion.y, ForceMode.VelocityChange);
			_rigidbody.AddForce (0, gravity, 0, ForceMode.Force);
		} else if (_targetPosition.magnitude > 0) {
			transform.position = Vector3.Lerp (transform.position, _targetPosition, sluggishness);
		} else {
			Reset ();
		}

		transform.rotation *= Quaternion.AngleAxis ((Input.GetAxis ("Horizontal") * rotationSpeed) + ((Input.acceleration.x - _defaultAcceleration.x) * acceleratorRotationSpeed), Vector3.up);

		if (_otherLight) {
			Vector3 offset = transform.forward * lightDistance;
			if (gameManager != null && gameManager.IsFrontView()) {
				offset = new Vector3(-offset.x, 0, -offset.z);
			}

			_otherLight.transform.position = transform.position + offset + new Vector3 (0, 2 * lightHeight, 0);
			_otherLight.transform.LookAt (this.transform.position + new Vector3 (0, lightHeight, 0));
		}
			
		if (gameManager != null && transform.position.y < gameManager.edge) {
			transform.position = _startPosition;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.ResetInertiaTensor ();
			gameManager.LoadScene ("title");
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag.Equals ("Coin")) {
			gameManager.PlayAudioClip (coinSound);
		}
		Destroy(other.gameObject);
		gameManager.AddCoin ();
	}

	void OnCollisionEnter(Collision collisionInfo) {

		_floorHeight = transform.position.y;

		if (DateTime.Now.Ticks - _lastJumpTime > 1000000) {
			_doJump = true;
			_lastJumpTime = DateTime.Now.Ticks;

			_speed *= 1 - slowDown;
			_speed -= ((Input.GetAxis ("Vertical") * motionForce) - ((Input.acceleration.z - _defaultAcceleration.x) * acceleratorForce));

			if (Math.Abs (_speed) > maxSpeed) {
				_speed = (_speed / Math.Abs (_speed)) * maxSpeed;
			}

			_motion = _speed * new Vector2 (transform.forward.x, transform.forward.z);

			if (_motion.magnitude <= .2f) {
				_motion = new Vector2 (0, 0);
			}
		}

		if (collisionInfo.gameObject.tag.Equals ("Goal")) {
			_targetPosition = collisionInfo.gameObject.transform.position;
			Reset ();
			gameManager.EnterGoal ();
		}
	}

	public void Reset() {
		_motion = new Vector3 (0, 0, 0);
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.ResetInertiaTensor ();
	}

	public float GetJumpHeight() {
		return transform.position.y - _floorHeight;
	}
}
