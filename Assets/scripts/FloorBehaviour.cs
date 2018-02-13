using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBehaviour : MonoBehaviour {

	public bool doRotate;
	public bool doTranslate;
	public int rotationFrames = 0;
	public float rotationAngle = 0;
	public bool isRotationCounterClockwise = false;
	public Vector3 targetPosition = Vector3.zero;
	public Vector3 targetRotation = Vector3.zero;
	public int moveFrames = 1000;

	private Vector3 _startPosition;
	private Vector3 _startRotation;
	private bool _hasReachedTarget;
	private bool _hasReachedRotation;
	private bool _startTime;
	private int _frameCount;
	private Vector3 _frameMotion;

	// Use this for initialization
	void Start () {
		_startPosition = transform.position;
		_startRotation = transform.rotation.eulerAngles;
		_frameMotion = (targetPosition - _startPosition) / moveFrames;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_frameCount++;
		if (doTranslate) {
			if (!_hasReachedTarget) {
				transform.position += _frameMotion;
				if (_frameCount >= moveFrames) {
					_hasReachedTarget = true;
					_frameCount = 0;
				}
			} else {
				transform.position -= _frameMotion;
				if (_frameCount >= moveFrames) {
					_hasReachedTarget = false;
					_frameCount = 0;
				}
			}
		}
		if (doRotate) {
			if (rotationAngle > 0 && rotationAngle < 360) {

			}
		}
	}
}
