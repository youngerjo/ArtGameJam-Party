using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSceneManager : MonoBehaviour {

	public ProgressBar player1HpBar;
	public ProgressBar player2HpBar;
	public Inventory player1Inventory;
	public Inventory player2Inventory;
	public Renderer vsRenderer;

	private StateMachine game = new StateMachine();
	private StateMachine gate = new StateMachine();

	private int itemCount = 0;

	void Awake() {
		
		{
			State state = new State("ready");
			state.OnBegin += Ready_OnBegin;
			state.OnUpdate += Ready_OnUpdate;
			game.AddState(state);
		}

		{
			State state = new State("play");
			state.OnBegin += Play_OnBegin;
			state.OnUpdate += Play_OnUpdate;
			game.AddState(state);
		}

		{
			State state = new State("over");
			state.OnBegin += Over_OnBegin;
			state.OnUpdate += Over_OnUpdate;
			game.AddState(state);
		}

		{
			State state = new State("closed");
			state.OnBegin += GateClosed_OnBegin;
			gate.AddState(state);
		}

		{
			State state = new State("player1");
			state.OnBegin += GatePlayer1_OnBegin;
			gate.AddState(state);
		}

		{
			State state = new State("player2");
			state.OnBegin += GatePlayer2_OnBegin;
			gate.AddState(state);			
		}

		{
			State state = new State("utopia");
			state.OnBegin += GateUtopia_OnBegin;
			gate.AddState(state);
		}
	}

	void Start () {
		
		NotificationCenter.shared.AddHandler("Player1Hit", OnPlayer1Hit);
		NotificationCenter.shared.AddHandler("Player2Hit", OnPlayer2Hit);
		NotificationCenter.shared.AddHandler("Player1GetItem", OnPlayer1GetItem);
		NotificationCenter.shared.AddHandler("Player2GetItem", OnPlayer2GetItem);

		game.BeginState("ready");
		gate.BeginState("closed");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Ready_OnBegin(State state) {
		Debug.Log("Game.Ready.Begin");
	}

	void Ready_OnUpdate(State state) {

	}

	void Play_OnBegin(State state) {
		Debug.Log("Game.Play.Begin");
	}

	void Play_OnUpdate(State state) {

	}

	void Over_OnBegin(State state) {
		Debug.Log("Game.Over.Begin");
	}

	void Over_OnUpdate(State state) {

	}

	void GateClosed_OnBegin(State state) {
		Debug.Log("Gate.Closed.Begin");
	}

	void GatePlayer1_OnBegin(State state) {
		Debug.Log("Gate.Player1.Begin");
	}

	void GatePlayer2_OnBegin(State state) {
		Debug.Log("Gate.Player2.Begin");
	}

	void GateUtopia_OnBegin(State state) {
		Debug.Log("Gate.Utopia.Begin");
	}

	void OnPlayer1Hit(Notification notification) {
		float hitPointRatio = (float)notification.userInfo;
		player1HpBar.SetValue(hitPointRatio);
	}

	void OnPlayer2Hit(Notification notification) {
		float hitPointRatio = (float)notification.userInfo;
		player2HpBar.SetValue(hitPointRatio);
	}

	void OnPlayer1GetItem(Notification notification) {
		Item item = notification.userInfo as Item;
		
		if (player1Inventory.AddItem(item)) {
			OnAnyPlayerGetItem();
		}
	}

	void OnPlayer2GetItem(Notification notification) {
		Item item = notification.userInfo as Item;

		if (player2Inventory.AddItem(item)) {
			OnAnyPlayerGetItem();
		}
	}

	void OnAnyPlayerGetItem() {
		itemCount++;

		float alpha = (float)(Mathf.Max(0, 5 - itemCount) / 5.0f);
		Color color = new Color(1.0f, 1.0f, 1.0f, alpha);
		
		vsRenderer.material.SetColor("_TintColor", color);
	}
}
