using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour {
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
    [Tooltip("The time in seconds between entering the portal and the title screen laoding.")]
    public float endGameDelay;
    [Tooltip("The audio source used for playing sound effects.")]
    public AudioSource soundEffects;
    [Tooltip("The various jump sound effects.")]
    public AudioClip[] jumpSoundEffects;
    [Tooltip("The various grab sound effects.")]
    public AudioClip[] grabSoundEffects;
    [Tooltip("The slam crash sound effect.")]
    public AudioClip crashSoundEffect;
    [Tooltip("The jump descending sound effect.")]
    public AudioClip descendSoundEffect;
    [Tooltip("The glide sound effect (loops)")]
    public AudioClip glideSoundEffect;
    [Tooltip("Canvas holding the pause menu.")]
    public Canvas pauseMenu;

    [Tooltip("Sprite for the arrow (blue) speech bubble.")]
    public Sprite blueSpeechBubble;

    public Color glideColor;
    public Color slamColor;
    public Color grabColor;
}
