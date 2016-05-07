using UnityEngine;
using System.Collections;

public class MasterHandScript : MonoBehaviour {

	BossState currState;

	GameObject player;

	Vector3 dist;
	Vector3 playerPos;
	Vector3 smashTargetPosition;
	Vector3 groundPos;
	Vector3 swipeStartPosition;
	Vector3 swipeTargetPosition;

	public float moveMult;
	float idleMoveSpeed = 7f;
	float resetMoveSpeed = 1f;
	float smashSearchMoveSpeed = 2f;
	float smashMoveSpeed = 5f;
	float playerMoveDir;
	float randTime;
	float randDist;
	float randMult;
	int randMoveSelect;
	GameObject fingerIndexTip;
	GameObject fingerMiddleTip;
	GameObject fingerRingTip;
	GameObject fingerPinkyTip;
	GameObject fingerThumbTip;
	PlayerMovement playerMoveComp;

	RaycastHit[] hits;

	bool closeLaserCooling;

	// Use this for initialization
	void Start () {
		checkForGround ();
		currState = BossState.RESET_TO_PLAYER;
		player = (GameObject) GameObject.FindGameObjectWithTag ("Player");
		playerMoveComp = player.GetComponent<PlayerMovement> ();
		playerMoveDir = playerMoveComp.movementDir;

		// Get fingers
		fingerIndexTip = transform.FindChild ("FingerIndexTip").gameObject;
		fingerMiddleTip = transform.FindChild ("FingerMiddleTip").gameObject;
		fingerRingTip = transform.FindChild ("FingerRingTip").gameObject;
		fingerPinkyTip = transform.FindChild ("FingerPinkyTip").gameObject;
		fingerThumbTip = transform.FindChild ("FingerThumbTip").gameObject;


		// Set initial bools
		closeLaserCooling = false;
	}
	
	// Update is called once per frame
	void Update () {

		// States Machine
		switch (currState) {
		case BossState.IDLE:
			idling ();
			transform.LookAt (player.transform.position);
			laserClosePlayer ();
			break;
		case BossState.RESET_TO_PLAYER:
			//transform.LookAt (player.transform.position);
			resetToPlayer ();
			break;
		case BossState.LASERS:
			break;
		case BossState.SMASH_SEARCH:
			setSmashRotation ();
			smashSearch ();
			break;
		case BossState.SMASH:
			smashMove ();
			break;
		case BossState.PRE_SWIPE:
			setSwipeRotation ();
			preSwipe ();
			break;
		case BossState.SWIPE:
			swipe ();
			break;
		}
	}


	/*
	~~~~~~~~~~~~~~~~~~~~~~
		IDLE SELECT MOVE
	~~~~~~~~~~~~~~~~~~~~~~
	*/

	void idling () {
		if (!IsInvoking ("selectMove")) {
			randTime = Random.Range (2f, 4f);
			Invoke ("selectMove", randTime);
		}
	}

	void selectMove () {
		randMoveSelect = (int) Random.Range (0, 2);
		Debug.Log (randMoveSelect);
		switch (randMoveSelect) {
		case 0:
			setSmashSearchState ();
			break;
		case 1:
			setPreSwipeState ();
			break;
		case 2:
			setLasersState ();
			break;
		}
	}

	/*
	~~~~~~~~~~~~~~~~~~~~~~
		LASER MOVES
	~~~~~~~~~~~~~~~~~~~~~~
	*/

	void laserClosePlayer () {
		dist = transform.position - player.transform.position;
		if (!closeLaserCooling) {
			if (Mathf.Abs (dist.x) < 10f) {
				fireMiddleLaser ();
				closeLaserCooling = true;
				Invoke ("resetCoolingCloseLaser", 6);
			}
		}
	}
		
	public void shootLazer(GameObject lazerSource)
	{
		setDistDir ();
		
		LineRenderer lr = gameObject.AddComponent<LineRenderer>();
		lr.SetVertexCount(2);
		lr.material = new Material (Shader.Find("Particles/Additive"));
		lr.SetColors(Color.red, Color.magenta);
		lr.SetPosition(0, lazerSource.transform.position);
		lr.SetPosition(1, player.transform.position);
		lr.SetWidth(0.3f, 0.2f);
		// Raycast dat shiz
		hits = Physics.RaycastAll (transform.position, transform.forward, 20f);
		foreach (RaycastHit hit in hits) {
			if (hit.transform.CompareTag ("Player")) {
				hit.transform.gameObject.GetComponent<PlayerHealth> ().TakeDamage (5);
				player.GetComponent<Rigidbody> ().AddForce (new Vector3(-250f * moveMult, 0, 0));
			}
		}
		Invoke("destroyLazer", 0.2f);
	}

	public void destroyLazer()
	{
		Destroy(GetComponent<LineRenderer>());
	}

	void fireMiddleLaser () {
		shootLazer (transform.FindChild ("FingerMiddleTip").gameObject);
	}

	void fireIndexLaser () {
		shootLazer (transform.FindChild ("FingerIndexTip").gameObject);
	}

	void fireRingLaser () {
		shootLazer (transform.FindChild ("FingerRingTip").gameObject);
	}

	void firePinkyLaser () {
		shootLazer (transform.FindChild ("FingerPinkyTip").gameObject);
	}

	void resetCoolingCloseLaser () {
		closeLaserCooling = false;
	}

	/*
	~~~~~~~~~~~~~~~~~~~~~~~~~~~
		SMASH MOVES
	~~~~~~~~~~~~~~~~~~~~~~~~~~~
	*/

	void smashSearch () {
		setDistDir ();
		playerPos = player.transform.position;
		transform.position = Vector3.Lerp(transform.position, new Vector3 (playerPos.x + (3 * playerMoveDir), playerPos.y + 15f, playerPos.z), Time.deltaTime * smashSearchMoveSpeed);
		smashTargetPosition = playerPos;
		if (!IsInvoking ("setSmashState")) {
			randTime = Random.Range (3f, 6f);
			Invoke ("setSmashState", randTime);
		}
	}

	void smashMove () {
		transform.position = Vector3.Lerp(transform.position, new Vector3 (smashTargetPosition.x, groundPos.y,smashTargetPosition.z), Time.deltaTime * smashMoveSpeed);

		if (!IsInvoking ("setResetState")) {
			Invoke ("setResetState", 2);
		}
	}

	void setSmashRotation () {
		transform.localRotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 180, 270), Time.deltaTime * smashSearchMoveSpeed);
	}
		
	

	/*
	~~~~~~~~~~~~~~~~~~~~~~
		SWIPE MOVE
	~~~~~~~~~~~~~~~~~~~~~~
	*/

	void preSwipe () {
		setDistDir ();
		playerPos = player.transform.position;

		if ((int) Random.Range (0, 2) == 1) {
			randMult = 1;
		} else {
			randMult = -1;
		}

		if (swipeStartPosition == Vector3.zero) {
			randDist = Random.Range(20f, 25f);
			swipeStartPosition = new Vector3 (playerPos.x + (randDist * moveMult), playerPos.y + 3f, playerPos.z);
		}

		randDist = Random.Range(10f, 15f);
		swipeTargetPosition = new Vector3(playerPos.x - (randDist * moveMult), swipeStartPosition.y, playerPos.z);

		transform.position = Vector3.Lerp(transform.position, swipeStartPosition, Time.deltaTime * smashSearchMoveSpeed);
		if (!IsInvoking ("setSwipeState")) {
			randTime = Random.Range (2f, 3f);
			Invoke ("setSwipeState", randTime);
		}
	}

	void swipe () {
		transform.position = Vector3.Lerp(transform.position, swipeTargetPosition, Time.deltaTime * smashMoveSpeed);

		if (!IsInvoking ("setResetState")) {
			Invoke ("setResetState", 3);
		}
	}

	void setSwipeRotation () {
			transform.localRotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (90, 180 * moveMult, 0), Time.deltaTime * smashSearchMoveSpeed);
	}

	/*
	~~~~~~~~~~~~~~~~~~~~~~
		Misc Functions
	~~~~~~~~~~~~~~~~~~~~~~
	*/

	void checkForGround () {
		hits = Physics.RaycastAll (transform.position, new Vector3 (0, -1, 0), 100f);
		foreach (RaycastHit hit in hits) {
			if (hit.transform.CompareTag ("Ground")) {
				groundPos = hit.transform.position;
			}
		}
	}

	void setIdleRotation () {
		transform.localRotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 270, 0), Time.deltaTime * idleMoveSpeed);
	}


	/*
	~~~~~~~~~~~~~~~~~~~~~~
		Reset Functions
	~~~~~~~~~~~~~~~~~~~~~~
	*/

	void resetTargetPositions () {
		swipeTargetPosition = Vector3.zero;
		swipeStartPosition = Vector3.zero;
		smashTargetPosition = Vector3.zero;
		randMult = 0;
		randDist = 0;
	}

	void resetToPlayer () {
		resetTargetPositions ();
		setDistDir ();
		playerPos = player.transform.position;
		transform.position = Vector3.Lerp(transform.position, new Vector3 (playerPos.x + (15f * moveMult), playerPos.y + 5f, playerPos.z), Time.deltaTime * resetMoveSpeed);
		if (!IsInvoking ("setIdleState")) {
			Invoke ("setIdleState", 4);
		}
	}

	void setDistDir () {
		playerMoveDir = playerMoveComp.movementDir;
		dist = transform.position - player.transform.position;
		if (dist.x > 0)
			moveMult = 1;
		else
			moveMult = -1;
	}

	/*
	~~~~~~~~~~~~~~~~~~~~~~
		STATE SETTERS
	~~~~~~~~~~~~~~~~~~~~~~
	*/

	void setResetState () {
		currState = BossState.RESET_TO_PLAYER;
	}

	void setIdleState () {
		currState = BossState.IDLE;
	}

	void setSmashSearchState () {
		currState = BossState.SMASH_SEARCH;
	}

	void setSmashState () {
		currState = BossState.SMASH;
	}

	void setPreSwipeState () {
		currState = BossState.PRE_SWIPE;
	}

	void setSwipeState () {
		currState = BossState.SWIPE;
	}

	void setLasersState () {
		currState = BossState.LASERS;
	}

	public BossState getCurrState () {
		return currState;
	}

	public enum BossState {
		IDLE,
		RESET_TO_PLAYER,
		SMASH_SEARCH,
		SMASH,
		PRE_SWIPE,
		SWIPE,
		LASERS
	}
}
