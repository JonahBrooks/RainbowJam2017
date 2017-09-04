using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyCollection : MonoBehaviour {

	public bool drawOutline = true;
	Color selectedDrawColor = Color.yellow;
	Color unselectedDrawColor = Color.cyan;

	void OnDrawGizmos() {
		if (drawOutline) {
			var rectTransform = GetComponent<RectTransform>();
			Gizmos.color = unselectedDrawColor;
			Gizmos.DrawWireCube(rectTransform.position, new Vector3(
				rectTransform.rect.width, rectTransform.rect.height, 0.0f
			));
		}
	}

	void OnDrawGizmosSelected() {
		var rectTransform = GetComponent<RectTransform>();
		Gizmos.color = selectedDrawColor;
		Gizmos.DrawWireCube(rectTransform.position, new Vector3(
				rectTransform.rect.width, rectTransform.rect.height, 0.0f
			));
	}
}
