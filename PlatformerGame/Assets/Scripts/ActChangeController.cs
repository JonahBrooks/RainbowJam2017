using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActChangeController : MonoBehaviour {

    [Tooltip("The music source currently playing music from last act")]
    public AudioSource audioSource;
    [Tooltip("The music clip that needs to be started (intro music from this act)")]
    public AudioClip introMusicToStart;
    [Tooltip("The body of the music clip to be looped (body music from this act)")]
    public AudioClip bodyMusicToStart;
    [Tooltip("The sprite renderer for the background (far) that needs to be changed.")]
    public SpriteRenderer farBackground;
    [Tooltip("The sprite renderer for the background (far, back) that will eventually store the new background.")]
    public SpriteRenderer farBackgroundBack;
    [Tooltip("The new sprite to use as the new background (far).")]
    public Sprite farBackgroundToUse;
    [Tooltip("The sprite renderer for the background (medium) that needs to be changed.")]
    public SpriteRenderer mediumBackground;
    [Tooltip("The sprite renderer for the background (medium, back) that will eventually store the new background.")]
    public SpriteRenderer mediumBackgroundBack;
    [Tooltip("The new sprite to use as the new background (medium).")]
    public Sprite mediumBackgroundToUse;
    [Tooltip("The sprite renderer for the background (near) that needs to be changed.")]
    public SpriteRenderer nearBackground;
    [Tooltip("The sprite renderer for the background (near, back) that will eventually store the new background.")]
    public SpriteRenderer nearBackgroundBack;
    [Tooltip("The new sprite to use as the new background (near).")]
    public Sprite nearBackgroundToUse;
    [Tooltip("The time in seconds of the entire blending process.")]
    public float shiftDuration;

    private float shiftDelay;

    private bool actChanged;
    private bool playingBody;
    private bool playedIntro;

	// Use this for initialization
	void Start () {
        actChanged = false;
        playingBody = false;
        playedIntro = false;
        shiftDelay = shiftDuration / 256.0f;
	}
	
	// Update is called once per frame
	void Update () {
        
		if(audioSource != null && !audioSource.isPlaying && playedIntro && !playingBody)
        {
            if(bodyMusicToStart != null)
            {
                playingBody = true;
                audioSource.loop = true;
                audioSource.clip = bodyMusicToStart;
                audioSource.Play();
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(!actChanged && collision.CompareTag("Player"))
        {
            // Only allow the act to change to this act once
            actChanged = true;
            if (audioSource != null)
            {
                audioSource.Stop();
                if (introMusicToStart != null)
                {
                    audioSource.clip = introMusicToStart;
                    audioSource.loop = false;
                    audioSource.Play();
                    playedIntro = true;
                }
            }
            if (farBackground != null && farBackgroundToUse != null)
            {
                // Copy last act's background into the front background
                farBackground.sprite = farBackgroundBack.sprite;
                // Assign the back background to be this act's background
                farBackgroundBack.sprite = farBackgroundToUse;
                // Gradually shift the alpha of last act's background until this one is completely visible
                StartCoroutine(FarBackgroundShiftCoroutine());
            }
            if (mediumBackground != null && mediumBackgroundToUse != null)
            {
                // Copy last act's background into the front background
                mediumBackground.sprite = mediumBackgroundBack.sprite;
                // Assign the back background to be this act's background
                mediumBackgroundBack.sprite = mediumBackgroundToUse;
                // Gradually shift the alpha of last act's background until this one is completely visible
                StartCoroutine(MediumBackgroundShiftCoroutine());
            }
            if (nearBackground != null && nearBackgroundToUse != null)
            {
                // Copy last act's background into the front background
                nearBackground.sprite = nearBackgroundBack.sprite;
                // Assign the back background to be this act's background
                nearBackgroundBack.sprite = nearBackgroundToUse;
                // Gradually shift the alpha of last act's background until this one is completely visible
                StartCoroutine(NearBackgroundShiftCoroutine());
            }
        }
    }

    IEnumerator FarBackgroundShiftCoroutine()
    {
        float alpha = 1;
        Color color = farBackground.color;

        while (alpha >= 0)
        {
            color.a = alpha;
            farBackground.color = color;
            alpha-=1.0f/256.0f;
            yield return new WaitForSeconds(shiftDelay);
        }
        color.a = 0;
        farBackground.color = color;
    }

    IEnumerator MediumBackgroundShiftCoroutine()
    {
        float alpha = 1;
        Color color = mediumBackground.color;

        while (alpha >= 0)
        {
            color.a = alpha;
            mediumBackground.color = color;
            alpha -= 1.0f / 256.0f;
            yield return new WaitForSeconds(shiftDelay);
        }
        color.a = 0;
        mediumBackground.color = color;
    }

    IEnumerator NearBackgroundShiftCoroutine()
    {
        float alpha = 1;
        Color color = nearBackground.color;

        while (alpha >= 0)
        {
            color.a = alpha;
            nearBackground.color = color;
            alpha -= 1.0f / 256.0f;
            yield return new WaitForSeconds(shiftDelay);
        }
        color.a = 0;
        nearBackground.color = color;
    }
}
