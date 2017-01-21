using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSceneManager : MonoBehaviour {

	public ProgressBar player1HpBar;
	public ProgressBar player2HpBar;
	public Inventory player1Inventory;
	public Inventory player2Inventory;
	

	// Use this for initialization
	void Start () {
		
		NotificationCenter.shared.AddHandler("Player1Hit", OnPlayer1Hit);
		NotificationCenter.shared.AddHandler("Player2Hit", OnPlayer2Hit);
		NotificationCenter.shared.AddHandler("Player1GetItem", OnPlayer1GetItem);
		NotificationCenter.shared.AddHandler("Player2GetItem", OnPlayer2GetItem);
	}
	
	// Update is called once per frame
	void Update () {
		
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
		player1Inventory.AddItem(item);
	}

	void OnPlayer2GetItem(Notification notification) {
		Item item = notification.userInfo as Item;
		player2Inventory.AddItem(item);		
	}
}
