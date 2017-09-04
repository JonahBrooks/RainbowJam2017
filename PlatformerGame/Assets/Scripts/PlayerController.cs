using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 
 *    
 * Notes from the discord chat:
 * 
 * The jumps are not consistant. So I'm never sure where I will land. Sometimes the jumps are huge and sometimes I can't get double jump going? It might be that (done)
 * the movement controls could afford to strike a finer balance between the momentum-based slipping of the first version and the snappy stop-on-a-dime precision of this new one. ( done )
 * Currently there's little appreciable difference between the double jump and glide in terms of distance I am able to cover so maybe make the glide have slightly less initial dropoff to it. ( done? ) 
 * movement controls need more friction and higher accelleration to feel less slippery and more reactive. ( done ) 
 * I like the impact that the double jump has, but lets maybe boost the regular jump some more so that it doesn't feel so inadequate for clearing gaps. ( done ) 
 * Camera view is nice, though lets zoom it out a LITTLE bit more ( done )
 * maybe place some invisible walls around the border so we don't keep falling off everytime we overshoot a jump. ( done )
 *  
 *  The glide could start off in a straight line like Peach's skirt float but here, give it gradual drop off as the effects sloooowly start to wear down ( done )
 *      applies constant forward force as you glide ( done )
 *  Controls could do with some rebinding, ( done )
 *      and after jumping, have them hang in the air just for a moment as they build up acceleration upon descent ( done? )
 *  give the character some sense of weight (so it takes a little while for them to build up top running speed) ( done ? )
 *  jumping physics need tighter tuning ( done )
 *  I definitely feel that the camera has to be pulled much further back. Like, N+ levels of further back ( done )
 *  lock the camera so that it ONLY scrolls up and down with the character as they move ( done )
 *  the ground slam feels good as is but to give it more "oomph", ( done )  
 *      have the move interrupt momentum mid-jump and  ( done )
 *      momentarily freeze the player character as they recover from the smash. ( done ) 
 *  the double jump could stand to feel a bit more accentuated; that this is a knowingly unnatural kind of jump. Like it's slightly faster and gains more ground than the regular jump alone ( done )
 */



public class PlayerController : MonoBehaviour {

    [Tooltip("The maximum speed the player can move while walking.")]
    public float maxSpeed;
    [Tooltip("The maximum speed the player can move while gliding.")]
    public float maxGlideSpeed;
    [Tooltip("The sideways force applied to the character when a or d is pressed.")]
    public float moveForce;
    [Tooltip("THe sideways force applied to the character when gliding and a or d is pressed.")]
    public float glideForce;
    [Tooltip("The upward force applied on the first jump when space is pressed.")]
    public float jumpForce;
    [Tooltip("The upward force applied on the second jump when space is pressed.")]
    public float doubleJumpForce;
    [Tooltip("The downward force applied when down is pressed to generate a slam.")]
    public float slamForce;
    [Tooltip("The number of seconds the player should rise in the air before slaming down with slam.")]
    public float slamDelay;
    [Tooltip("The number of seconds the player should be stunned after slamming the ground.")]
    public float stunDelay;
    [Tooltip("The number of seconds the player should hang in the air at the top of a double jump.")]
    public float hangtimeDelay;
    [Tooltip("The maximum verticle speed the player can have to be considered at the peak of their double jump.")]
    public float hangtimeThreshold;
    [Tooltip("The gravity scale used while gliding/dashing.")]
    public float glideGravity;
    [Tooltip("The gravity scale used while not gliding/dashing.")]
    public float normalGravity;
    [Tooltip("The amount of upward force applied while in an updraft.")]
    public float updraftForce;
    [Tooltip("The factor by which horizontal velocity is adjusted when stopping. 1 is friction only, 0 is full immediate stop")]
    public float stoppingFactor;
    [Tooltip("The distance to move a grabbable object if it is too close to push.")]
    public float grabbableObjectGap;
    [Tooltip("Used in determining if the player is on the ground.")]
    public BoxCollider2D groundCheck;
    [Tooltip("Used in determining if the player can grab an object in front of them.")]
    public Transform grabCheck;
    [Tooltip("The audio source used for playing sound effects.")]
    public AudioSource soundEffects;
    [Tooltip("The various jump sound effects.")]
    public AudioClip[] jumpSoundEffects;
    [Tooltip("The various grab sound effects.")]
    public AudioClip[] grabSoundEffects;
    [Tooltip("The slam crash sound effect.")]
    public AudioClip crashSoundEffect;
    [Tooltip("The slam descending sound effect.")]
    public AudioClip descendSoundEffect;
    [Tooltip("The glide sound effect (loops)")]
    public AudioClip glideSoundEffect;
   

    private Rigidbody2D rb2d;
    //private SpriteRenderer sprite;
    private bool grabbedObjectIsOnRight;
    private bool isFacingRight;
    private bool isGrounded;
    private bool isStopping;
    private bool isStunned;
    private bool hasPeaked;
    private bool canMove;
    private bool canTurn;
    private bool isStartingSlam;
    private bool isSlamming;
    private bool canSlam;
    private bool hasSlamAbilitiy;
    private bool isStartingJump;
    private bool isGrabbing;
    private bool isGliding;
    private bool isInUpdraft;
    private bool canGlide;
    private bool canJump;
    private bool canDoubleJump;
    private bool hasDoubleJumpAbility;
    private bool hasGlideAbility;
    private bool hasGrabAbility;
    private bool hasSingleWallJumpAbility;
    private Vector2 groundCheckTopLeft;
    private Vector2 groundCheckBottomRight;
    private Transform grabbedObject;
    private Transform lastGrabbedObjectJumpedFrom;
    private Transform slammedObject;

    private Globals globals;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        //sprite = GetComponent<SpriteRenderer>();
        isFacingRight = true;
        isGrounded = false;
        isStopping = false;
        isStunned = false;
        hasPeaked = false;
        canMove = true;
        canTurn = true;
        isStartingSlam = false;
        isSlamming = false;
        canSlam = true;
        isStartingJump = false;
        isGrabbing = false;
        isGliding = false;
        isInUpdraft = false;
        canGlide = true;
        canJump = true;
        canDoubleJump = true;
        hasSlamAbilitiy = true; // TODO: Change this to false when we have NPCs implemented
        hasDoubleJumpAbility = false; // TODO: Change this to false when we have NPCs implemented
        hasGlideAbility = true; // TODO: Change this to false when we have NPCs implemented
        hasGrabAbility = true; // TODO: Change this to false when we have NPCs implemented
        hasSingleWallJumpAbility = false; // This probably remains false forever. Just leaving it in as a quick toggle.
        rb2d.gravityScale = normalGravity;

        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Elevator"))
        {
            gameObject.transform.parent = collision.transform;
        }
        if( collision.CompareTag("Updraft") && collision.GetComponent<UpdraftController>().isActive)
        {
            isInUpdraft = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Elevator"))
        {
            gameObject.transform.parent = null;
        }
        if(collision.CompareTag("Updraft"))
        {
            isInUpdraft = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void FixedUpdate()
    {
        float axis = Input.GetAxis("Horizontal");

        
        // If the player is not already going at max speed, and is trying to move
        if (!isGliding && canMove && Mathf.Abs(axis) > float.Epsilon && axis * rb2d.velocity.x < maxSpeed)
        {
            isStopping = false;
            // Apply a force in the direction of the movement
            rb2d.AddForce(axis * Vector2.right * moveForce);
        }
        else if (isGliding && canMove && Mathf.Abs(axis) > float.Epsilon && axis * rb2d.velocity.x < maxGlideSpeed)
        {
            isStopping = false;
            // Apply a force in the direction of the movement
            rb2d.AddForce(axis * Vector2.right * glideForce);
        }


        // If the player is going over max speed
        if (!isGliding && Mathf.Abs(rb2d.velocity.x) > maxSpeed)
        {
            // Clamp x velocity to max speed
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }
        else if (isGliding && Mathf.Abs(rb2d.velocity.x) > maxGlideSpeed)
        {
            // Clamp x velocity to max glide speed
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxGlideSpeed, rb2d.velocity.y);
        }

        // If the player is not trying to move, reduce moving speed
        if (!isStopping && !Input.GetButton("Right") && !Input.GetButton("Left") && !Input.GetButton("Dash"))
        {
            // Reduce momentum
            rb2d.velocity = new Vector2(rb2d.velocity.x * stoppingFactor, rb2d.velocity.y);
            isStopping = true;
        }

        // Flip the sprite to face direction of travel
        if (axis < 0 && isFacingRight && canTurn)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else if (axis > 0 && !isFacingRight && canTurn)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        // Handle updraft physics
        if(isInUpdraft)
        {
            rb2d.AddForce(Vector2.up * updraftForce);
        }

        // If all the conditions are met for the player to jump (see Update())
        if(isStartingJump)
        {
            // Apply upward force to make the player jump
            if(isGrounded)
            {
                rb2d.AddForce(jumpForce * Vector2.up);
            }
            else if(isGrabbing)
            {
                lastGrabbedObjectJumpedFrom = grabbedObject;
                // Drop current item
                DropObject();

                // Do a wall jump
                // Face away fom wall
                if(grabbedObjectIsOnRight == isFacingRight)
                {
                    isFacingRight = !isFacingRight;
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                }
                // Apply diagonal force to wall jump
                if(grabbedObjectIsOnRight)
                {   // Jump left
                    rb2d.AddForce(jumpForce * Vector2.left);
                    rb2d.AddForce(jumpForce * Vector2.up);
                }
                else
                {   // jump right
                    rb2d.AddForce(jumpForce * Vector2.right);
                    rb2d.AddForce(jumpForce * Vector2.up);
                }
            }
            else // Double jump uses doubleJumpForce instead of jumpForce
            {
                rb2d.AddForce(doubleJumpForce * Vector2.up);
            }

            // Load sound effect at random
            soundEffects.clip = jumpSoundEffects[Random.Range(0, jumpSoundEffects.Length)];
            // Make sure looping is turned off
            soundEffects.loop = false;
            // Play sound effect
            soundEffects.Play();

            // And disable the new jump flag to prevent a second application of the upward force
            isStartingJump = false;
        }

        // If all the conditions are met for the player to slam (see Update())
        if(isStartingSlam)
        {
            // Interrupt momentum before slam
            rb2d.velocity = Vector2.zero;
            // Apply downward force to make the player slam down
            rb2d.AddForce(slamForce * Vector2.down);
            // And disable the new slam flag to prevent a second application of the downward force
            isStartingSlam = false;
            // Turn on isSlamming flag to later check for landing from a slam
            isSlamming = true;
            // Disable moving while slamming
            canMove = false;
         
            // Load sound effect for descent
            soundEffects.clip = descendSoundEffect;
            // Make sure loop is turned off
            soundEffects.loop = false;
            // Play sound effect
            soundEffects.Play();
        }

        // If landing from a slam
        if(isSlamming && isGrounded)
        {
            // Re-enable movement // Even though it is about to be disabled in StunCoroutine
            canMove = true;
            // No longer slamming
            isSlamming = false;
            // Stun the character for stun delay
            StartCoroutine(StunCoroutine());
            // OverlapArea to find slammable object below player, call Switch on that object
            Collider2D[] hits = new Collider2D[1];
            ContactFilter2D cf2d = new ContactFilter2D();
            cf2d.layerMask = 1 << LayerMask.NameToLayer("Slammable");
            cf2d.useLayerMask = true;
            Physics2D.OverlapArea(groundCheckTopLeft, groundCheckBottomRight, cf2d, hits);
            if (hits[0] != null)
            {
                slammedObject = hits[0].transform;
            }
            // Execute slammed object's Switch method
            if(slammedObject != null)
            {
                slammedObject.GetComponent<SlamSwitchController>().Switch();

            }
            // Load sound effect for landing from a slam
            soundEffects.clip = crashSoundEffect;
            // Make sure loop is turned off
            soundEffects.loop = false;
            // Play sound effect
            soundEffects.Play();
        }

        
    }

    // Update is called once per frame
    void Update () {

        // Used in checking for isGrounded and finding smashed objects
        Vector2 worldOffset = groundCheck.transform.TransformPoint(groundCheck.offset);
        groundCheckTopLeft = new Vector2(worldOffset.x - groundCheck.size.x  / 2.0f, worldOffset.y + groundCheck.size.y / 2.0f);
        groundCheckBottomRight = new Vector2(worldOffset.x + groundCheck.size.x  / 2.0f, worldOffset.y - groundCheck.size.y / 2.0f);
        
        // Check to see if the groundcheck box collider overlaps the ground
        isGrounded = Physics2D.OverlapArea(groundCheckTopLeft, groundCheckBottomRight, 1 << LayerMask.NameToLayer("Ground"));
        // Or a grabbable object
        isGrounded = isGrounded || Physics2D.OverlapArea(groundCheckTopLeft, groundCheckBottomRight, 1 << LayerMask.NameToLayer("Grabbable"));
        // Or a slammable object
        isGrounded = isGrounded || Physics2D.OverlapArea(groundCheckTopLeft, groundCheckBottomRight, 1 << LayerMask.NameToLayer("Slammable"));

        // JUMPING
        
        // If both of those conditions are true, reset the player's double jump status
        if (isGrounded && !isStunned)
        {
            canDoubleJump = true;
            canSlam = true;
            canJump = true;
            canGlide = true;
            hasPeaked = false;
            //Let you grab the same object again
            lastGrabbedObjectJumpedFrom = null;

        }

        if (!hasDoubleJumpAbility)
        {
            canDoubleJump = false;
        }

        // If the player can either jump or double jump and the jump button is pressed,
        if (Input.GetButtonDown("Jump") && !isGliding && ((isGrounded || isGrabbing || (canDoubleJump && hasDoubleJumpAbility))) && canJump)
        {
            //sprite.color = Color.white;
            ChangeColor(Color.white);
            // Flag jumping as true for use in fixedupdate
            isStartingJump = true;
            // If jumping in the air while not wall grabbing, disable double jump to prevent triple jumps
            if(!isGrounded && !isGrabbing)
            {
                // Drop all vertical momentum before doing double jump
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                canDoubleJump = false;
            }
        }

        

        // Lower gravity for a delay when velocity hits zero at peak of jump after double jump
        if (!isGrounded && !hasPeaked && (Mathf.Abs(rb2d.velocity.y) < hangtimeThreshold) && !(canDoubleJump && hasDoubleJumpAbility))
        {
            hasPeaked = true;
            StartCoroutine(HangtimeCoroutine());
        }

        

        // SLAMMING

        // If player is trying to slam and can currently slam
        if(Input.GetButtonDown("Slam") && canSlam && hasSlamAbilitiy)
        {
            //sprite.color = Color.red;
            ChangeColor(globals.slamColor);
            // If player is on the ground
            if(isGrounded)
            {
                // Jump then slam after slam delay
                isStartingJump = true;
                StartCoroutine(SlamCoroutine());
            }
            else
            {
                // Otherwise start slam right now from in the air
                isStartingSlam = true;
                canSlam = false;
            }
        } // If slamming has been completed
        else if(!Input.GetButton("Slam") && isGrounded)
        {
            // Change back to white
            //sprite.color = Color.white;
            ChangeColor(Color.white);
        }

        // DASHING/GLIDING

        // If gliding is being initialized
        if(Input.GetButtonDown("Dash") && hasGlideAbility && canGlide && !isStartingJump)
        {
            // Drop all momentum to allow mid jump glides
            rb2d.velocity = Vector2.zero;
            canJump = false;
            isGliding = true;

            // Load sound effect for gliding
            soundEffects.clip = glideSoundEffect;
            // Make sure looping is turned ON
            soundEffects.loop = true;
            // Play sound effect
            soundEffects.Play();
        }
        // If player is holding the glide button while falling and has the glide ability
        if (Input.GetButton("Dash") && isGliding && !isGrabbing && hasGlideAbility && canGlide && !isStartingJump)
        {
            //sprite.color = Color.yellow;
            ChangeColor(globals.glideColor);
            // Adjust gravity to create glide effect
            rb2d.gravityScale = glideGravity;
            // Disable jump in the event that you were grounded when taking off
            canJump = false;
        }// If not gliding anymore
        else if(Input.GetButtonUp("Dash"))
        {
            //sprite.color = Color.white;
            ChangeColor(Color.white);
            rb2d.gravityScale = normalGravity;
            // Leave jump disabled until landing
            //canJump = true;
            isGliding = false;
            // Disable glide until landing to prevent glide spam
            canGlide = false;

            // Stop sound effect from looping
            soundEffects.loop = false;
            // Stop currently playing sound effect
            soundEffects.Stop();
        }

        // GRABBING

        // If player presses the grab button and has the grab ability
        if (Input.GetButtonDown("Grab") && hasGrabAbility)
        {
            //sprite.color = Color.blue;
            ChangeColor(globals.grabColor);
            GrabObject();
            if (grabbedObject != null)
            {
                // Reset jumps so you can jump from a grabbed wall
                canJump = true;
                canDoubleJump = true;
            }
        }
        else if (Input.GetButton("Grab") && hasGrabAbility)
        {
            //sprite.color = Color.blue;
            ChangeColor(globals.grabColor);
            canGlide = true;
            // Grab any wall contacted
            GrabObject();
        }// If no longer holding grab
        else if (Input.GetButtonUp("Grab"))
        {
            // Change color back to white
            //sprite.color = Color.white;
            ChangeColor(Color.white);
            DropObject();
        }
    }

    private void DropObject()
    {
        if (grabbedObject != null)
        {
            isGrabbing = false;
            grabbedObject.GetComponentInParent<FixedJoint2D>().connectedBody = null;
            // Trigger the Release event of the grabbed object
            grabbedObject.GetComponentInParent<GrabbableController>().Release();
        }
        grabbedObject = null;
    }

    private void GrabObject()
    {
        // Only execute this function if no object is currently being held
        if(grabbedObject != null)
        {
            return;
        }

        // Use a transfrom and linecast to check for overlapping blue objects
        RaycastHit2D[] hits = new RaycastHit2D[1];
        ContactFilter2D cf2d = new ContactFilter2D();
        cf2d.layerMask = 1 << LayerMask.NameToLayer("Grabbable");
        cf2d.useLayerMask = true;
        Physics2D.Linecast(transform.position, grabCheck.position, cf2d, hits);
        grabbedObject = hits[0].transform;

        // Can't grab the same wall after jumping from it
        if (grabbedObject == lastGrabbedObjectJumpedFrom && !hasSingleWallJumpAbility)
        {
            grabbedObject = null; 
        }

        if (grabbedObject != null)
        {
            canJump = true; // Can always wall jump
            isGrabbing = true;
            // Trigger the Grab event of the grabbed object
            grabbedObject.GetComponentInParent<GrabbableController>().Grab();
            // Connect to grabbedObject's fixedJoint2D
            grabbedObject.GetComponentInParent<FixedJoint2D>().connectedBody = rb2d;
            // Store the direction from the player the grabbed object is on
            grabbedObjectIsOnRight = isFacingRight;
            // Load sound effect at random
            soundEffects.clip = grabSoundEffects[Random.Range(0, grabSoundEffects.Length)];
            // Make sure looping is turned off
            soundEffects.loop = false;
            // Play sound effect
            soundEffects.Play();
        }
    }

    void ChangeColor(Color newColor) {
        var graphics = transform.Find("Graphics");
        graphics.Find("BackSprite").GetComponent<SpriteRenderer>().color = newColor;
        graphics.Find("CapeSprite").GetComponent<SpriteRenderer>().color = newColor;
        var particlesModule = graphics.Find("Particles").GetComponent<ParticleSystem>().main;
        particlesModule.startColor = newColor;
    }


    // Start slam after delay of slamDelay seconds
    IEnumerator SlamCoroutine()
    {
        yield return new WaitForSeconds(slamDelay);

        isStartingSlam = true;
        canSlam = false;
    }


    // Stops the player from moving or slamming for stunDelay seconds
    IEnumerator StunCoroutine()
    {
        canMove = false;
        canSlam = false;
        canGlide = false;
        canJump = false;
        canTurn = false;
        isStunned = true;

        yield return new WaitForSeconds(stunDelay);
        canMove = true;
        canSlam = true;
        canGlide = true;
        canJump = true;
        canTurn = true;
        isStunned = false;
    }

    // Disables gravity for hangetimeDelay seconds
    IEnumerator HangtimeCoroutine()
    {
        rb2d.gravityScale = 0;
        yield return new WaitForSeconds(hangtimeDelay);
        rb2d.gravityScale = normalGravity;
    }

 }
