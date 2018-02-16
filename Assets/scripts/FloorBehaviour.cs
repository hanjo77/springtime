using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBehaviour : MonoBehaviour {

	public bool doRotate;
	public bool doTranslate;
	public bool isInfiniteRotation;
	public int rotationFrames = 500;
	public Vector3 startPosition;
	public Vector3 startRotation;
	public Vector3 targetPosition = Vector3.zero;
	public Vector3 targetRotation = Vector3.zero;
	public int moveFrames = 500;
	public GameObject floorTile;
	public int id;

	private int _translateFrameCount;
	private int _rotationFrameCount;
	private Vector3 _frameMotion;
	private Vector3 _frameRotation;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		startRotation = transform.rotation.eulerAngles;
		_frameMotion = (targetPosition - startPosition) / moveFrames;
		_frameRotation = (targetRotation - startRotation) / rotationFrames;
		floorTile = GetComponentInChildren<TextureResize> ().gameObject;
		if (id == 0) {
			id = GetInstanceID ();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_translateFrameCount++;
		_rotationFrameCount++;
		if (doTranslate) {
			transform.position += _frameMotion;
			if (_translateFrameCount >= moveFrames) {
				_frameMotion *= -1;
				_translateFrameCount = 0;
			}
		}
		if (doRotate) {
			transform.rotation *= Quaternion.Euler(_frameRotation);
			if (!isInfiniteRotation && _rotationFrameCount >= rotationFrames) {
				_frameRotation *= -1;
				_rotationFrameCount = 0;
			}
		}
	}
}
