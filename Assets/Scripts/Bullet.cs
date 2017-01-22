using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public Transform renderingObject;

	private float _bulletLife = 0.0f;
	private float _bulletDamage = 0.0f;
	private float _bulletStunDamage = 0.0f;

	private float _growingDuration = 0.0f;
	private float _flyingRotation = 0.0f;
	private float _flyingIntensity = 0.0f;

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

	private StateMachine locomotion = new StateMachine();

	public Item item = null;

	void Awake() {

		_bulletLife = GameConfig.shared.bulletLife;
		_bulletDamage = GameConfig.shared.bulletDamage;
		_bulletStunDamage = GameConfig.shared.bulletStunDamage;

		{
			State state = new State("grow");
			state.OnBegin += Grow_OnBegin;
			state.OnUpdate += Grow_OnUpdate;

			locomotion.AddState(state);
		}

		{
			State state = new State("fly");
			state.OnBegin += Fly_OnBegin;
			state.OnUpdate += Fly_OnUpdate;

			locomotion.AddState(state);
		}

		{
			State state = new State("purge");
			state.OnBegin += Purge_OnBegin;

			locomotion.AddState(state);
		}
	}

	// Use this for initialization
	void Start () {
		locomotion.BeginState("grow");
	}

	void Update() {
		locomotion.Update(Time.deltaTime);
	}

	void Grow_OnBegin(State state) {
        Color color;
        _growingDuration = Random.Range(0.2f, 0.5f);
		_flyingRotation = Random.Range(-180.0f, 180.0f);
		_flyingIntensity = Random.Range(0.0f, 1.0f);
        if (item != null)
        {
            if (item.colorType == Item.ColorType.Blue)
            {
                color = new Color(.0f, .0f, 1.0f, _flyingIntensity);
                renderingObject.GetComponent<Renderer>().material.SetColor("_TintColor", color);
            }
            else if (item.colorType == Item.ColorType.Purle)
            {
                color = new Color(1.0f, .0f, 1.0f, _flyingIntensity);
                renderingObject.GetComponent<Renderer>().material.SetColor("_TintColor", color);
            }
            else if (item.colorType == Item.ColorType.Green)
            {
                color = new Color(.0f, 1.0f, .0f, _flyingIntensity);
                renderingObject.GetComponent<Renderer>().material.SetColor("_TintColor", color);
            }
        }

		
		
		renderingObject.transform.localScale = Vector3.one * 0.5f;
	}

	void Grow_OnUpdate(State state) {

		float progress = Mathf.Min(state.elapsedTime / _growingDuration, 1.0f);
		float angle = _flyingRotation * progress;
		float scale = _flyingIntensity * progress * 0.5f + 0.5f;

		renderingObject.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		renderingObject.transform.localScale = Vector3.one * scale;

		if (progress >= 1.0f) {
			locomotion.BeginState("fly");
		}
	}

	void Fly_OnBegin(State state) {

	}

	void Fly_OnUpdate(State state) {

		if (state.elapsedTime > _bulletLife) {
			locomotion.BeginState("purge");
		}
	}

	void Purge_OnBegin(State state) {
		Object.Destroy(gameObject);
	}

	public void Purge() {
		locomotion.BeginState("purge");
	}

	public bool isPurged {
		get {
			return locomotion.currentState.name == "purge";
		}
	}
}
