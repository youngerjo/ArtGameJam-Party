using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

	public enum Direction {
		LeftToRight,
		RightToLeft
	}

	public Direction direction = Direction.LeftToRight;

	public Transform foreground;

	public void SetValue(float value) {

		if (direction == Direction.LeftToRight) {
			foreground.localPosition = new Vector3((value - 1.0f) * 0.5f, 0.0f, 0.0f);
		}
		else if (direction == Direction.RightToLeft) {
			foreground.localPosition = new Vector3((1.0f - value) * 0.5f, 0.0f, 0.0f);
		}

		foreground.localScale = new Vector3(value, 1.0f, 1.0f);
	}
}
