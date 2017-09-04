using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : Switchable {


    [Tooltip("Position in the world for this platform to start.")]
    public Vector2 startPos;
    [Tooltip("Position in the world for this platform to end up.")]
    public Vector2 endPos;
    [Tooltip("The speed at which this platform should move.")]
    public float speedDelta;
    [Tooltip("The amount of time in seconds the platform should pause at top and bottom.")]
    public float pauseDelay;
    [Tooltip("Distance between an acceptable end point and the actual end point.")]
    public float switchDelta;
    [Tooltip("Whether this platform should make a return trip after reaching the end.")]
    public bool isOneWay;


    private bool isGoingBack;
    private bool startedActive;
    private bool firstRun;
    private float distX;
    private float distY;
    private float time;
    private float pingPong;
    private float lastPingPong;

    private void setIsGoingBack(bool set)
    {
        if (isGoingBack != set)
        {
            isGoingBack = set;
            if (isOneWay)
            {
                isActive = false;
            }
            if (isActive == true)
            {
                // Only call this coroutine if active is true, otherwise it sets it to true at the end
                StartCoroutine(PauseDelayCoroutine());
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        transform.localPosition = startPos;
        isGoingBack = false;
        isActive = startedActive;
        firstRun = true;
        time = 0.0f;
        lastPingPong = 0.0f;
    }

    // Use this for initialization
    void Start () {
        transform.localPosition = startPos;
        isGoingBack = false;
        startedActive = isActive;
        firstRun = true;
        time = 0.0f;
        lastPingPong = 0.0f;
        distX = endPos.x - startPos.x;
        distY = endPos.y - startPos.y;
    }
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            if(firstRun)
            {
                StartCoroutine(PauseDelayCoroutine());  // Start paused if there is a pause delay
                firstRun = false;
            }

            time += Time.deltaTime;
            pingPong = Mathf.PingPong(time * speedDelta, 1);
            transform.localPosition = startPos + new Vector2(pingPong*distX, pingPong*distY);
            if(lastPingPong - pingPong < 0)
            {
                setIsGoingBack(true);
            }
            else
            {
                setIsGoingBack(false);
            }
            lastPingPong = pingPong;
        }
	}

    IEnumerator PauseDelayCoroutine()
    {
        isActive = false;
        yield return new WaitForSeconds(pauseDelay);
        isActive = true;
        
    }
}
