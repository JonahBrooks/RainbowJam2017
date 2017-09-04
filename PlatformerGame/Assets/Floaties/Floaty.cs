using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floaty : MonoBehaviour {

	const float minSpeed = 2.5f;
	const float maxSpeed = 10.0f;

	[Range(minSpeed, maxSpeed)]
	public float speed;

	float _period;
	float _amplitude;
	float _periodOffset;

	// Use this for initialization
	void Start () {
		var travelDistance = transform.parent.GetComponent<RectTransform>().rect.height;
		_amplitude =  travelDistance / 2.0f;
		_period = travelDistance / speed;
		//_period = Mathf.Lerp(minPeriod, maxPeriod, (1 - period));
		_periodOffset = Random.Range(0.0f, _period);
	}
	
	// Update is called once per frame
	void Update () {
		float theta = (Time.timeSinceLevelLoad + _periodOffset) / _period;
		float y = _amplitude * Mathf.Sin(theta);
		var newPosition = new Vector3(transform.localPosition.x, y, 0.0f);
		transform.localPosition = newPosition;
	}
}
