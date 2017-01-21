using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtopiaSceneManager : MonoBehaviour {
	
	public Renderer whiteOut;
	public Renderer background;

	private StateMachine utopia = new StateMachine();
	public float enterDuration = 5.0f;
	public float stayDuration = 13.0f;
	public float farewellDuration = 1.0f;

	void Awake() {

		{
			State state = new State("enter");
			state.OnBegin += Enter_OnBegin;
			state.OnUpdate += Enter_OnUpdate;

			utopia.AddState(state);
		}

		{
			State state = new State("stay");
			state.OnBegin += Stay_OnBegin;
			state.OnUpdate += Stay_OnUpdate;

			utopia.AddState(state);
		}

		{
			State state = new State("farewell");
			state.OnBegin += Farewell_OnBegin;
			state.OnUpdate += Farewell_OnUpdate;

			utopia.AddState(state);
		}
	}

	void Start () {
		utopia.BeginState("enter");
	}
	
	// Update is called once per frame
	void Update () {
		utopia.Update(Time.deltaTime);
	}

	void Enter_OnBegin(State state) {

	}

	void Enter_OnUpdate(State state) {
		float progress = Mathf.Min(state.elapsedTime / enterDuration, 1.0f);

		Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f - progress);
		whiteOut.material.SetColor("_TintColor", color);

		if (progress >= 1.0f) {
			utopia.BeginState("stay");
		}
	}

	void Stay_OnBegin(State state) {

	}

	void Stay_OnUpdate(State state) {
		float progress = Mathf.Min(state.elapsedTime / stayDuration, 1.0f);

		if (progress >= 1.0f) {
			utopia.BeginState("farewell");
		}
	}

	void Farewell_OnBegin(State state) {

	}

	void Farewell_OnUpdate(State state) {
		float progress = Mathf.Min(state.elapsedTime / farewellDuration, 1.0f);

		Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f - progress);
		background.material.SetColor("_TintColor", color);

		if (progress >= 1.0f) {
			LoadCredit();
		}
	}

	void LoadCredit() {
		Application.LoadLevel("Credit");
	}
}
