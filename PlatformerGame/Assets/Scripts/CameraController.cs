using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Tooltip("The background (far) transform that should stay static with the camera.")]
    public Transform backgroundFar;
    [Tooltip("The background (medium) transform that should be moved with parallax scrolling.")]
    public Transform backgroundMedium;
    [Tooltip("The background (near) transform that should be moved with parallax scrolling.")]
    public Transform backgroundNear;
    [Tooltip("The background (far, back) transform that should stay static with the camera.")]
    public Transform backgroundFarBack;
    [Tooltip("The background (medium, back) transform that should be moved with parallax scrolling.")]
    public Transform backgroundMediumBack;
    [Tooltip("The background (near, back) transform that should be moved with parallax scrolling.")]
    public Transform backgroundNearBack;
    [Tooltip("The player the camera should follow.")]
    public Transform target;
    [Tooltip("To what degree the background (medium) should scroll with parallax scrolling. Larger numbers indicate faster scrolling.")]
    public float parallaxFactor1;
    [Tooltip("To what degree the background (near) should scroll with parallax scrolling. Larger numbers indicate faster scrolling.")]
    public float parallaxFactor2;
    [Tooltip("Whether parallax scrolling should be enabled.")]
    public bool parallaxEnabled;

    private bool usingLerp;

    void Update()
    {
        if (!usingLerp) {
            // Move camera to stay centered on player
            transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
        } 

        // Keep backgroundFar in camera at all times
        backgroundFar.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        backgroundFarBack.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        if (parallaxEnabled)
        {
            // Move background to create a parallax effect
            backgroundMedium.transform.position = new Vector3(backgroundMedium.transform.position.x, -transform.position.y, backgroundMedium.transform.position.z) * -parallaxFactor1;
            backgroundNear.transform.position = new Vector3(backgroundNear.transform.position.x, -transform.position.y, backgroundNear.transform.position.z) * -parallaxFactor2;
            backgroundMediumBack.transform.position = new Vector3(backgroundMedium.transform.position.x, -transform.position.y, backgroundMedium.transform.position.z) * -parallaxFactor1;
            backgroundNearBack.transform.position = new Vector3(backgroundNear.transform.position.x, -transform.position.y, backgroundNear.transform.position.z) * -parallaxFactor2;
        }
    }


    public void ChangeTargetWithLerp(Transform newTarget, float transitionDuration) {
        var player = GameObject.Find("Player");        
        if (newTarget != player.transform) {
            // Kill the player's input on the player character, and allow
            // for some extra time for this to finish.
            player.GetComponent<PlayerCharacter>().enabled = false;
            player.GetComponent<NPCCharacter>().enabled = true;
            player.GetComponent<NPCCharacter>().Stop();
            transitionDuration += 1.0f;
        }

        usingLerp = true;
        target = newTarget;
        StartCoroutine(Transition(transitionDuration));
    }

    
    IEnumerator Transition(float transitionDuration) {
        // Give the player time to come to rest.
        yield return new WaitForSeconds(1.0f);

        float t = 0.0f;
        Vector3 startingPos = transform.position;
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale/transitionDuration);

            var newPosition = Vector3.Lerp(startingPos, target.position, t);
            newPosition.x = 0;
            newPosition.z = -10;
            transform.position = newPosition;
            yield return 0;
        }

        usingLerp = false;

        // Return player control.
        var player = GameObject.Find("Player");
        if (target == player.transform) {   
            player.GetComponent<PlayerCharacter>().enabled = true;
            player.GetComponent<NPCCharacter>().enabled = false;
        }
    }

}
