using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementScript : MonoBehaviour {

	static readonly float P_H_ACC = 45;
	static readonly float P_MAX_SPEED = 15;
	static readonly float P_JUMP_V_SPEED = 30f;
	static readonly float P_JUMP_POWER = 30; // The amount the player can hold the jump button until the jump stops ascending
	static readonly float P_KICK_V_SPEED = 15;
	static readonly float P_DIVE_THRESHOLD = 12;
	static readonly float P_DIVE_H_SPEED = 30;

	static readonly float ACC_GRAV = 120;
	static readonly float TERMINAL_VEL = -120;

	public Transform cameraTransform;
	PlayerControls controls;

	private CharacterController controller;

	// Input variables
	Vector2 leftStickAxes;
	[SerializeField]
	private float playerAngle, stickAngle; // Angle of the player and stick
	// World variables
	[SerializeField]
	private MovementState state;
	[SerializeField]
	private float hSpeed, vSpeed;
	private bool jumpPressed;
	[SerializeField]
	private float jumpPower;

	private void OnEnable() { controls.Gameplay.Enable(); }
	private void OnDisable() { controls.Gameplay.Disable(); }

	private void Awake() {

		state = MovementState.IDLE;

		controls = new PlayerControls();
		controller = GetComponent<CharacterController>();

		// Jump pressed/released
		controls.Gameplay.jump.performed += ctx => {
			if(controller.isGrounded) {
				vSpeed = P_JUMP_V_SPEED;
			}
			jumpPower = P_JUMP_POWER;
			jumpPressed = true;
		};
		controls.Gameplay.jump.canceled += ctx => {
			jumpPower = 0;
			jumpPressed = false; 
		};

		// Attack pressed
		controls.Gameplay.attack.performed += ctx => { 
			if(hSpeed >= P_DIVE_THRESHOLD) { 
				// Dive
				state = MovementState.DIVING;
				hSpeed = P_DIVE_H_SPEED;
			} else { 
				// Kick
				vSpeed = P_KICK_V_SPEED; 
			}
		};
	}

	void Update() {

		leftStickAxes = controls.Gameplay.move.ReadValue<Vector2>();

		if(Mathf.Abs(leftStickAxes.x) >= 0.1 || Mathf.Abs(leftStickAxes.y) >= 0.1) { 
			activeMovement();		
		} else { 
			passiveMovement();
		}

		// Manage jump power
		if(jumpPower > 0) { 
			jumpPower -= 100 * Time.deltaTime;	
		} else { 
			jumpPower = 0;
		}

		// Manage player falling
		if(controller.isGrounded && jumpPower == 0) { 
			vSpeed = -1f; // vSpeed needs to always be -1 to maintain contact with the ground for the CharacterController
		} else { 
			if(vSpeed > TERMINAL_VEL) { 
				float accGrav = jumpPower > 0 ? ACC_GRAV/jumpPower : ACC_GRAV;
				vSpeed -= accGrav * Time.deltaTime;
			} else { 
				vSpeed = TERMINAL_VEL;
			}
		}
		
		// Actually move the player		
		float player_yaw = transform.eulerAngles.y;
		controller.Move(new Vector3(hSpeed * Mathf.Sin(player_yaw*Mathf.Deg2Rad), vSpeed, hSpeed * Mathf.Cos(player_yaw*Mathf.Deg2Rad)) * Time.deltaTime);

	}

	public void activeMovement() { 
	
		// The angle that the stick is pointing
		stickAngle = Mathf.Atan2(leftStickAxes.x, leftStickAxes.y) * Mathf.Rad2Deg;
	
		if(controller.isGrounded && state != MovementState.DIVING && state != MovementState.ROLL) {
					
			float cam_yaw = cameraTransform.eulerAngles.y;
			float player_yaw = transform.eulerAngles.y;
			// The target direction that the player wants to point
			float targetAngle = cam_yaw + stickAngle;
			float angleDifference = Mathf.Abs(player_yaw-targetAngle);

			print("cam: " + cam_yaw + ", stick: " + stickAngle + ", target: " + targetAngle + ", player_yaw: " + player_yaw + ", diff: " + angleDifference);

			//float new_player_yaw = player_yaw + Mathf.Sign(targetAngle-player_yaw) * 5 * Mathf.Clamp((angleDifference/90),0,1);
			float new_player_yaw = Mathf.LerpAngle(player_yaw, targetAngle, 50 * Mathf.Clamp((angleDifference/90),0,1) * Time.deltaTime);

			transform.rotation = Quaternion.Euler(0, new_player_yaw, 0);

		}

		// If the player speed is greater than the max speed, apply friction to decelerate
		if(Mathf.Abs(hSpeed) > P_MAX_SPEED) { 
			hSpeed += -Mathf.Sign(hSpeed) * P_H_ACC * Time.deltaTime;
		} else {
			hSpeed = Mathf.Clamp(hSpeed + (P_H_ACC * Time.deltaTime), -P_MAX_SPEED, P_MAX_SPEED);
		}

	}

	public void passiveMovement() { 
	
		// Passive sliding
		if(hSpeed > 0) { 
			// TODO: Set state to sliding
			hSpeed -= 2 * P_H_ACC * Time.deltaTime;
		} else { 
			hSpeed = 0;
		}
	}

	public MovementState getMovementState() { 
		return state;
	}

}

public enum MovementState { 
	IDLE, WALKING, RUNNING, DIVING, ROLL
}