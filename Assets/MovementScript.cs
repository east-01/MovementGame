using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementScript : MonoBehaviour
{

	static float P_H_ACC = 20;
	static float P_V_ACC = 50;
	static float P_MAX_SPEED = 30;
	static float P_JUMP_POWER = 20;
	static float P_TERMINAL_VEL = -80;

	public Transform cameraTransform;

	[SerializeField]
	private float hSpeed, vSpeed;
	[SerializeField]
	private float controller_hAxis, controller_vAxis;
	[SerializeField]
	private float angle;

	// Update is called once per frame
	void Update()
	{

		controller_hAxis = Input.GetAxis("Horizontal");
		controller_vAxis = Input.GetAxis("Vertical");
		bool isMoving = controller_hAxis != 0 || controller_vAxis != 0;

		if (isMoving) {
			// The angle that the stick is pointing
			angle = Mathf.Atan2(controller_hAxis, controller_vAxis) * Mathf.Rad2Deg;
			hSpeed = Mathf.Clamp(hSpeed + (P_H_ACC * Time.deltaTime), -P_MAX_SPEED, P_MAX_SPEED);
		} else { 
			if(hSpeed > 0) { 
				// TODO: Set state to sliding
				hSpeed -= 2 * P_H_ACC * Time.deltaTime;
			}
			if(hSpeed < 0) hSpeed = 0;
		}

		if(isGrounded()) { 
			vSpeed = 0;
		} else { 
			if(vSpeed > P_TERMINAL_VEL) { 
				vSpeed -= P_V_ACC * Time.deltaTime;
			}
			if(vSpeed < P_TERMINAL_VEL) vSpeed = P_TERMINAL_VEL;
		}

		if(isGrounded() && Input.GetKeyDown(KeyCode.Space)) { 
			vSpeed = P_JUMP_POWER;	
		}

		// Grab the camera yaw, this is the angle that rotates around the vertical axis
		float cam_yaw = cameraTransform.eulerAngles.y;
		// The yaw that the player is supposed to point
		float player_yaw = cam_yaw + angle;
		transform.rotation = Quaternion.Euler(new Vector3(0f, player_yaw, 0f));

		transform.Translate(new Vector3(0, vSpeed, hSpeed) * Time.deltaTime);

	}

	bool isGrounded() {
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		RaycastHit hit;
		float distanceToObstacle = 0;
		if (Physics.SphereCast(transform.position, collider.radius, Vector3.down, out hit, 10)) {
			distanceToObstacle = hit.distance;
		} else { // Cast didn't hit any collider, means we are NOT grounded
			return false;
		}
		print(distanceToObstacle);
		return distanceToObstacle < collider.radius;
	}

}
