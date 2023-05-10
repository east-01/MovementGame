using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CapsuleCollider coll = GetComponent<CapsuleCollider>();
		if(coll != null) { 
			coll.enabled = false;	
			print("removed bs");
		} else { 
			print("failed to find bs");	
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
