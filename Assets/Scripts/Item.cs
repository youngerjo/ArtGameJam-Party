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
}
