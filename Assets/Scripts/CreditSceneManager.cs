using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditSceneManager : MonoBehaviour {

	public Transform scroller;
	public Transform credit;
	public Transform logo;

	private float elapsedTime = 0.0f;

	private StateMachine fsm = new StateMachine();

	void Awake() {

		{
			State state = new State("fadeIn");
			state.OnUpdate += FadeIn_OnUpdate;

			fsm.AddState(state);
		}

		{
			State state = new State("scroll");
			state.OnUpdate += Scroll_OnUpdate;

			fsm.AddState(state);
		}

		{
			State state = new State("stop");
			state.OnUpdate += Stop_OnUpdate;

			fsm.AddState(state);
		}

		{
			State state = new State("fadeOut");
			state.OnUpdate += FadeOut_OnUpdate;

			fsm.AddState(state);
		}
	}

	void Start() {
		fsm.BeginState("fadeIn");
	}
	
	// Update is called once per frame
	void Update () {
		fsm.Update(Time.deltaTime);
	}

	void FadeIn_OnUpdate(State state) {
		float progress = Mathf.Min(state.elapsedTime / 2.0f, 1.0f);

		Material creditMaterial = credit.GetComponent<Renderer>().material;

		Color color = new Color(1.0f, 1.0f, 1.0f, progress);
		creditMaterial.SetColor("_TintColor", color);

		if (progress >= 1.0f) {
			fsm.BeginState("scroll");
		}
	}

	void Scroll_OnUpdate(State state) {
			Vector3 position = scroller.localPosition;
			position.y += Time.deltaTime * 2.0f;
			scroller.localPosition = position;			

			if (logo.position.y > -0.5f) {
				fsm.BeginState("stop");
			}
	}

	void Stop_OnUpdate(State state) {

		if (state.elapsedTime > 3.0f) {
			fsm.BeginState("fadeOut");
		}
	}

	void FadeOut_OnUpdate(State state) {
		float progress = Mathf.Min(state.elapsedTime / 2.0f, 1.0f);

		Material creditMaterial = credit.GetComponent<Renderer>().material;
		Material logoMaterial = logo.GetComponent<Renderer>().material;

		Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f - progress);
		creditMaterial.SetColor("_TintColor", color);
		logoMaterial.SetColor("_TintColor", color);

		if (progress >= 1.0f) {
			LoadTitle();
		}
	}

	void LoadTitle() {
		Application.LoadLevel("Title");
	}
}
