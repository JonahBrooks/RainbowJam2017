using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerController : MonoBehaviour {

    public NPCController NPCToTrigger;
    public Collider2D triggerableObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == triggerableObject)
        {
            NPCToTrigger.Go();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
