using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour {

	public Material[] materials;

	private int currentIndex = 0;

	void Start() {

		if (materials.Length > 0) {
			GetComponent<Renderer>().material = materials[0];
		}
	}

	public void SwitchToIndex(int index) {

		if (index < 0 || index >= materials.Length) {
			return;
		}

		currentIndex = index;
		GetComponent<Renderer>().material = materials[index];
	}

	public void SwitchToNext() {
		currentIndex++;

		if (currentIndex >= materials.Length) {
			currentIndex = materials.Length;
		}

		SwitchToIndex(currentIndex);
	}

	public void SwitchToFirst() {
		SwitchToIndex(0);
	}
}
