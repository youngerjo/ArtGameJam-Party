using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntrance : MonoBehaviour {

	public PortalExit exit;

	void OnTriggerEnter(Collider other) {

		if (other.GetType() == typeof(CharacterController)) {
			
			Vector3 newPosition = other.transform.position;
			newPosition.x = exit.transform.position.x;
			other.transform.position = newPosition;
		}
	}
}
