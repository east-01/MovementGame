using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public Transform target;
	public float distance = 5.0f;
	public float height = 2.0f;
	public float smoothSpeed = 0.5f;

	private Vector3 offset;

	void Start() {
		offset = transform.position - target.position;
	}

	void LateUpdate() {
		Vector3 desiredPosition = target.position + (-target.forward * distance) + (Vector3.up * height);
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

		transform.position = smoothedPosition;
		transform.LookAt(target.position);
	}
}
