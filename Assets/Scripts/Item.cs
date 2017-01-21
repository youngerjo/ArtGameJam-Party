using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

	public enum ColorType {
		Unspecified,
		Green,
		Blue,
		Purle
	}

	public ColorType colorType = ColorType.Unspecified;

	public Item() {

		int value = Random.Range(0, 3);

		if (value == 0) {
			colorType = ColorType.Green;
		}
		else if (value == 1) {
			colorType = ColorType.Blue;
		}
		else if (value == 2) {
			colorType = ColorType.Purle;
		}
	}
}
