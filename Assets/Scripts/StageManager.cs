using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    void Update() {
        if(transform.position.y < -10) { 
			print("You died.");
			CharacterController controller = GetComponent<CharacterController>();
			controller.enabled = false;
			transform.position = new Vector3(0, 10, 0);
			controller.enabled = true;
			GetComponent<MovementScript>().setMovementState(MovementState.IDLE);
		}
    }
}
