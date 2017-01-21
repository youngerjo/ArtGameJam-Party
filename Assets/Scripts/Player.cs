using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public InputConfig inputConfig;
	public Bullet bulletPrefab;

	public enum LookAt {
		Left,
		Right
	}

	public LookAt lookAt = LookAt.Left;

	private CharacterController character;


	private float hitPoint = 0.0f;
	private float stunPoint = 0.0f;
	private float stunRegen = 0.0f;
	private float moveSpeed = 0.0f;
	private float moveAccel = 0.0f;
	private float moveDecel = 0.0f;
	private float jumpPower = 0.0f;
	private float coolTime = 0.0f;
	private float bulletPower = 0.0f;
	private float hitPower = 0.0f;

	private float currentHitPoint = 0.0f;
	private float currentStunPoint = 0.0f;
	private float currentMoveVelocity = 0.0f;
	private Vector3 inertia = Vector3.zero;

	private StateMachine activity = new StateMachine();
	private StateMachine locomotion = new StateMachine();
	private StateMachine jumping = new StateMachine();
	private StateMachine firing = new StateMachine();

	void Awake() {

		character = GetComponent<CharacterController>();

		{
			State state = new State("alive");
			state.OnBegin += Alive_OnBegin;
			state.OnUpdate += Alive_OnUpdate;

			activity.AddState(state);
		}

		{
			State state = new State("dead");
			state.OnBegin += Dead_OnBegin;
			state.OnUpdate += Dead_OnUpdate;

			activity.AddState(state);
		}

		{
			State state = new State("idle");
			state.OnBegin += Idle_OnBegin;
			state.OnUpdate += Idle_OnUpdate;

			locomotion.AddState(state);
		}

		{
			State state = new State("move");
			state.OnBegin += Move_OnBegin;
			state.OnUpdate += Move_OnUpdate;

			locomotion.AddState(state);
		}		

		{
			State state = new State("crouch");
			state.OnBegin += Crouch_OnBegin;
			state.OnUpdate += Crouch_OnUpdate;

			locomotion.AddState(state);
		}

		{
			State state = new State("stunned");
			state.OnBegin += Stunned_OnBegin;
			state.OnUpdate += Stunned_OnUpdate;

			locomotion.AddState(state);
		}

		{
			State state = new State("grounded");
			state.OnBegin += Grounded_OnBegin;
			state.OnUpdate += Grounded_OnUpdate;

			jumping.AddState(state);
		}

		{
			State state = new State("hover");
			state.OnBegin += Hover_OnBegin;
			state.OnUpdate += Hover_OnUpdate;

			jumping.AddState(state);
		}

		{
			State state = new State("ceaseFire");
			state.OnBegin += CeaseFire_OnBegin;
			state.OnUpdate += CeaseFire_OnUpdate;

			firing.AddState(state);
		}

		{
			State state = new State("openFire");
			state.OnBegin += OpenFire_OnBegin;
			state.OnUpdate += OpenFire_OnUpdate;

			firing.AddState(state);
		}
	}

	void Start () {
		activity.BeginState("alive");
	}
	
	// Update is called once per frame
	void Update () {

		activity.Update(Time.deltaTime);

		if (activity.currentState.name == "dead") {
		}
		else if (activity.currentState.name == "alive") {

			if (locomotion.currentState.name != "stunned") {

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
			}

			locomotion.Update(Time.deltaTime);
			jumping.Update(Time.deltaTime);
			firing.Update(Time.deltaTime);

			UpdateMovement();
			UpdateInertia();
			UpdateCollision();
			UpdateStunPoint();
		}
	}

	void OnTriggerEnter(Collider other) {

		Bullet bullet = other.GetComponent<Bullet>();

		if (bullet != null && !bullet.isPurged) {
			bullet.Purge();

			TakeDamageFromBullet(bullet);
		}
	}

	void Alive_OnBegin(State state) {

		hitPoint = GameConfig.shared.hitPoint;
		stunPoint = GameConfig.shared.stunPoint;
		stunRegen = GameConfig.shared.stunRegen;
		moveSpeed = GameConfig.shared.moveSpeed;
		moveAccel = GameConfig.shared.moveAccel;
		moveDecel = GameConfig.shared.moveDecel;
		jumpPower = GameConfig.shared.jumpPower;
		coolTime = GameConfig.shared.coolTime;
		bulletPower = GameConfig.shared.bulletPower;
		hitPower = GameConfig.shared.hitPoint;

		currentHitPoint = hitPoint;
		currentStunPoint = hitPoint;
		currentMoveVelocity = 0.0f;

		locomotion.BeginState("idle");
		jumping.BeginState("grounded");
		firing.BeginState("ceaseFire");
	}

	void Alive_OnUpdate(State state) {

	}

	void Dead_OnBegin(State state) {

	}

	void Dead_OnUpdate(State state) {

	}

	void Idle_OnBegin(State state) {

	}

	void Idle_OnUpdate(State state) {

		bool moveLeft = Input.GetKey(inputConfig.moveLeft);
		bool moveRight = Input.GetKey(inputConfig.moveRight);
		bool crouch = Input.GetKey(inputConfig.crouch);
		
		if (crouch) {
			locomotion.BeginState("crouch");
		}
		else if (moveLeft || moveRight) {
			locomotion.BeginState("move");
		}
	}

	void Move_OnBegin(State state) {

	}

	void Move_OnUpdate(State state) {

		bool moveLeft = Input.GetKey(inputConfig.moveLeft);
		bool moveRight = Input.GetKey(inputConfig.moveRight);
		bool crouch = Input.GetKey(inputConfig.crouch);
		
		if (crouch) {
			locomotion.BeginState("crouch");
		}
		else if (moveLeft && moveRight) {
			// Pressing both keys. Do nothing
		}
		else if (moveLeft) {
			lookAt = LookAt.Left;
		}
		else if (moveRight) {
			lookAt = LookAt.Right;
		}
		else {
			locomotion.BeginState("idle");
		}
	}

	void Crouch_OnBegin(State state) {

	}

	void Crouch_OnUpdate(State state) {

		// if (state.elapsedTime > 1.5f) {
		// 	locomotion.BeginState("idle");
		// 	jumping.BeginState("hover");
		// }

		if (Input.GetKeyUp(inputConfig.crouch)) {
			locomotion.BeginState("idle");
		}
	}

	void Stunned_OnBegin(State state) {
		firing.BeginState("ceaseFire");
	}

	void Stunned_OnUpdate(State state) {

		if (currentStunPoint > stunPoint * 0.2f) {
			locomotion.BeginState("idle");
		}
	}

	void Grounded_OnBegin(State state) {

	}

	void Grounded_OnUpdate(State state) {

		if (Input.GetKeyDown(inputConfig.jump) && state.elapsedTime > 0.1f) {

			inertia += Vector3.up * jumpPower;
			jumping.BeginState("hover");
		}
		else if (!CheckGrounded()) {
			jumping.BeginState("hover");
		}
	}

	void Hover_OnBegin(State state) {
		
	}

	void Hover_OnUpdate(State state) {

		if (inertia.y <= 0.0f && CheckGrounded()) {
			jumping.BeginState("grounded");
		}
	}

	void CeaseFire_OnBegin(State state) {

	}

	void CeaseFire_OnUpdate(State state) {
		
	}

	void OpenFire_OnBegin(State state) {
		ShootBullet();
	}

	void OpenFire_OnUpdate(State state) {

		if (state.elapsedTime > coolTime) {
			ShootBullet();
			state.ResetTime();
		}

		if (Input.GetKeyUp(inputConfig.fire)) {
			firing.BeginState("ceaseFire");
		}
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

	void UpdateStunPoint() {

		currentStunPoint += stunRegen * Time.deltaTime;
		currentStunPoint = Mathf.Min(currentStunPoint, stunPoint);
	}

	private void ShootBullet() {

		Vector3 origin = transform.position + Vector3.up * 0.5f;
		Vector3 direction = Vector3.zero;

		if (lookAt == LookAt.Left) {
			direction = Vector3.left;
		}
		else if (lookAt == LookAt.Right) {
			direction = Vector3.right;
		}

		origin += direction;

		GameObject go = GameObject.Instantiate(bulletPrefab.gameObject, origin, Quaternion.identity);
		go.GetComponent<Rigidbody>().AddForce(direction * bulletPower);

		if (Random.Range(0.0f, 1.0f) < GameConfig.shared.itemChance) {
			go.GetComponent<Bullet>().item = new Item();
		}
	}

	private void TakeDamageFromBullet(Bullet bullet) {

		currentHitPoint -= bullet.bulletDamage;

		if (currentHitPoint <= 0.0f) {
			activity.BeginState("dead");
		}

		currentStunPoint -= bullet.bulletStunDamage;
		
		if (currentStunPoint <= 0.0f) {
			locomotion.BeginState("stunned");
		}
	}
}
