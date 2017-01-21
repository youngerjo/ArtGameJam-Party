using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	private float _bulletLife = 0.0f;
	private float _bulletDamage = 0.0f;
	private float _bulletStunDamage = 0.0f;

	public float bulletDamage {
		get {
			return _bulletDamage;
		}
	}

	public float bulletStunDamage {
		get {
			return _bulletStunDamage;
		}
	}

	private bool _purged = false;

	public bool isPurged {
		get {
			return _purged;
		}
	}

	public Item item = null;

	void Awake() {
		_bulletLife = GameConfig.shared.bulletLife;
		_bulletDamage = GameConfig.shared.bulletDamage;
		_bulletStunDamage = GameConfig.shared.bulletStunDamage;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(PurgeAfterDelay(_bulletLife));
	}	

	IEnumerator PurgeAfterDelay(float delay) {
		
		yield return new WaitForSeconds(delay);
		Purge();
	}

	public void Purge() {

		if (_purged) {
			return;
		}

		_purged = true;
		Object.Destroy(gameObject);
	}
}
