using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSceneManager : MonoBehaviour {

	public ProgressBar player1HpBar;
	public ProgressBar player2HpBar;
	public Inventory player1Inventory;
	public Inventory player2Inventory;
	public Renderer vsRenderer;
    public Renderer readyRenderer;
    public Renderer Bluedoor;
    public Renderer Pinkdoor;
    public Renderer blueopen;
    public Renderer pinkopen;
    public Renderer Background;
    public Renderer BG_L;
    public Renderer BG_R;
    public Renderer darkfilter;

	private StateMachine game = new StateMachine();
	private StateMachine gate = new StateMachine();

    private StateMachine tension = new StateMachine();

    private bool onedooron;

	private int itemCount = 0;

    private int player1item = 0;
    private int player2item = 0;

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
            state.OnUpdate += GatePlayer1_OnUpdate;
			gate.AddState(state);
		}

		{
			State state = new State("player2");
			state.OnBegin += GatePlayer2_OnBegin;
            state.OnUpdate += GatePlayer2_OnUpdate;
			gate.AddState(state);			
		}

		{
			State state = new State("utopia");
			state.OnBegin += GateUtopia_OnBegin;
            state.OnUpdate += GateUtopia_OnUpdate;
			gate.AddState(state);
		}

        {
            State state = new State("low");
            state.OnBegin += TensionLow_OnBegin;
            tension.AddState(state);
        }

        {
            State state = new State("medium");
            state.OnBegin += TensionMedium_OnBegin;
            tension.AddState(state);
        }

        {
            State state = new State("high");
            state.OnBegin += TensionHigh_OnBegin;
            tension.AddState(state);
        }
	}

	void Start () {

		NotificationCenter.shared.AddHandler("Player1Hit", OnPlayer1Hit);
		NotificationCenter.shared.AddHandler("Player2Hit", OnPlayer2Hit);
		NotificationCenter.shared.AddHandler("Player1GetItem", OnPlayer1GetItem);
		NotificationCenter.shared.AddHandler("Player2GetItem", OnPlayer2GetItem);
        NotificationCenter.shared.AddHandler("ResetGame", ResetItem);
        NotificationCenter.shared.AddHandler("ResetGame", Reset_Game_Over);
        NotificationCenter.shared.AddHandler("DoorOn", DoorOn);
        NotificationCenter.shared.AddHandler("PlayerEnterColorDoor", EnterDoor);
        NotificationCenter.shared.AddHandler("Background_Change_Degree", BackgroundChange);

        game.BeginState("ready");
		gate.BeginState("closed");
        tension.BeginState("low");
	}
	
	// Update is called once per frame
	void Update () {
        game.Update(Time.deltaTime);
        gate.Update(Time.deltaTime);
	}

	void Ready_OnBegin(State state) {
		Debug.Log("Game.Ready.Begin");
        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        readyRenderer.material.SetColor("_TintColor", color);

        SoundPlayer.shared.Play("GamePlay_BGM_1", false);
        SoundPlayer.shared.Play("GamePlay_BGM_2", true);
        SoundPlayer.shared.Play("GamePlay_BGM_3", true);

        tension.BeginState("low");

        StartCoroutine(AlphatoDest(darkfilter, 0.03f));
    }

    void Ready_OnUpdate(State state)
    {

        if (state.elapsedTime < 2f)
        {
            Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f-state.elapsedTime/2);
            readyRenderer.material.SetColor("_TintColor", color);
        }
        else
        {
            Color color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            readyRenderer.material.SetColor("_TintColor", color);
            game.BeginState("play");
        }
    }

	void Play_OnBegin(State state) {
		Debug.Log("Game.Play.Begin");

	}

	void Play_OnUpdate(State state) {


        
    }

	void Over_OnBegin(State state) {
        StartCoroutine(AlphaOff(Bluedoor));
        StartCoroutine(AlphaOff(Pinkdoor));

	}

	void Over_OnUpdate(State state) {
        if (state.elapsedTime > 1f)
        {
            game.BeginState("ready");
            onedooron = false;
        }
	}

	void GateClosed_OnBegin(State state) {
		Debug.Log("Gate.Closed.Begin");
	}

	void GatePlayer1_OnBegin(State state) {
		Debug.Log("Gate.Player1.Begin");
	}

    void GatePlayer1_OnUpdate(State state)
    {
        if (state.elapsedTime > 1f)
        {
           Application.LoadLevel("Player1Win");
        }
    }


    void GatePlayer2_OnBegin(State state) {
		Debug.Log("Gate.Player2.Begin");
	}

    void GatePlayer2_OnUpdate(State state)
    {
        if (state.elapsedTime > 1f)
        {
            Application.LoadLevel("Player2Win");
        }
    }

    void GateUtopia_OnBegin(State state) {
		Debug.Log("Gate.Utopia.Begin");

        Background.enabled = false;
        BG_L.enabled = true;
        BG_R.enabled = true;

        Bluedoor.gameObject.GetComponent<Collider>().enabled = false;
        Pinkdoor.gameObject.GetComponent<Collider>().enabled = false;

        StartCoroutine(AlphaOff(Bluedoor));
        StartCoroutine(AlphaOff(Pinkdoor));
    }

    void GateUtopia_OnUpdate(State state)
    {
        if (BG_L.transform.position.x > -150.0f)
        {
            BG_L.transform.position -= new Vector3(5 * Time.deltaTime, 0, 0);
            BG_R.transform.position += new Vector3(5 * Time.deltaTime, 0, 0);
        }
        else {
            Application.LoadLevel("Utopia");
        }
    }
    
    void TensionLow_OnBegin(State state) {
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_1", 1.0f);
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_2", 0.0f);
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_3", 0.0f);
    }

    void TensionMedium_OnBegin(State state) {
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_1", 0.0f);
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_2", 1.0f);
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_3", 0.0f);
    }

    void TensionHigh_OnBegin(State state) {
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_1", 0.0f);
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_2", 0.0f);
        SoundPlayer.shared.SetSourceVolume("GamePlay_BGM_3", 1.0f);
    }

	void OnPlayer1Hit(Notification notification) {
		float hitPointRatio = (float)notification.userInfo;
		player1HpBar.SetValue(hitPointRatio);

        UpdateTension(hitPointRatio);	
    }

	void OnPlayer2Hit(Notification notification) {
		float hitPointRatio = (float)notification.userInfo;
		player2HpBar.SetValue(hitPointRatio);

        UpdateTension(hitPointRatio);
    }

    void UpdateTension(float hitPointRatio) {

        if (tension.currentState.name == "medium" && hitPointRatio < 0.33f) {
            tension.BeginState("high");
        }
        else if (tension.currentState.name == "low" && hitPointRatio < 0.66f) {
            tension.BeginState("medium");
        }
    }

	void OnPlayer1GetItem(Notification notification) {
		Item item = notification.userInfo as Item;

		if (player1Inventory.AddItem(item)) {
			OnAnyPlayerGetItem();
            player1item++;
            Notification _notification = new Notification("Background_Change_Degree", (int)Mathf.Abs(player1item - player2item));
            NotificationCenter.shared.PostNotification(_notification);
        }
	}

	void OnPlayer2GetItem(Notification notification) {
		Item item = notification.userInfo as Item;

        if (player2Inventory.AddItem(item)) {
			OnAnyPlayerGetItem();
            player2item++;
            Notification _notification = new Notification("Background_Change_Degree", (int)Mathf.Abs(player1item - player2item));
            NotificationCenter.shared.PostNotification(_notification);
        }
	}


    void OnAnyPlayerGetItem() {
		itemCount++;

		float alpha = (float)(Mathf.Max(0, 5 - itemCount) / 5.0f);
		Color color = new Color(1.0f, 1.0f, 1.0f, alpha);
		
		vsRenderer.material.SetColor("_TintColor", color);
	}

    void Reset_Game_Over(Notification notification)
    {
        game.BeginState("over");
    }

    void ResetItem(Notification notification)
    {
        player1Inventory.RemoveAllItems();
        player2Inventory.RemoveAllItems();
    }

    void DoorOn(Notification notification)
    {
        if (onedooron == false)
        {
            if ((int)notification.userInfo == 1)
            {
                Bluedoor.gameObject.GetComponent<Collider>().enabled = true;
                StartCoroutine(AlphaOn(Bluedoor));

            }
            else
            {
                Pinkdoor.gameObject.GetComponent<Collider>().enabled = true;
                StartCoroutine(AlphaOn(Pinkdoor));
            }
            onedooron = true;
        }else
        {
            gate.BeginState("utopia");//
        }
    }

    void EnterDoor(Notification notification)
    {
        if((string)notification.userInfo == "Player1")
        {
            StartCoroutine(AlphaOff(Bluedoor));
            StartCoroutine(AlphaOn(blueopen));
            gate.BeginState("player1");
        }
        else
        {
            StartCoroutine(AlphaOff(Pinkdoor));
            StartCoroutine(AlphaOn(pinkopen));
            gate.BeginState("player2");
        }
    }

    void BackgroundChange(Notification notification)
    {
        print(notification.userInfo);
        if ((int)notification.userInfo == 3)
        {
            StartCoroutine(AlphatoDest(darkfilter, 0.15f));
        }else if((int)notification.userInfo == 2)
        {
            StartCoroutine(AlphatoDest(darkfilter, 0.07f));
        }
        else
        {
            StartCoroutine(AlphatoDest(darkfilter, 0.03f));
        }
    }

    IEnumerator AlphaOn(Renderer R)
    {
        float a;
        a = R.material.GetColor("_TintColor").a + Time.deltaTime;
        while (a < .5f)
        {
            a += Time.deltaTime;
            Color color = new Color(.5f, .5f, .5f, a);
            R.material.SetColor("_TintColor", color);
            yield return null;
        }
    }

    IEnumerator AlphaOff(Renderer R)
    {
        float a;
        a = R.material.GetColor("_TintColor").a - Time.deltaTime;
        while (a > 0f)
        {
            a -= Time.deltaTime;
            Color color = new Color(.5f, .5f, .5f, a);
            R.material.SetColor("_TintColor", color);
            yield return null;
        }
    }

    IEnumerator AlphatoDest(Renderer R, float destination)
    {
        float a =  R.material.GetColor("_TintColor").a;
        if (a > destination)
        {
            while (Mathf.Abs(a-destination)>0.01f)
            {
                a -= Time.deltaTime;
                Color color = new Color(.5f, .5f, .5f, a);
                R.material.SetColor("_TintColor", color);
                yield return null;
            }
            a = destination;
        }else if(a<destination)
        {
            while (Mathf.Abs(a - destination) > 0.01f)
            {
                a += Time.deltaTime;
                Color color = new Color(.5f, .5f, .5f, a);
                R.material.SetColor("_TintColor", color);
                yield return null;
            }
        }
    }

}
