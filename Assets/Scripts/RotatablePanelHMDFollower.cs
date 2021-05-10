﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatablePanelHMDFollower : MonoBehaviour
{
	private const float TOTAL_DURATION = 3.0f;
	private const float HMD_MOVEMENT_THRESHOLD = 0.3f;

	[SerializeField] private float _maxDistance = 0.3f;
	[SerializeField] private float _minDistance = 0.05f;
	[SerializeField] private float _minZDistance = 0.05f;

	private OVRCameraRig _cameraRig;
	private Vector3 _panelInitialPosition = Vector3.zero;
	private Coroutine _coroutine = null;
	private Vector3 _prevPos = Vector3.zero;
	private Vector3 _lastMovedToPos = Vector3.zero;

	private void Awake()
	{
		_cameraRig = FindObjectOfType<OVRCameraRig>();
		_panelInitialPosition = transform.position;
	}

	private void Update()
	{
		var centerEyeAnchorPos = _cameraRig.centerEyeAnchor.position;
		var myPosition = transform.position;
		//Distance from centereye since last time we updated panel position.
		float distanceFromLastMovement = Vector3.Distance(centerEyeAnchorPos, _lastMovedToPos);
		float headMovementSpeed = (_cameraRig.centerEyeAnchor.position - _prevPos).magnitude / Time.deltaTime;
		var currDiffFromCenterEye = transform.position - centerEyeAnchorPos;
		var currDistanceFromCenterEye = currDiffFromCenterEye.magnitude;


		// 1) wait for center eye to stabilize after distance gets too large
		// 2) check if center eye is too close to panel
		// 3) check if depth isn't too close
		if (((distanceFromLastMovement > _maxDistance) || (_minZDistance > currDiffFromCenterEye.z) || (_minDistance > currDistanceFromCenterEye)) &&
			headMovementSpeed < HMD_MOVEMENT_THRESHOLD && _coroutine == null)
		{
			if (_coroutine == null)
			{
				_coroutine = StartCoroutine(LerpToHMD());
			}
		}

		_prevPos = _cameraRig.centerEyeAnchor.position;
	}

	private Vector3 CalculateIdealAnchorPosition()
	{
		Vector3 cur = new Vector3(_panelInitialPosition.x, 0f, _panelInitialPosition.z);
		Vector3 target = new Vector3(_cameraRig.centerEyeAnchor.forward.x, 0f, _cameraRig.centerEyeAnchor.forward.z);
		Quaternion rot = new Quaternion();
		rot.SetFromToRotation(cur, target);
		Matrix4x4 rotMatrix = Matrix4x4.Rotate(rot);
		return _cameraRig.centerEyeAnchor.position + rotMatrix.MultiplyPoint(_panelInitialPosition);
	}

	private IEnumerator LerpToHMD()
	{
		Vector3 cur = new Vector3(_panelInitialPosition.x, 0f, _panelInitialPosition.z);
		Vector3 target = new Vector3(_cameraRig.centerEyeAnchor.forward.x, 0f, _cameraRig.centerEyeAnchor.forward.z);
		Quaternion rot = new Quaternion();
		rot.SetFromToRotation(cur, target);
		Matrix4x4 rotMatrix = Matrix4x4.Rotate(rot);
		transform.LookAt(new Vector3(_cameraRig.centerEyeAnchor.position.x, transform.position.y,_cameraRig.centerEyeAnchor.position.z));
		Vector3 newPanelPosition = _cameraRig.centerEyeAnchor.position + rotMatrix.MultiplyPoint(_panelInitialPosition);
		_lastMovedToPos = _cameraRig.centerEyeAnchor.position;
		float startTime = Time.time;
		float endTime = Time.time + TOTAL_DURATION;

		while (Time.time < endTime)
		{
			transform.position =
			  Vector3.Lerp(transform.position, newPanelPosition, (Time.time - startTime) / TOTAL_DURATION);
			yield return null;
		}

		transform.position = newPanelPosition;
		_coroutine = null;
	}
}
