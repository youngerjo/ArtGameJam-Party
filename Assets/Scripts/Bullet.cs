using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	private float bulletLife = 0.0f;

	void Awake() {
		bulletLife = GameConfig.shared.bulletLife;
	}

	// Use this for initialization
	void Start () {
		Object.Destroy(gameObject, bulletLife);
	}	
}
