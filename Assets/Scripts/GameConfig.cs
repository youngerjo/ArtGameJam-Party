using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour {

	public static GameConfig shared;

	void Awake() {
		GameConfig.shared = this;
	}

	public float gravity = 9.8f;
	public float hitPoint = 100;
	public float moveSpeed = 10;
	public float moveAccel = 10;
	public float moveDecel = 20;
	public float jumpPower = 10;
}
