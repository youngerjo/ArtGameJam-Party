using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverSceneManager : MonoBehaviour {

	public Transform playerFall;
	public Transform background;
	public Renderer blackOut;

	private StateMachine gameOver = new StateMachine();
	private float elapsedTime = 0.0f;

	void Awake() {

		{
			State state = new State("fadeIn");
			state.OnUpdate += FadeIn_OnUpdate;

			gameOver.AddState(state);
		}

		{
			State state = new State("fall");
			state.OnUpdate += Fall_OnUpdate;

			gameOver.AddState(state);
		}

		{
			State state = new State("fadeOut");
			state.OnUpdate += FadeOut_OnUpdate;

			gameOver.AddState(state);
		}
	}

	void Start () {
		gameOver.BeginState("fadeIn");
	}
	
	// Update is called once per frame
	void Update () {

		gameOver.Update(Time.deltaTime);

		float y = background.localPosition.y + Time.deltaTime * 30.0f;

		if (y > 9.0f) {
			y = -9.0f;
		}

		background.localPosition = new Vector3(0.0f, y, background.localPosition.z);

		elapsedTime += Time.deltaTime;

		Vector3 position = playerFall.localPosition;
		position.x = Mathf.Sin(elapsedTime * 50.0f) * 0.5f + 1.0f;
		playerFall.localPosition = position;
	}

	void FadeIn_OnUpdate(State state) {

		float progress = Mathf.Min(state.elapsedTime / 1.0f, 1.0f);
		
		Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f - progress);
		blackOut.material.SetColor("_TintColor", color);

		if (progress >= 1.0f) {
			gameOver.BeginState("fall");
		}
	}

	void Fall_OnUpdate(State state) {

		if (state.elapsedTime > 1.5f) {
			gameOver.BeginState("fadeOut");
		}
	}

	void FadeOut_OnUpdate(State state) {

		float progress = Mathf.Min(state.elapsedTime / 1.0f, 1.0f);
		
		Color color = new Color(0.0f, 0.0f, 0.0f, progress);
		blackOut.material.SetColor("_TintColor", color);

		if (progress >= 1.0f) {
			LoadLevel();
		}
	}

	void LoadLevel() {
		Application.LoadLevel("Level");
	}
}
