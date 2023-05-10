using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour {
	public Transform target;
	public float distance = 5.0f;
	public float height = 2.0f;
	public float smoothSpeed = 0.5f;

	public static int CAMERA_POS_COUNT = 8;
	public static int CAMERA_DISTANCE = 5;
	public static int CAMERA_HEIGHT = 3;
	[SerializeField]
	private Vector3[] cameraOffsets;
	[SerializeField]
	private int cameraIndex;

	PlayerControls controls;

	private void OnEnable() { controls.Gameplay.Enable(); }
	private void OnDisable() { controls.Gameplay.Disable(); }

	private void Awake() {
		controls = new PlayerControls();

		controls.Gameplay.camera_left.performed += ctx => moveIndexBy(-1);
		controls.Gameplay.camera_right.performed += ctx => moveIndexBy(1);

	}

	void Start() {
		cameraOffsets = new Vector3[CAMERA_POS_COUNT];
		for(int i = 0; i < CAMERA_POS_COUNT; i++) { 
			float angle = (((float)i/(float)CAMERA_POS_COUNT) * 360f) * Mathf.Deg2Rad;
			cameraOffsets[i] = new Vector3(
				-Mathf.Cos(angle)*CAMERA_DISTANCE,
				CAMERA_HEIGHT,
				-Mathf.Sin(angle)*CAMERA_DISTANCE
			);
		}
		cameraIndex = 0;
	}

	void LateUpdate() {
//		Vector3 desiredPosition = target.position + (-target.forward * distance) + (Vector3.up * height);
//		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
//		transform.position = smoothedPosition;
		transform.position = target.position + cameraOffsets[cameraIndex];
		transform.LookAt(target.position);
	}

	public void moveIndexBy(int offset) { 
		cameraIndex += offset;
		if(cameraIndex <= -1) cameraIndex = CAMERA_POS_COUNT-1;
		if(cameraIndex >= CAMERA_POS_COUNT) cameraIndex = 0;
	}

}
