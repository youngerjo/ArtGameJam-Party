using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour {

	public static GameConfig shared;

	void Awake() {
		GameConfig.shared = this;
	}

	public float gravity = 9.8f;
	public float hitPoint = 100.0f;
	public float stunPoint = 100.0f;
	public float stunRegen = 10.0f;
	public float moveSpeed = 10.0f;
	public float moveAccel = 10.0f;
	public float moveDecel = 20.0f;
	public float jumpPower = 10.0f;
	public float coolTime = 0.5f;
	public float bulletPower = 1000.0f;
	public float bulletLife = 1.0f;
	public float bulletDamage = 5.0f;
	public float bulletStunDamage = 20.0f;
	public float hitPower = 100.0f;
	public float itemChance = 0.1f;
    public float hitDelay = 0.25f;
}
