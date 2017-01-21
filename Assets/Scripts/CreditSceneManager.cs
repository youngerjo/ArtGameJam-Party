using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditSceneManager : MonoBehaviour {

	public Transform scroller;

	private float elapsedTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;

		Vector3 position = scroller.localPosition;
		position.y += Time.deltaTime * 2.0f;
		scroller.localPosition = position;

		if (elapsedTime > 30.0f) {
			LoadTitle();
		}
	}

	void LoadTitle() {
		Application.LoadLevel("Title");
	}
}
