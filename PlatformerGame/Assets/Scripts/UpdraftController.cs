using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftController : Switchable {


    public override void Switch()
    {
        base.Switch();

        GetComponentInParent<ParticleSystem>().Play();
        
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
