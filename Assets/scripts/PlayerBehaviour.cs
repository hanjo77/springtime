using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour {

	public float motionForce = 2;
	public float maxSpeed = 5;
	public float acceleratorForce = 5;
	public float rotationSpeed = 1;
	public float acceleratorRotationSpeed = 5;
	public float jumpForce = 3;
	public GameObject lightSource;
	public GameObject levelEndCanvas;
	public GameObject hudCanvas;
	public float lightHeight = 3;
	public float lightDistance = 10;
	public float edge = -5;
	public float gravity = -10;
	public float sluggishness = .1f;
	public float slowDown = .2f;
	public Text scoreText;
	public Text timeText;
	public AudioClip jumpSound;
	public AudioClip coinSound;


	private Vector2 _motion = new Vector2 (0, 0);
	private Rigidbody _rigidbody;
	private GameObject _otherLight;
	private Quaternion _playerRotation;
	private Vector3 _startPosition;
	private bool _doJump;
	private bool _goalReached;
	private long _lastJumpTime;
	private float _speed = 0;
	private float _floorHeight = 0;
	private bool _showFrontView = false;
	private Vector3 _targetPosition = Vector3.zero;
	private DateTime _startTime;
	private Int32 _coins;
	private Vector3 _defaultAcceleration;
	private AudioSource _audioSource;
	private DateTime _pauseStart;

	// Use this for initialization
	void Start () {
		if (lightSource) {
			_rigidbody = GetComponent<Rigidbody> ();
			_otherLight = Instantiate(lightSource, new Vector3(0, .5f, 0), Quaternion.identity);
			_startPosition = transform.position;
		}
		_startTime = DateTime.Now;
		_defaultAcceleration = Input.acceleration;
		_audioSource = GetComponent<AudioSource> ();
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (lightSource && !_showFrontView) {
			lightSource.transform.position = new Vector3 (
				transform.position.x,
				transform.position.y + 10,
				transform.position.z
			);

			if (_doJump) {
				_rigidbody.AddForce (0, jumpForce, 0, ForceMode.Impulse);
				_doJump = false;
				_audioSource.clip = jumpSound;
				_audioSource.Play ();
			}

			_rigidbody.AddForce (_motion.x, 0, _motion.y, ForceMode.VelocityChange);
			_rigidbody.AddForce (0, gravity, 0, ForceMode.Force);
		} 
		else if (_targetPosition.magnitude > 0) {
			transform.position = Vector3.Lerp (transform.position, _targetPosition, sluggishness);
		}

		transform.rotation *= Quaternion.AngleAxis ((Input.GetAxis ("Horizontal") * rotationSpeed) + ((Input.acceleration.x - _defaultAcceleration.x) * acceleratorRotationSpeed), Vector3.up);

		if (_otherLight) {
			Vector3 offset = transform.forward * lightDistance;
			if (_showFrontView) {
				offset = new Vector3(-offset.x, 0, -offset.z);
			}

			_otherLight.transform.position = transform.position + offset + new Vector3 (0, 2 * lightHeight, 0);
			_otherLight.transform.LookAt (this.transform.position + new Vector3 (0, lightHeight, 0));
		}

		if (scoreText && timeText) {
			scoreText.text = _coins.ToString ();
			TimeSpan timeElapsed = DateTime.Now - _startTime;
			timeText.text = string.Format("{0:00}:{1:00}", timeElapsed.Minutes, timeElapsed.Seconds);
		}

		if (transform.position.y < edge) {
			transform.position = _startPosition;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.ResetInertiaTensor ();
			SceneManager.LoadScene ("title", LoadSceneMode.Single);
		}

		if (_goalReached) {
			StartCoroutine(EndLevel ());
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag.Equals ("Coin")) {
			_audioSource.clip = coinSound;
			_audioSource.Play ();
		}
		Destroy(other.gameObject);
		_coins++;
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
			_motion = new Vector3 (0, 0, 0);
			_showFrontView = true;
			_targetPosition = collisionInfo.gameObject.transform.position;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.ResetInertiaTensor ();
			gravity = -1;
			levelEndCanvas.SetActive (true);
			hudCanvas.SetActive (false);
			_goalReached = true;
		}
	}

	public void Pause() {
		_pauseStart = DateTime.Now;
	}

	public void Play() {
		TimeSpan pauseTime = DateTime.Now - _pauseStart;
		_startTime += pauseTime;
	}

	IEnumerator EndLevel() {
		yield return new WaitForSeconds(5);
		transform.position = _startPosition;
		SceneManager.LoadScene ("title", LoadSceneMode.Single);
	}

	public float GetJumpHeight() {
		return transform.position.y - _floorHeight;
	}

	public void SetFrontView(bool showFrontView) {
		_showFrontView = showFrontView;
	}

	public bool IsFrontView() {
		return _showFrontView;
	}
}
