using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour {

	public GameObject activeObject;

	public Item.ColorType colorType = Item.ColorType.Unspecified;

	public bool isOn {
		get {
			return activeObject.active;
		}

		set {
			activeObject.active = value;
		}
	}
}
