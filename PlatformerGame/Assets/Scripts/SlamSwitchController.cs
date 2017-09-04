using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamSwitchController : Switchable {

    [Tooltip("The ElevatorController affected by this switch.")]
    public Switchable[] objectsToSwitch;
    [Tooltip("Whether or not the platform should go dynamic after being hit.")]
    public bool switchToDynamic;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Switch()
    {
        if(switchToDynamic)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        if(objectsToSwitch.Length > 0)
        {
            foreach(Switchable objectToSwitch in objectsToSwitch)
            {
                objectToSwitch.Switch();
            }
        }
    }

}
