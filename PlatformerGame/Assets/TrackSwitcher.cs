using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSwitcher : MonoBehaviour {

    [Tooltip("The audiosource controlling this song.")]
    public AudioSource audioSource;
    [Tooltip("The non-looping intro to the song.")]
    public AudioClip intro;
    [Tooltip("The looping body of the song.")]
    public AudioClip body;

    private bool hasPlayedBody;

	// Use this for initialization
	void Start () {
        audioSource.loop = false;
        audioSource.clip = intro;
        audioSource.Play();
        hasPlayedBody = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (!audioSource.isPlaying && !hasPlayedBody)
        {
            audioSource.clip = body;
            audioSource.loop = true;
            audioSource.Play();
            hasPlayedBody = true;
        }
	}
}
