using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    [Tooltip("PlayerController used for accessing world modifiers like gravity and max speed.")]
    public PlayerController pc;
    [Tooltip("Sprite to be used by while walking about.")]
    public Sprite mainBodySprite;
    [Tooltip("Sprite to be used by while taking special actions.")]
    public Sprite specialActionBodySprite;
    [Tooltip("Sprite for the heart speech bubble.")]
    public Sprite heartSpeechBubble;
    [Tooltip("Sprite for the exclamation point speech bubble.")]
    public Sprite exclamationSpeechBubble;
    [Tooltip("Sprite for the question mark speech bubble.")]
    public Sprite questionSpeechBubble;
    [Tooltip("Sprite for the arrow (blue) speech bubble.")]
    public Sprite blueSpeechBubble;
    [Tooltip("Sprite for the arrow (red) speech bubble.")]
    public Sprite redSpeechBubble;
    [Tooltip("Sprite for the arrow (yellow) speech bubble.")]
    public Sprite yellowSpeechBubble;
    [Tooltip("Sprite for the arrow (wasd) speech bubble.")]
    public Sprite wasdSpeechBubble;
    [Tooltip("Mirrored sprite for the arrow (wasd) speech bubble.")]
    public Sprite dsawSpeechBubble;
    [Tooltip("Time before prison break (NOTE: Sync this with the ceiling controller.")]
    public float timeTilPrisonBreak;

    private Rigidbody2D rb2d;
    private float jumpForce;
    private float moveForce;
    private float slamForce;
    private float maxSpeed;
    private float maxGlideSpeed;

    private bool prisonBreakIsHappening;
    private float paceSpeed;
    private float minWaitTime;
    private float maxWaitTime;
    private float panicSpeed;
    private float panicWalkTime;

    private bool hasMoved;
    private bool isFacingRight;
    private float rightwardForce;
    private float upwardForce;
    private SpriteRenderer speechBubble;
    private SpriteRenderer mainSpriteRenderer;

    // Use this for initialization
    void Start () {
        speechBubble = GetComponentsInChildren<SpriteRenderer>()[1]; // This will always be the speech bubble
        mainSpriteRenderer = GetComponent<SpriteRenderer>();
        speechBubble.enabled = false;
        rb2d = GetComponent<Rigidbody2D>();
        hasMoved = false;
        jumpForce = pc.jumpForce;
        moveForce = pc.moveForce;
        slamForce = pc.slamForce;
        maxSpeed = pc.maxSpeed;
        maxGlideSpeed = pc.maxGlideSpeed;
        rb2d.gravityScale = pc.normalGravity;
        isFacingRight = true;
        rightwardForce = 0.0f;
        upwardForce = 0.0f;

        prisonBreakIsHappening = false;
        paceSpeed = 1.0f;
        minWaitTime = 0.1f;
        maxWaitTime = 1.0f;
        panicSpeed = 4.0f;
        panicWalkTime = 1.0f;

        StartCoroutine(PrisonBreakTimerCoroutine());
        if (gameObject.name == "Prisoner1")
        {
            StartCoroutine(Prisoner1MovementCoroutine());
        }
        else if (gameObject.name == "Prisoner2")
        {
            StartCoroutine(Prisoner2And5MovementCoroutine());
        }
        else if (gameObject.name == "Prisoner3")
        {
            StartCoroutine(Prisoner3And4MovementCoroutine());
        }
        else if (gameObject.name == "Prisoner4")
        {
            StartCoroutine(Prisoner3And4MovementCoroutine());
        }
        else if (gameObject.name == "Prisoner5")
        {
            StartCoroutine(Prisoner2And5MovementCoroutine());
        }

    }

    public void Go()
    {
        // Navigate
        if (gameObject.name == "Yellow")
        {
            StartCoroutine(YellowMovementCoroutine());
        }
        else if (gameObject.name == "Red")
        {
            StartCoroutine(RedMovementCoroutine());
        }
        else if (gameObject.name == "Citizen1")
        {
            StartCoroutine(Citizen1MovementCoroutine());
        }
        else if (gameObject.name == "Citizen2")
        {
            StartCoroutine(Citizen2MovementCoroutine());
        }
        else if (gameObject.name == "Citizen3")
        {
            StartCoroutine(Citizen3MovementCoroutine());
        }
        else if (gameObject.name == "Citizen4")
        {
            StartCoroutine(Citizen4MovementCoroutine());
        }
        else if (gameObject.name == "Citizen5")
        {
            StartCoroutine(Citizen5MovementCoroutine());
        }
        else if (gameObject.name == "Citizen6")
        {
            StartCoroutine(Citizen6MovementCoroutine());
        }
        else if (gameObject.name == "Citizen7")
        {
            StartCoroutine(Citizen7MovementCoroutine());
        }
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(rb2d.velocity.x) > maxSpeed)
        {
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }
        // Move using the variables set in coroutines
        rb2d.AddForce(new Vector2(rightwardForce, upwardForce));
    }

    // Update is called once per frame
    void Update () {

	}

    public void Jump()
    {
        upwardForce = jumpForce;
        rightwardForce = 0.0f;
        rb2d.AddForce(new Vector2(rightwardForce, upwardForce));
        upwardForce = 0.0f;
    }

    public void MoveRight()
    {
        // Turn to face right
        if (!isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        upwardForce = 0.0f;
        rightwardForce = moveForce;
    }

    public void MoveLeft()
    {
        // Turn to face left
        if(isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        upwardForce = 0.0f;
        rightwardForce = -moveForce;
    }

    public void Stop()
    {
        upwardForce = 0.0f;
        rightwardForce = 0.0f;
        rb2d.velocity = Vector2.zero;
        maxSpeed = pc.maxSpeed;
        rb2d.gravityScale = pc.normalGravity;
        mainSpriteRenderer.sprite = mainBodySprite;
    }



    public void GlideRight()
    {
        // Turn to face right
        if (!isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        Stop();
        upwardForce = 0.0f;
        rightwardForce = moveForce;
        rb2d.gravityScale = pc.glideGravity;
        maxSpeed = maxGlideSpeed;
        mainSpriteRenderer.sprite = specialActionBodySprite;
    }

    public void GlideLeft()
    {
        // Turn to face left
        if (isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        Stop();
        upwardForce = 0.0f;
        rightwardForce = -moveForce;
        rb2d.gravityScale = pc.glideGravity;
        maxSpeed = maxGlideSpeed;
        mainSpriteRenderer.sprite = specialActionBodySprite;
    }

    public void Slam()
    {
        // Slam for one frame, then return to normal 0 forces
        upwardForce = -slamForce;
        rightwardForce = 0.0f;
        Stop();
        rb2d.AddForce(new Vector2(rightwardForce, upwardForce));
        upwardForce = 0.0f;
        mainSpriteRenderer.sprite = specialActionBodySprite;
    }

    public void HideSpeechBubble()
    {
        speechBubble.enabled = false;
    }

    public void DisplaySpeechBubble(Sprite bubbleToDisplay)
    {
        speechBubble.sprite = bubbleToDisplay;
        speechBubble.enabled = true; 

    }

    public void Grab()
    {
        // TODO: Implement Grab()
        mainSpriteRenderer.sprite = specialActionBodySprite;
    }

    IEnumerator PrisonBreakTimerCoroutine()
    {
        yield return new WaitForSeconds(timeTilPrisonBreak);
        prisonBreakIsHappening = true;
    }

    IEnumerator YellowMovementCoroutine()
    {
        if(!hasMoved)
        {
            hasMoved = true;
            // Unlock rigid body
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

            float runTime1 = 0.2f;
            float jumpTime = 0.1f;
            float runTime2 = 0.9f;

            // Run forward a bit
            MoveRight();
            yield return new WaitForSeconds(runTime1);

            // Jump for one frame, then wait for jumpTime
            Jump();
            yield return new WaitForSeconds(jumpTime);

            // Glide
            GlideRight();
            yield return new WaitForSeconds(runTime2);

            // Stop
            Stop(); 

            DisplaySpeechBubble(yellowSpeechBubble);

        }
    }


    IEnumerator RedMovementCoroutine()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            // Unlock rigid body
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

            float jumpTime = 0.8f;
            float slamTime = 1.0f;

            // Jump for one frame, then wait for jump time
            Jump();
            yield return new WaitForSeconds(jumpTime);

            // Slam for one frame, then wait for slamTime
            Slam();
            yield return new WaitForSeconds(slamTime);

            // Exit
            Stop();

            DisplaySpeechBubble(redSpeechBubble);
        }

    }


    IEnumerator BlueMovementCoroutine()
    {
        return null;

    }

    IEnumerator Citizen1MovementCoroutine()
    {
        if(!hasMoved)
        {
            hasMoved = true;
            float jumpTime1 = 0.1f;
            float waitTime1 = 0.4f;
            float moveTime1 = 0.5f;
            float waitTime2 = 0.5f;
            float jumpTime2 = 0.1f;
            float waitTime3 = 0.4f;
            float moveTime2 = 5.0f;

            Jump();
            yield return new WaitForSeconds(jumpTime1);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime1);

            MoveRight();
            yield return new WaitForSeconds(moveTime1);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime2);

            Jump();
            yield return new WaitForSeconds(jumpTime2);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime3);

            MoveLeft();
            yield return new WaitForSeconds(moveTime2);

            Stop();


        }
    }

    IEnumerator Citizen2MovementCoroutine()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            float jumpTime1 = 0.1f;
            float waitTime1 = 0.4f;
            float jumpTime2 = 0.1f;
            float waitTime2 = 0.4f;
            float moveTime1 = 2.0f;

            Jump();
            yield return new WaitForSeconds(jumpTime1);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime1);

            Jump();
            yield return new WaitForSeconds(jumpTime2);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime2);

            GlideLeft();
            yield return new WaitForSeconds(moveTime1);

           
            Stop();


        }
    }

    IEnumerator Citizen3MovementCoroutine()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            float moveTime0 = 0.5f;
            float jumpTime1 = 0.1f;
            float waitTime1 = 0.4f;
            float jumpTime2 = 0.1f;
            float waitTime2 = 0.4f;
            float jumpTime3 = 0.5f;
            float moveTime1 = 5.0f;

            MoveLeft();
            yield return new WaitForSeconds(moveTime0);

            Jump();
            yield return new WaitForSeconds(jumpTime1);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime1);

            Jump();
            yield return new WaitForSeconds(jumpTime2);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime2);

            Jump();
            yield return new WaitForSeconds(jumpTime3);

            GlideLeft();
            yield return new WaitForSeconds(moveTime1);


            Stop();


        }
    }

    IEnumerator Citizen4MovementCoroutine()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            float waitTime0 = 0.25f;
            float jumpTime1 = 0.1f;
            float waitTime1 = 0.4f;
            float jumpTime2 = 0.1f;
            float waitTime2 = 0.4f;
            float moveTime1 = 2.0f;

            // wait
            yield return new WaitForSeconds(waitTime0);

            Jump();
            yield return new WaitForSeconds(jumpTime1);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime1);

            Jump();
            yield return new WaitForSeconds(jumpTime2);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime2);

            MoveRight();
            yield return new WaitForSeconds(moveTime1);


            Stop();


        }
    }

    IEnumerator Citizen5MovementCoroutine()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            float jumpTime1 = 0.1f;
            float waitTime1 = 0.4f;
            float jumpTime2 = 0.1f;
            float waitTime2 = 0.4f;
            float moveTime1 = 2.0f;


            Jump();
            yield return new WaitForSeconds(jumpTime1);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime1);

            Jump();
            yield return new WaitForSeconds(jumpTime2);

            Stop();
            // wait
            yield return new WaitForSeconds(waitTime2);

            MoveRight();
            yield return new WaitForSeconds(moveTime1);


            Stop();


        }
    }



    IEnumerator Citizen6MovementCoroutine()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            float waitTime1 = 1.0f;
            float jumpTime1 = 0.1f;
            float waitTime2 = 0.4f;
            float jumpTime2 = 0.1f;
            float waitTime3 = 0.4f;
            float jumpTime3 = 0.1f;
            float moveTime2 = 5.0f;

            // Wait
            yield return new WaitForSeconds(waitTime1);

            Jump();
            yield return new WaitForSeconds(jumpTime1);

            Stop();
            // Wait
            yield return new WaitForSeconds(waitTime2);

            Jump();
            yield return new WaitForSeconds(jumpTime2);

            Stop();
            // Wait
            yield return new WaitForSeconds(waitTime3);

            Jump();
            yield return new WaitForSeconds(jumpTime3);

            Stop();
            MoveLeft();
            yield return new WaitForSeconds(moveTime2);

            Stop();


        }
    }

    IEnumerator Citizen7MovementCoroutine()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            float waitTime1 = 0.2f;
            float moveTime1 = 1.5f;
            float waitTime2 = 1.0f;
            float waitTime3 = 0.1f;
            float waitTime4 = 1.5f;
            float moveTime2 = 5.0f;

            // Wait
            yield return new WaitForSeconds(waitTime1);

            GlideLeft();
            yield return new WaitForSeconds(moveTime1);

            Stop();
            // Wait
            yield return new WaitForSeconds(waitTime2);

            MoveRight(); // Just to face right
            Stop();
            yield return new WaitForSeconds(waitTime3);

            DisplaySpeechBubble(heartSpeechBubble);
            // Wait
            yield return new WaitForSeconds(waitTime4);

            HideSpeechBubble();
            GlideLeft();
            yield return new WaitForSeconds(moveTime2);

            
            Stop();


        }
    }

    IEnumerator Prisoner1MovementCoroutine()
    {


        int randomizer = 0;
        float randomWaitTime = 0.1f;


        while(true)
        {
            maxSpeed = paceSpeed;
            // Wander left, right, or wait for random time
            randomizer = UnityEngine.Random.Range(0, 3);
            if(randomizer == 0)
            {
                MoveLeft();
            }
            else if(randomizer == 1)
            {
                MoveRight();
            }
            else if(randomizer == 2)
            {
                Stop();
            }
            if(isFacingRight)
            {
                HideSpeechBubble();
                DisplaySpeechBubble(wasdSpeechBubble);
            }
            else
            {
                HideSpeechBubble();
                DisplaySpeechBubble(dsawSpeechBubble);
            }
            randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(randomWaitTime);

            // Run left and right with no waiting between
            if(prisonBreakIsHappening)
            {
                HideSpeechBubble();
                DisplaySpeechBubble(dsawSpeechBubble);
                maxSpeed = panicSpeed;
            }
            while(prisonBreakIsHappening)
            {
                MoveLeft();

                yield return new WaitForSeconds(panicWalkTime);
            }
        }

    }

    IEnumerator Prisoner2And5MovementCoroutine()
    {

        int randomizer = 0;
        float randomWaitTime = 0.1f;

        while (true)
        {

            maxSpeed = paceSpeed;
            // Wander left, right, or wait for random time
            randomizer = UnityEngine.Random.Range(0, 3);
            if (randomizer == 0)
            {
                MoveLeft();
            }
            else if (randomizer == 1)
            {
                MoveRight();
            }
            else if (randomizer == 2)
            {
                Stop();
            }
            randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(randomWaitTime);

            // Run left and right with no waiting between
            maxSpeed = panicSpeed;
            while (prisonBreakIsHappening)
            {
                MoveRight();
                yield return new WaitForSeconds(panicWalkTime);
                MoveLeft();
                yield return new WaitForSeconds(panicWalkTime);
            }
        }
    }

    IEnumerator Prisoner3And4MovementCoroutine()
    {

        int randomizer = 0;
        float randomWaitTime = 0.1f;

        while (true)
        {

            maxSpeed = paceSpeed;
            // Wander left, right, or wait for random time
            randomizer = UnityEngine.Random.Range(0, 3);
            if (randomizer == 0)
            {
                MoveLeft();
            }
            else if (randomizer == 1)
            {
                MoveRight();
            }
            else if (randomizer == 2)
            {
                Stop();
            }
            randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(randomWaitTime);

            // Run left and right with no waiting between
            maxSpeed = panicSpeed;
            while (prisonBreakIsHappening)
            {
                MoveLeft();
                yield return new WaitForSeconds(panicWalkTime);
                MoveRight();
                yield return new WaitForSeconds(panicWalkTime);
            }
        }
    }

}

