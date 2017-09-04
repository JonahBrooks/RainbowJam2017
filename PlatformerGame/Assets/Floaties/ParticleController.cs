using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

	[Range(0.0f, 15.0f)]
	public float emmisionRate;

	ParticleSystem _particles;
	SpriteRenderer _colorSource;

	// Use this for initialization
	void Start () {
		_particles = GetComponent<ParticleSystem>();
		_colorSource = GameObject.Find("Player").GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		var mainModule = _particles.main;

		// Set emission rate.
		var emission = _particles.emission;
		emission.rateOverDistance = emmisionRate;


		// Set color.
		float[] colorWeights = {0.4f, 0.75f, 1.0f, 1.0f};
		var chosenWeight = colorWeights[Random.Range(0, 3)];

		var newColor = new Color(
			_colorSource.color.r * chosenWeight,
			_colorSource.color.g * chosenWeight,
			_colorSource.color.b * chosenWeight,
			1.0f);

		mainModule.startColor = newColor;		
	}
}
