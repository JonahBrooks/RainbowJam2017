using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumController : MonoBehaviour {

    [Tooltip("Degree to which Pendulum should rotate first. (0 to 359)")]
    public float degree1;
    [Tooltip("Degree to which Pendulum should rotate second. (0 to 359)")]
    public float degree2;
    [Tooltip("Delay in seconds between rotation chunks.")]
    public float speed;
    [Tooltip("Delta degrees of chunk size to rotate each tic.")]
    public float delta;
    [Tooltip("End of the pendulum to keep straight up in world coordinates.")]
    public Transform end;

    private bool isGoingBack;
    // Use this for initialization
    void Start () {
        isGoingBack = false;
        StartCoroutine(PendulumCoroutine());
	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    IEnumerator PendulumCoroutine()
    {
        while(true)
        {
            if (isGoingBack)
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, -delta));
                end.Rotate(new Vector3(0.0f, 0.0f, delta));
                if (Mathf.Abs(transform.eulerAngles.z - degree2) <= delta)
                {
                    isGoingBack = false;
                }
            }
            else
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, delta));
                end.Rotate(new Vector3(0.0f, 0.0f, -delta));
                if (Mathf.Abs(transform.eulerAngles.z - degree1) <= delta)
                {
                    isGoingBack = true;
                }
            }
            yield return new WaitForSeconds(speed);
        }
    }

}
