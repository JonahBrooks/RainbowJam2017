using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingShatterController : MonoBehaviour {

    [Tooltip("Audio source through which to play the explosion sound effect.")]
    public AudioSource sfx;
    [Tooltip("The audio clip for the explosion sound effect")]
    public AudioClip explosionClip;
    [Tooltip("The minimum amount of force to be applied to each ceiling tile.")]
    public float minForce;
    [Tooltip("The maximum amount of force to be applied to each ceiling tile.")]
    public float maxForce;
    [Tooltip("The minimum x direction of the force to be applied to each ceiling tile.")]
    public float minX;
    [Tooltip("The maximum x direction of the force to be applied to each ceiling tile.")]
    public float maxX;
    [Tooltip("The minimum y direction of the force to be applied to each ceiling tile.")]
    public float minY;
    [Tooltip("The maximum y direction of the force to be applied to each ceiling tile.")]
    public float maxY;
    [Tooltip("The amount of time before the explosion happens. (NOTE: Sync this with the NPC scripts)")]
    public float initialWaitTime;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(CeilingShatterCoroutine());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator CeilingShatterCoroutine()
    {
        // Wait
        yield return new WaitForSeconds(initialWaitTime);

        // Drop tiles
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        // Fling tiles
        rb2d.AddForce(Random.Range(minForce, maxForce) * new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY)));
        if(sfx != null && explosionClip != null && !sfx.isPlaying)
        {
            sfx.clip = explosionClip;
            sfx.loop = false;
            sfx.Play();
        }

    }
}
