using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorResetTrigger : MonoBehaviour {

    public Switchable[] toReset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(Switchable obj in toReset)
        {
            if(obj != null)
            {
                obj.Reset();
            }

        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
