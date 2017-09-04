using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotationController : MonoBehaviour {

    [Tooltip("Delay in seconds between rotation chunks.")]
    public float speed;
    [Tooltip("Delta degrees of chunk size to rotate each tic.")]
    public float delta;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(ConstantRotationCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ConstantRotationCoroutine()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, delta));
            yield return new WaitForSeconds(speed);
        }
    }
}
