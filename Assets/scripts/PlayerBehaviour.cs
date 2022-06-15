using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Runtime.InteropServices;

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
	public int lives = 5;
	public float startHeight = 100;
	public bool isPlaying;
	public float setbackBorderOffset = 2.5f;

	public AudioClip jumpSound;
	public AudioClip coinSound;

	private Vector2 _motion = new Vector2 (0, 0);
	private Rigidbody _rigidbody;
	private Quaternion _playerRotation;
	private bool _doJump;
	private long _lastJumpTime;
	private Vector3 _lastPosition;
	private float _speed = 0;
	private float _floorHeight = 0;
	private Vector3 _targetPosition = Vector3.zero;
	private Vector3 _defaultAcceleration;
	private bool _setLastPosition;

	// Use this for initialization
	void Start () {
		if (lightSource) {
			_rigidbody = GetComponent<Rigidbody> ();
		}
		if (gameManager != null) {
			gameManager.livesText.text = "" + lives;
		}
		transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
		ResetAccelerator();
		isPlaying = false;
	}

    protected void OnEnable()
    {
        // All sensors start out disabled so they have to manually be enabled first.
		if (Accelerometer.current != null) {
	        InputSystem.EnableDevice(Accelerometer.current);
		}
		ResetAccelerator ();
    }

    protected void OnDisable()
    {
		if (Accelerometer.current != null) {
			InputSystem.DisableDevice(Accelerometer.current);
		}
    }

	Vector3 Acceleration () {
		Vector3 acceleration = new Vector3(0, 0, 0);
		if (Accelerometer.current != null) {
			Vector3 _acc = Accelerometer.current.acceleration.ReadValue();
			acceleration = new Vector3(
				(_acc.x - _defaultAcceleration.x) * acceleratorRotationSpeed,
				(_defaultAcceleration.y - _acc.y) * acceleratorForce,
				0
			);
		}
		return acceleration;
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

		PlayerInput _playerInput = GetComponent<PlayerInput>();
		InputAction _horizontalAction = _playerInput.actions["Horizontal"];
		float _horizontalValue = _horizontalAction.ReadValue<float>();

		transform.rotation *= Quaternion.AngleAxis ((_horizontalValue * rotationSpeed) + Acceleration().x, Vector3.up);
					
		if (gameManager != null && transform.position.y < gameManager.edge) {
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.ResetInertiaTensor ();
			ReduceLive();
		}

		RaycastHit hit;
		Vector3 down = new Vector3(0, -1, 0);
		if (_setLastPosition && Physics.Raycast (transform.position, down, out hit, 10)) {
			_lastPosition = new Vector3(transform.position.x, hit.point.y + startHeight, transform.position.z);
			_setLastPosition = false;
		}
	}

	void ReduceLive() {
		lives--;
		if (lives < 0) {
			gameManager.LoadScene ("title");
		}
		else {
			Reset ();
			gameManager.livesText.text = "" + lives;
			transform.position = _lastPosition;
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

			FloorBehaviour floor = null;
			if (collisionInfo.gameObject.transform.parent) {
				floor = collisionInfo.gameObject.transform.parent.gameObject.GetComponent<FloorBehaviour> ();
			}

			if (floor == null || !(floor.doTranslate || floor.doRotate)) {
				_setLastPosition = true;
			}

			PlayerInput _playerInput = GetComponent<PlayerInput>();
            InputAction _verticalAction = _playerInput.actions["Vertical"];
         	float _verticalValue = _verticalAction.ReadValue<float>() * -1;

			_speed *= 1 - slowDown;
			_speed -= ((_verticalValue * motionForce) - Acceleration().y);

			if (Math.Abs (_speed) > maxSpeed) {
				_speed = (_speed / Math.Abs (_speed)) * maxSpeed;
			}

			_motion = _speed * new Vector2 (transform.forward.x, transform.forward.z);

			if (_motion.magnitude <= .2f) {
				_motion = new Vector2 (0, 0);
			}
		}

		if (collisionInfo.transform.parent.gameObject.tag.Equals ("Goal")) {
			isPlaying = false;
			_targetPosition = collisionInfo.gameObject.transform.position;
			_lastPosition = Vector3.zero;
			_setLastPosition = false;
			// _otherLight = Instantiate(lightSource, new Vector3(0, .5f, 0), Quaternion.identity);
			_doJump = false;
			Reset ();
			gameManager.EnterGoal ();
		}
	}

	public void Reset() {
		_speed = 0;
		_motion = Vector3.zero;
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.ResetInertiaTensor ();
		ResetAccelerator();
	}

	public void ResetAccelerator() {
		_defaultAcceleration = Input.acceleration;
	}

	public float GetJumpHeight() {
		return transform.position.y - _floorHeight;
	}

	public Vector3 GetLastPosition() {
		return _lastPosition;
	}

}