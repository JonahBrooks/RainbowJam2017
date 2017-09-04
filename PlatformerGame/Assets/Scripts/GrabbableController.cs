using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableController : MonoBehaviour {

    [Tooltip("Pivot point to rotate.")]
    public Transform objectToRotate;
    [Tooltip("Degree to which Pivot should rotate. (0 to 359)")]
    public float degree;
    [Tooltip("Delay in seconds between rotation chunks.")]
    public float speed;
    [Tooltip("Delta degrees of chunk size to rotate each tic.")]
    public float delta;
    [Tooltip("Player character game object to be parented to this object.")]
    public GameObject player;
    [Tooltip("The object to be the new parent to player.")]
    public GameObject objectToBeParent;
    [Tooltip("The speed threshold at which the sound effect for draging should start or stop.")]
    public float soundEffectSpeedThreshold;

    private AudioSource soundEffects;
    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        soundEffects = GetComponent<AudioSource>();
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        // If this object is moving and the sound effect is not playing
		if(Mathf.Abs(rb2d.velocity.x) > soundEffectSpeedThreshold && !soundEffects.isPlaying)
        {
            soundEffects.Play();
        }// If this object has stopped and the sound effect is still playing
        else if (Mathf.Abs(rb2d.velocity.x) < soundEffectSpeedThreshold && soundEffects.isPlaying)
        {
            soundEffects.Stop();
        }
	}

    public void Grab()
    {
        if(objectToRotate != null)
        {
            // Turn on friction if this wall is going to be horizontal at some point
            GetComponent<Rigidbody2D>().sharedMaterial = null;
            StartCoroutine(RotateCoroutine());
        }
        // Parent the player to this object so the player can follow its movement.
        if (objectToBeParent != null && player != null)
        {
            player.transform.parent = objectToBeParent.transform;
        }
        // Create a new Fixed Joint 2D if none exists on this object
        if(GetComponent<FixedJoint2D>() == null)
        {
            gameObject.AddComponent<FixedJoint2D>();
        }
        // Adjust the mass to make it pushable
        gameObject.GetComponent<Rigidbody2D>().mass = 1;
    }

    public void Release()
    {
        if(objectToBeParent != null && player  != null)
        {
            player.transform.parent = null;
        }
        // Destroy teh fixed joint 2D so the physics work on this object again
        Destroy(gameObject.GetComponent<FixedJoint2D>());
        // Adjust the mass to make it unpushable
        gameObject.GetComponent<Rigidbody2D>().mass = 100;
    }


    IEnumerator RotateCoroutine()
    {
        while(Mathf.Abs(objectToRotate.eulerAngles.z - degree) > Mathf.Abs(delta))
        {
            yield return new WaitForSeconds(speed);
            objectToRotate.Rotate(new Vector3(0.0f, 0.0f, delta)); ;
        }
    }

}
