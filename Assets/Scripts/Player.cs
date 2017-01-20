using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public InputConfig inputConfig;
	private CharacterController character;


	private float hitPoint = 0;
	private float moveSpeed = 0;
	private float moveAccel = 0;
	private float moveDecel = 0;
	private float jumpPower = 0;

	private float currentMoveVelocity = 0.0f;
	private Vector3 inertia = Vector3.zero;

	private StateMachine locomotion = new StateMachine();
	private StateMachine jumping = new StateMachine();
	private StateMachine firing = new StateMachine();

	void Awake() {

		character = GetComponent<CharacterController>();

		{
			State state = new State("idle");
			state.OnBegin += Idle_OnBegin;
			state.OnUpdate += Idle_OnUpdate;
			state.OnEnd += Idle_OnEnd;

			locomotion.AddState(state);
		}

		{
			State state = new State("move");
			state.OnBegin += Move_OnBegin;
			state.OnUpdate += Move_OnUpdate;
			state.OnEnd += Move_OnEnd;

			locomotion.AddState(state);
		}		

		{
			State state = new State("crouch");
			state.OnBegin += Crouch_OnBegin;
			state.OnEnd += Crouch_OnEnd;

			locomotion.AddState(state);
		}

		{
			State state = new State("grounded");
			state.OnBegin += Grounded_OnBegin;
			state.OnUpdate += Grounded_OnUpdate;
			state.OnEnd += Grounded_OnEnd;

			jumping.AddState(state);
		}

		{
			State state = new State("hover");
			state.OnBegin += Hover_OnBegin;
			state.OnUpdate += Hover_OnUpdate;
			state.OnEnd += Hover_OnEnd;

			jumping.AddState(state);
		}

		{
			State state = new State("ceaseFire");
			state.OnBegin += CeaseFire_OnBegin;
			state.OnEnd += CeaseFire_OnEnd;

			firing.AddState(state);
		}

		{
			State state = new State("openFire");
			state.OnBegin += OpenFire_OnBegin;
			state.OnEnd += OpenFire_OnEnd;

			firing.AddState(state);
		}
	}

	void Start () {
		
		hitPoint = GameConfig.shared.hitPoint;
		moveSpeed = GameConfig.shared.moveSpeed;
		moveAccel = GameConfig.shared.moveAccel;
		moveDecel = GameConfig.shared.moveDecel;
		jumpPower = GameConfig.shared.jumpPower;

		locomotion.BeginState("idle");
		jumping.BeginState("grounded");
		firing.BeginState("ceaseFire");
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(inputConfig.crouch)) {
			locomotion.BeginState("crouch");
		}

		if (Input.GetKeyUp(inputConfig.crouch)) {
			locomotion.BeginState("idle");
		}

		if (Input.GetKeyDown(inputConfig.fire)) {
			firing.BeginState("openFire");
		}

		if (Input.GetKeyUp(inputConfig.fire)) {
			firing.BeginState("ceaseFire");
		}

		locomotion.Update(Time.deltaTime);
		jumping.Update(Time.deltaTime);
		firing.Update(Time.deltaTime);

		UpdateMovement();
		UpdateInertia();
		UpdateCollision();
	}

	void Idle_OnBegin(State state) {

	}

	void Idle_OnUpdate(State state) {

		bool moveLeft = Input.GetKey(inputConfig.moveLeft);
		bool moveRight = Input.GetKey(inputConfig.moveRight);
		
		if (moveLeft || moveRight) {
			locomotion.BeginState("move");
		}
	}

	void Idle_OnEnd(State state) {

	}

	void Move_OnBegin(State state) {

	}

	void Move_OnUpdate(State state) {

		bool moveLeft = Input.GetKey(inputConfig.moveLeft);
		bool moveRight = Input.GetKey(inputConfig.moveRight);
		
		if (!CheckGrounded()) {
			jumping.BeginState("hover");
		}
		else if (!moveLeft && !moveRight) {
			locomotion.BeginState("idle");
		}
	}

	void Move_OnEnd(State state) {

	}

	void Crouch_OnBegin(State state) {

	}

	void Crouch_OnEnd(State state) {

	}

	void Grounded_OnBegin(State state) {

	}

	void Grounded_OnUpdate(State state) {

		if (Input.GetKeyDown(inputConfig.jump) && state.elapsedTime > 0.1f) {

			inertia += Vector3.up * jumpPower;
			jumping.BeginState("hover");
		}
	}

	void Grounded_OnEnd(State state) {

	}

	void Hover_OnBegin(State state) {
		
	}

	void Hover_OnUpdate(State state) {

		if (inertia.y <= 0.0f && CheckGrounded()) {
			jumping.BeginState("grounded");
		}
	}

	void Hover_OnEnd(State state) {

	}

	void CeaseFire_OnBegin(State state) {

	}

	void CeaseFire_OnEnd(State state) {

	}

	void OpenFire_OnBegin(State state) {

	}

	void OpenFire_OnEnd(State state) {

	}

	bool CheckGrounded() {
		
		Vector3 origin = character.transform.position;
		float length = character.height * 0.55f;

		Ray ray = new Ray(origin, Vector3.down);
		RaycastHit[] hits = Physics.RaycastAll(ray, length);

		return hits.Length > 0;
	}

	void UpdateMovement() {

		if (locomotion.currentState.name == "move") {

			if (Input.GetKey(inputConfig.moveLeft)) {

				currentMoveVelocity -= moveAccel * Time.deltaTime;
				currentMoveVelocity = Mathf.Max(currentMoveVelocity, -moveSpeed);
			}

			if (Input.GetKey(inputConfig.moveRight)) {

				currentMoveVelocity += moveAccel * Time.deltaTime;
				currentMoveVelocity = Mathf.Min(currentMoveVelocity, moveSpeed);
			}
		}
		else {

			if (currentMoveVelocity < 0.0f) {

				currentMoveVelocity += moveDecel * Time.deltaTime;
				currentMoveVelocity = Mathf.Min(currentMoveVelocity, 0.0f);
			}
			else if (currentMoveVelocity > 0.0f) {

				currentMoveVelocity -= moveDecel * Time.deltaTime;
				currentMoveVelocity = Mathf.Max(currentMoveVelocity, 0.0f);
			}
		}

		Vector3 planarVelocity = new Vector3(currentMoveVelocity, 0.0f, 0.0f);

		Vector3 velocity = inertia + planarVelocity;
		Vector3 deltaPosition = velocity * Time.deltaTime;
		
		character.Move(deltaPosition);
	}

	void UpdateInertia() {

		if (jumping.currentState.name == "hover") {
			inertia += Vector3.down * GameConfig.shared.gravity * Time.deltaTime;
		}
		else {
			inertia.y = 0.0f;
		}
	}

	void UpdateCollision() {

		LayerMask platform = LayerMask.NameToLayer("Platform");
		
		if (inertia.y > 0.0f) {
			Physics.IgnoreLayerCollision(gameObject.layer, platform, true);
		}
		else {
			Physics.IgnoreLayerCollision(gameObject.layer, platform, false);
		}
	}
}
