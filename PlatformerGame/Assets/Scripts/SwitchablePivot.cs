using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchablePivot : Switchable {

    [Tooltip("Degree to which Pivot should rotate. (0 to 359)")]
    public float degree;
    [Tooltip("Delay in seconds between rotation chunks.")]
    public float speed;
    [Tooltip("Delta degrees of chunk size to rotate each tic.")]
    public float delta;

    private bool isDone;

    // Use this for initialization
    void Start () {
        isDone = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isDone && isActive)
        {
            isDone = true;
            StartCoroutine(RotateCoroutine());
        }
	}

    IEnumerator RotateCoroutine()
    {
        while (Mathf.Abs(gameObject.transform.eulerAngles.z - degree) > Mathf.Abs(delta))
        {
            yield return new WaitForSeconds(speed);
            gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, delta)); ;
        }
    }
}
