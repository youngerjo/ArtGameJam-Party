using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallpaper : MonoBehaviour {

	public Texture texture;
	public float horizontalScale = 1.0f;
	public float verticalScale = 1.0f;

	void OnGUI() {

		float width = Screen.width * horizontalScale;
		float height = Screen.height * verticalScale;
		float x = (Screen.width * (1.0f - horizontalScale)) * 0.5f;
		float y = (Screen.height * (1.0f - verticalScale)) * 0.5f;

		GUI.DrawTexture(new Rect(x, y, width, height), texture, ScaleMode.StretchToFill);
	}
}
