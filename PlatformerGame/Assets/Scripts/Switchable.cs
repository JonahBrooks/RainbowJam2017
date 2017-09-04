using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switchable : MonoBehaviour {

    [Tooltip("Whether or not this object should be currently active.")]
    public bool isActive;

    private bool hasSwitched;

    public virtual void Switch()
    {
        if(!hasSwitched)
        {
            hasSwitched = true;
            isActive = true;
        }
    }

    public virtual void Reset()
    {
        hasSwitched = false;
        
    }

    // Use this for initialization
    void Start () {
        hasSwitched = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
