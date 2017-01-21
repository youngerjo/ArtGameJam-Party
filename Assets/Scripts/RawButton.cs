using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawButton : MonoBehaviour {

	public Renderer normalRenderer;
	public Renderer hoverRenderer;

	void Update() {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit info;

		if (GetComponent<Collider>().Raycast(ray, out info, 1000.0f)) {
			normalRenderer.enabled = false;
			hoverRenderer.enabled = true;

			if (Input.GetMouseButtonDown(0)) {
				LoadLevel();
			}
		}	
		else {
			normalRenderer.enabled = true;
			hoverRenderer.enabled = false;
		}
	}

	void LoadLevel() {
		Application.LoadLevel("Level");
	}
}
