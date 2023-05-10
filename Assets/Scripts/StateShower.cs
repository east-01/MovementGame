using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateShower : MonoBehaviour {
	
	private MovementScript movementScript;
	private CharacterController characterController;
	private MovementState lastState;

    void Start() {
        movementScript = GetComponent<MovementScript>();
		if(movementScript == null) { 
			Debug.LogError("StateShower: Tried to get the movement script but this component doesn't seem to have one.");
			this.enabled = false;
		}
		characterController = GetComponent<CharacterController>();
		if(characterController == null) { 
			Debug.LogError("StateShower: Tried to get the character controller but this component doesn't seem to have one.");
			this.enabled = false;		
		}
    }

    void Update() {
		MovementState state = movementScript.getMovementState();
		// If there's no state change, return
		if(lastState == state) return;

		if(state == MovementState.IDLE || state == MovementState.WALKING || state == MovementState.RUNNING) { 
			transform.localScale = Vector3.one;
			characterController.center = new Vector3(0, 0, 0);
		} else if(state == MovementState.DIVING) { 
			transform.localScale = new Vector3(1, 0.5f, 2);
			characterController.center = new Vector3(0, 1, 0);
		}

		lastState = state;
    }

}
