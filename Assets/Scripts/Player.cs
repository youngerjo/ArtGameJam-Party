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
	private float currentCoolTime = 0.0f;
	private Vector3 inertia = Vector3.zero;

    private bool jumpenabled=true;

	private StateMachine activity = new StateMachine();
	private StateMachine locomotion = new StateMachine();
	private StateMachine jumping = new StateMachine();
	private StateMachine firing = new StateMachine();
    private StateMachine gettinghit = new StateMachine();
    private Animator animator;

	void Awake() {
        
		character = GetComponent<CharacterController>();
        animator = transform.FindChild("Sprite").GetComponent<Animator>();

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

        {
            State state = new State("getHit");
            state.OnBegin += GetHit_OnBegin;
            state.OnUpdate += GetHit_OnUpdate;

            gettinghit.AddState(state);
        }

        {
            State state = new State("notHit");
            state.OnBegin += NotHit_OnBegin;
            state.OnUpdate += NotHit_OnUpdate;

            gettinghit.AddState(state);
        }
    }

	void Start () {
		activity.BeginState("alive");

        NotificationCenter.shared.AddHandler("ResetGame", ResetOnGameOver_repeat);
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

                if (gettinghit.currentState.name == "notHit")
                {
                    if (Input.GetKeyDown(inputConfig.fire))
                    {
                        firing.BeginState("openFire");
                    }

                    if (Input.GetKeyUp(inputConfig.fire))
                    {
                        firing.BeginState("ceaseFire");
                    }
                }
              
			}

			locomotion.Update(Time.deltaTime);
			jumping.Update(Time.deltaTime);
			firing.Update(Time.deltaTime);
            gettinghit.Update(Time.deltaTime);

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

            if (locomotion.currentState.name == "stunned")
            {
                if (locomotion.currentState.elapsedTime > 1.5f)
                {
                    TakeDamageFromBullet(bullet);
                }
            }
            else
            {
                TakeDamageFromBullet(bullet);
            }
			
		}

    }

    void OnTriggerStay(Collider other)
    {
        if (LayerMask.LayerToName(gameObject.layer) + "door" == other.name )
        {
            jumpenabled = false;
            if(Input.GetKey(inputConfig.jump)&&jumping.currentState.name!="hover"){
                Notification notification = new Notification("PlayerEnterColorDoor", LayerMask.LayerToName(gameObject.layer));
                NotificationCenter.shared.PostNotification(notification);
                animator.SetBool("door", true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(LayerMask.LayerToName(gameObject.layer) + "door" == other.name)
        {
            jumpenabled = true;
        }
    }

    private void ResetPlayer()
    {
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
        currentCoolTime = coolTime;


        locomotion.BeginState("idle");
        jumping.BeginState("grounded");
        firing.BeginState("ceaseFire");
        gettinghit.BeginState("notHit");
    }

    void Alive_OnBegin(State state)
    {
        ResetPlayer();
    }
        

    void Alive_OnUpdate(State state) {

	}

	void Dead_OnBegin(State state) {
        animator.SetBool("dead", true);
	}

	void Dead_OnUpdate(State state) {
        if (state.elapsedTime > 1f)
        {
            animator.SetBool("dead", false);
            animator.SetBool("idle", false);
            animator.SetBool("stun", false);
            animator.SetBool("hover", false);
            animator.SetBool("stopfire", true);
            animator.SetBool("jump", false);
            activity.BeginState("alive");// Do level initialization
        }
	}

	void Idle_OnBegin(State state) {
        animator.SetBool("idle", false);
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
            if(transform.FindChild("Sprite").transform.localScale.x != 1)
            {
                Vector3 scale = transform.FindChild("Sprite").transform.localScale;
                scale.x *= -1;
                transform.FindChild("Sprite").transform.localScale = scale;
            }
		}
		else if (moveRight) {
			lookAt = LookAt.Right;
            if (transform.FindChild("Sprite").transform.localScale.x != -1)
            {
                Vector3 scale = transform.FindChild("Sprite").transform.localScale;
                scale.x *= -1;
                transform.FindChild("Sprite").transform.localScale = scale;
            }
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
        animator.SetBool("stun", true);
	}

	void Stunned_OnUpdate(State state) {

		if (state.elapsedTime>3f) {// currentStunPoint > stunPoint * 0.2f) {
            animator.SetBool("stun", false);
			locomotion.BeginState("idle");
            currentStunPoint = stunPoint;
		}
	}

	void Grounded_OnBegin(State state) {
       // animator.SetBool("idle", false);
        // if (locomotion.currentState.name!="idle" && ) {

        // }
    }

	void Grounded_OnUpdate(State state) {

        if (animator.GetBool("idle"))
        {
            animator.SetBool("idle", false);
        }

		if (Input.GetKeyDown(inputConfig.jump) && state.elapsedTime > 0.1f&&locomotion.currentState.name !="stunned" && jumpenabled) {
            
			inertia += Vector3.up * jumpPower;
			jumping.BeginState("hover");
            animator.SetBool("jump", true);
        }

		else if (!CheckGrounded()) {
            animator.SetBool("hover", true);
            jumping.BeginState("hover");
		}
	}

	void Hover_OnBegin(State state) {
        if (firing.currentState.name != "openFire")
        {
           
        }
    }

	void Hover_OnUpdate(State state) {

		if (inertia.y <= 0.0f && CheckGrounded()) {
			jumping.BeginState("grounded");
            animator.SetBool("jump", false);
            animator.SetBool("hover", false);
            animator.SetBool("idle", true);
        }
	}

	void CeaseFire_OnBegin(State state) {
        animator.SetBool("stopfire", true);
    }

	void CeaseFire_OnUpdate(State state) {
		
	}

	void OpenFire_OnBegin(State state) {
        animator.SetBool("stopfire", false);
        //animator.SetTrigger("fire");
		ShootBullet();
	}

	void OpenFire_OnUpdate(State state) {

		if (state.elapsedTime > currentCoolTime) {
			ShootBullet();
			state.ResetTime();

			currentCoolTime = coolTime * Random.Range(0.5f, 1.5f);
		}
        
        if (Input.GetKeyUp(inputConfig.fire))
        {
            print("CEASE!");
            
            firing.BeginState("ceaseFire");
            
        }
	}


    void GetHit_OnBegin(State state)
    {
        animator.SetTrigger("attacked");
        firing.BeginState("ceaseFire");

    }

    void GetHit_OnUpdate(State state)
    {

        if (state.elapsedTime > GameConfig.shared.hitDelay)
        {
            gettinghit.BeginState("notHit");


        }
    }

    void NotHit_OnBegin(State state){
        
    }

    void NotHit_OnUpdate(State state)
    {

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
        animator.SetFloat("vel", Mathf.Abs(currentMoveVelocity));
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
		origin.y += Random.Range(-1.0f, 1.0f) * 0.5f;

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

		if (activity.currentState.name != "alive") {
			return;
		}

		currentHitPoint -= bullet.bulletDamage;

        string playerName = LayerMask.LayerToName(gameObject.layer);

        if (currentHitPoint <= 0.0f) {
            Notification notification = new Notification("ResetGame", LayerMask.LayerToName(gameObject.layer));
            NotificationCenter.shared.PostNotification(notification);
            Debug.Log("Game.Over.Begin");
            return;
            //activity.BeginState("dead");
        }
		else {
			gettinghit.BeginState("getHit");
		}

		

		{
			string notificationName = playerName + "Hit";
			float hitPointRatio = currentHitPoint / hitPoint;

			Notification notification = new Notification(notificationName, hitPointRatio);
			NotificationCenter.shared.PostNotification(notification);
		}

		if (bullet.item != null) {
			string notificationName = playerName + "GetItem";

			Notification notification = new Notification(notificationName, bullet.item);
			NotificationCenter.shared.PostNotification(notification);
		}

		currentStunPoint -= bullet.bulletStunDamage;
		
		if (currentStunPoint <= 0.0f) {
			locomotion.BeginState("stunned");
		}
	}
     void ResetOnGameOver_repeat(Notification notification)
    {
        string playerName = LayerMask.LayerToName(gameObject.layer);
        {
            string notificationName = playerName + "Hit";


            Notification _notification = new Notification(notificationName, 1f);
            NotificationCenter.shared.PostNotification(_notification);
        }


        string playernum = (string)notification.userInfo;
        if (playernum == LayerMask.LayerToName(gameObject.layer))
        {
            activity.BeginState("dead");
        }else
        {
            ResetPlayer();
        }
        
    }

}
