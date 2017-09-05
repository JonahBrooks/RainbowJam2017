using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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



public abstract class Character : MonoBehaviour {

	// Instead of getting input from the Input methods, which only work for the player character,
	// we get "input" in the form of these "commands." It is up to Character subclasses
	// to fill this struct every frame. (The player subclass will check Input; NPC subclass(es) will
	// get commands elsewhere.)
	public class CommandState {
        public bool rollOverOk;

		public float horizontal;
		public bool btnLeft;
		public bool btnRight;

        public bool btnDownJump;

		public bool btnDownSlam;
		public bool btnSlam;
        public bool btnUpSlam;	

		public bool btnDownDash;
		public bool btnDash;
		public bool btnUpDash;

		public bool btnDownGrab;
		public bool btnGrab;
		public bool btnUpGrab;

        public bool btnDownEsc;

        // Don't call this! It should be called once in Character.FixedUpdate.
        // Potential problem: because some variables were originally accessed in FixedUpdate
        // and some in Update, there is a chance either function would have seen the same
        // set of data twice, and that code has not been adjusted to take that into consideration.
        public void FrameRollOver() {
            if (!rollOverOk)
                return;

            if (btnDownJump) {
                btnDownJump = false;
            }

            if (btnDownSlam) {
                btnDownSlam = false;
                btnSlam = true;
                btnUpSlam = false;
            }

            if (btnUpSlam) {
                btnDownSlam = false;
                btnSlam = false;
                btnUpSlam = false;
            }

            if (btnDownDash) {
                btnDownDash = false;
                btnDash = true;
                btnUpDash = false;
            }

            if (btnUpDash) {
                btnDownDash = false;
                btnDash = false;
                btnUpDash = false;
            }

            if (btnDownGrab) {
                btnDownGrab = false;
                btnGrab = true;
                btnUpGrab = false;
            }

            if (btnUpGrab) {
                btnDownGrab = false;
                btnGrab = false;
                btnUpGrab = false;
            }
        }
	}

    protected Rigidbody2D rb2d;
    protected SpriteRenderer sprite;
    protected bool grabbedObjectIsOnRight;
    protected bool isFacingRight;
    protected bool isGrounded;
    protected bool isStopping;
    protected bool isStunned;
    protected bool hasPeaked;
    protected bool canMove;
    protected bool canTurn;
    protected bool isStartingSlam;
    protected bool isSlamming;
    protected bool canSlam;
    protected bool hasSlamAbilitiy;
    protected bool isStartingJump;
    protected bool isGrabbing;
    protected bool isGliding;
    protected bool isInUpdraft;
    protected bool canGlide;
    protected bool canJump;
    protected bool canDoubleJump;
    protected bool hasDoubleJumpAbility;
    protected bool hasGlideAbility;
    protected bool hasGrabAbility;
    protected bool hasSingleWallJumpAbility;
    protected bool isPaused;
    protected Vector2 groundCheckTopLeft;
    protected Vector2 groundCheckBottomRight;
    protected Transform grabbedObject;
    protected Transform lastGrabbedObjectJumpedFrom;
    protected Transform slammedObject;

	protected Globals globals;
	protected CommandState commands;
	protected BoxCollider2D groundCheck;
	protected Transform grabCheck;
    protected bool playSoundEffects;

    protected float maxSpeed;
    protected float moveForce;

    public void UseSlowSpeed() {
        maxSpeed = globals.maxSpeed * 0.6f;
        moveForce = globals.moveForce * 0.6f;
    }

    public void UseNormalSpeed() {
        maxSpeed = globals.maxSpeed;
        moveForce = globals.moveForce;
    }

	// Must be implemented in derived classes.
	protected abstract void UpdateCommands();

	// May be implemented in derived classes. If it is, the derived method should make
	// a call to base.Start().
	protected virtual void Start () {
		commands = new CommandState();
		globals = GameObject.Find("Globals").GetComponent<Globals>();
		groundCheck = transform.Find("GroundCheck").GetComponent<BoxCollider2D>();
		grabCheck = transform.Find("GrabCheck");

        maxSpeed = globals.maxSpeed;
        moveForce = globals.moveForce;
		
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
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
        canGlide = true;
        canJump = true;
        canDoubleJump = true;
        hasSlamAbilitiy = true; // TODO: Change this to false when we have NPCs implemented
        hasDoubleJumpAbility = false; // TODO: Change this to false when we have NPCs implemented
        hasGlideAbility = true; // TODO: Change this to false when we have NPCs implemented
        hasGrabAbility = true; // TODO: Change this to false when we have NPCs implemented
        hasSingleWallJumpAbility = false; // This probably remains false forever. Just leaving it in as a quick toggle.
        isPaused = false;
        rb2d.gravityScale = globals.normalGravity;

        globals.pauseMenu.enabled = false;
    }

	protected void Update() {
        commands.FrameRollOver();

		UpdateCommands();
		UpdateMovement();
        commands.rollOverOk = true;


    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Elevator"))
        {
            gameObject.transform.parent = collision.transform;
        }
        if (collision.CompareTag("Updraft") && collision.GetComponent<UpdraftController>().isActive)
        {
            isInUpdraft = true;
        }
        if (collision.CompareTag("EndGame"))
        {
            StartCoroutine(EndGameCoroutine());
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Elevator"))
        {
            gameObject.transform.parent = null;
        }
        if (collision.CompareTag("Updraft"))
        {
            isInUpdraft = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // In OnTriggerStay so that overlapping colliders don't render them unusable
        if (collision.CompareTag("Updraft") && collision.GetComponent<UpdraftController>().isActive)
        {
            isInUpdraft = true;
        }
    }

    protected void FixedUpdate()
    {
		float axis = commands.horizontal;		

        
        // If the player is not already going at max speed, and is trying to move
        if (!isGliding && canMove && Mathf.Abs(axis) > float.Epsilon && axis * rb2d.velocity.x < maxSpeed)
        {
            isStopping = false;
            // Apply a force in the direction of the movement
            rb2d.AddForce(axis * Vector2.right * moveForce);
        }
        else if (isGliding && canMove && Mathf.Abs(axis) > float.Epsilon && axis * rb2d.velocity.x < globals.maxGlideSpeed)
        {
            isStopping = false;
            // Apply a force in the direction of the movement
            rb2d.AddForce(axis * Vector2.right * globals.glideForce);
        }

        // If the player is going over max speed
        if (!isGliding && Mathf.Abs(rb2d.velocity.x) > maxSpeed)
        {
            // Clamp x velocity to max speed
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }
        else if (isGliding && Mathf.Abs(rb2d.velocity.x) > globals.maxGlideSpeed)
        {
            // Clamp x velocity to max glide speed
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * globals.maxGlideSpeed, rb2d.velocity.y);
        }

        // If the player is not trying to move, reduce moving speed
        if (!isStopping && !commands.btnRight && !commands.btnLeft && !commands.btnDash)
        {				   
            // Reduce momentum
            rb2d.velocity = new Vector2(rb2d.velocity.x * globals.stoppingFactor, rb2d.velocity.y);
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
            rb2d.AddForce(Vector2.up * globals.updraftForce);
        }

        // If all the conditions are met for the player to jump (see Update())
        if(isStartingJump)
        {
            // Apply upward force to make the player jump
            if(isGrounded)
            {
                rb2d.AddForce(globals.jumpForce * Vector2.up);
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
                    rb2d.AddForce(globals.jumpForce * Vector2.left);
                    rb2d.AddForce(globals.jumpForce * Vector2.up);
                }
                else
                {   // jump right
                    rb2d.AddForce(globals.jumpForce * Vector2.right);
                    rb2d.AddForce(globals.jumpForce * Vector2.up);
                }
            }
            else // Double jump uses doubleJumpForce instead of jumpForce
            {
                rb2d.AddForce(globals.doubleJumpForce * Vector2.up);
            }

            if (playSoundEffects) {
                // Load sound effect at random
                var sfxIndex = Random.Range(0, globals.jumpSoundEffects.Length);
                globals.soundEffects.clip = globals.jumpSoundEffects[sfxIndex];
                // Make sure looping is turned off
                globals.soundEffects.loop = false;
                // Play sound effect
                globals.soundEffects.Play();
            }

            // And disable the new jump flag to prevent a second application of the upward force
            isStartingJump = false;
        }

        // If all the conditions are met for the player to slam (see Update())
        if(isStartingSlam)
        {
            // Interrupt momentum before slam
            rb2d.velocity = Vector2.zero;
            // Apply downward force to make the player slam down
            rb2d.AddForce(globals.slamForce * Vector2.down);
            // And disable the new slam flag to prevent a second application of the downward force
            isStartingSlam = false;
            // Turn on isSlamming flag to later check for landing from a slam
            isSlamming = true;
            // Disable moving while slamming
            canMove = false;
         
            if (playSoundEffects) {
                // Load sound effect for descent
                globals.soundEffects.clip = globals.descendSoundEffect;
                // Make sure loop is turned off
                globals.soundEffects.loop = false;
                // Play sound effect
                globals.soundEffects.Play();
            }
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
                slammedObject.GetComponentInParent<SlamSwitchController>().Switch();

            }

            if (playSoundEffects) {
                // Load sound effect for landing from a slam
                globals.soundEffects.clip = globals.crashSoundEffect;
                // Make sure loop is turned off
                globals.soundEffects.loop = false;
                // Play sound effect
                globals.soundEffects.Play();
            }
        }

    }
    
    protected void UpdateMovement () {

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
        if (commands.btnDownJump && !isGliding && ((isGrounded || isGrabbing || (canDoubleJump && hasDoubleJumpAbility))) && canJump)
        {
            sprite.color = Color.white;
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
        if (!isGrounded && !hasPeaked && (Mathf.Abs(rb2d.velocity.y) < globals.hangtimeThreshold) && !(canDoubleJump && hasDoubleJumpAbility))
        {
            hasPeaked = true;
            StartCoroutine(HangtimeCoroutine());
        }

        

        // SLAMMING

        // If player is trying to slam and can currently slam
        if(commands.btnDownSlam && canSlam && hasSlamAbilitiy)
        {
            sprite.color = Color.red;
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
        else if(!commands.btnSlam && isGrounded)
        {
            // Change back to white
            sprite.color = Color.white;
        }

        // DASHING/GLIDING

        // If gliding is being initialized
        if(commands.btnDownDash && hasGlideAbility && canGlide && !isStartingJump)
        {
            // Drop all momentum to allow mid jump glides
            rb2d.velocity = Vector2.zero;
            canJump = false;
            isGliding = true;

            if (playSoundEffects) {
                // Load sound effect for gliding
                globals.soundEffects.clip = globals.glideSoundEffect;
                // Make sure looping is turned ON
                globals.soundEffects.loop = true;
                // Play sound effect
                globals.soundEffects.Play();
            }
        }
        // If player is holding the glide button while falling and has the glide ability
        if (commands.btnDash && isGliding && !isGrabbing && hasGlideAbility && canGlide && !isStartingJump)
        {
            sprite.color = Color.yellow;
            // Adjust gravity to create glide effect
            rb2d.gravityScale = globals.glideGravity;
            // Disable jump in the event that you were grounded when taking off
            canJump = false;
        }// If not gliding anymore
        else if(commands.btnUpDash)
        {
            sprite.color = Color.white;
            rb2d.gravityScale = globals.normalGravity;
            // Leave jump disabled until landing
            //canJump = true;
            isGliding = false;
            // Disable glide until landing to prevent glide spam
            canGlide = false;

            if (playSoundEffects) {
                // Stop sound effect from looping
                globals.soundEffects.loop = false;
                // Stop currently playing sound effect
                globals.soundEffects.Stop();
            }
        }

        // GRABBING

        // If player presses the grab button and has the grab ability
        if (commands.btnDownGrab && hasGrabAbility)
        {
            sprite.color = Color.blue;
            GrabObject();
            if (grabbedObject != null)
            {
                // Reset jumps so you can jump from a grabbed wall
                canJump = true;
                canDoubleJump = true;
            }
        }
        else if (commands.btnGrab && hasGrabAbility)
        {
			canGlide = true;
            sprite.color = Color.blue;
            // Grab any wall contacted
            GrabObject();
        }// If no longer holding grab
        else if (commands.btnUpGrab)
        {
            // Change color back to white
            sprite.color = Color.white;
            DropObject();
        }
    }

    protected void DropObject()
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

    protected void GrabObject()
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

            if (playSoundEffects)
            {
                // Load sound effect at random
                var sfxIndex = Random.Range(0, globals.grabSoundEffects.Length);
                globals.soundEffects.clip = globals.grabSoundEffects[sfxIndex];
                // Make sure looping is turned off
                globals.soundEffects.loop = false;
                // Play sound effect
                globals.soundEffects.Play();
            }
        }
    }


    public void PauseOrUnpauseGame()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0;
            globals.pauseMenu.enabled = true;
        }
        else
        {
            Time.timeScale = 1;
            globals.pauseMenu.enabled = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Start slam after delay of slamDelay seconds
    IEnumerator SlamCoroutine()
    {
        yield return new WaitForSeconds(globals.slamDelay);

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

        yield return new WaitForSeconds(globals.stunDelay);
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
        yield return new WaitForSeconds(globals.hangtimeDelay);
        rb2d.gravityScale = globals.normalGravity;
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(globals.endGameDelay);
        SceneManager.LoadScene("Title");

    }
}
