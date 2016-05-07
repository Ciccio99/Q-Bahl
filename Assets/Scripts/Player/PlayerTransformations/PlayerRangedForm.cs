using UnityEngine;
using System.Collections;

public class PlayerRangedForm : PlayerTransform {

	GameObject laserBeamObject;
	GameObject laserBeamClone;
	GameObject laserBulletObject;
	GameObject laserBulletClone;
	GameObject bombObject;
	GameObject bombClone;
	Transform playerArm;
	PlayerAiming playerAim;
	float randAngle;


	public float cooldown1;
	public bool isCooling1 = false;
	public float cooldown2;
	public bool isCooling2 = false;
	public float cooldown3;
	public bool isCooling3 = false;

	void Awake () {
		playerAim = GetComponent<PlayerAiming> ();
		laserBeamObject = Resources.Load ("Prefabs/Abilities/LaserBeam") as GameObject;
		laserBulletObject = Resources.Load ("Prefabs/Abilities/LaserBullet") as GameObject;
		bombObject = Resources.Load ("Prefabs/Abilities/RangedSpecialBomb") as GameObject;
		playerArm = transform.FindChild ("PlayerArm");
		transform_name = "Ranged";
		transform_description = "I'm over here.";
	}
	
	public override void Ability1() {
		Debug.Log ("Ranged Ability 1");
		fireGatlingGun ();
	}
	
	public override void Ability2() {
		Debug.Log ("Ranged Ability 2");
		shootBeamLaser ();
	}
	
	public override void SpecialAbility() {
		Debug.Log ("Ranged Special Ability");
		shootBomb();
	}

	void fireGatlingGun () {
		if (!isCooling1) {
			Vector3 bulletDir = (playerAim.mouse_pos - transform.position).normalized;
			laserBulletClone = (GameObject)Instantiate (laserBulletObject, new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z), Quaternion.Euler(0, 0, playerAim.turn_angle));
			bulletDir.z = 0f;
			//bulletDir = bulletDir.normalized;
			laserBulletClone.GetComponent<Rigidbody> ().velocity = bulletDir * 30f;
			isCooling1 = true;
			Invoke ("resetCooling1", 0.1f);
		}

	}

	void shootBeamLaser () {
		if (!isCooling2) {
			Vector3 laserDir = (playerAim.mouse_pos - transform.position).normalized;
			laserBeamClone = (GameObject) Instantiate (laserBeamObject, playerArm.position, Quaternion.identity);
			laserBeamClone.transform.localRotation = Quaternion.Euler ( -1 * playerAim.turn_angle, 90, 0);
			isCooling2 = true;
			Invoke ("resetCooling2", 5);
		}
	}

	void shootBomb(){
		if (!isCooling3) {
			isCooling3 = true;
			bombClone = (GameObject) Instantiate (bombObject, new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z), Quaternion.Euler(0, 0, playerAim.turn_angle));
			Vector3 bombDir = (playerAim.mouse_pos - transform.position).normalized;
			bombClone.GetComponent<Rigidbody> ().velocity = new Vector3 (bombDir.x * 15f, bombDir.y * 15f, 0f);
			bombClone.GetComponent<Rigidbody> ().AddTorque (0f, 0f, -1000f);
			Invoke ("resetCooling3", 15f);
		}
	}

	private void resetCooling1 () {
		isCooling1 = false;
	}

	private void resetCooling2 () {
		isCooling2 = false;
	}

	private void resetCooling3 () {
		isCooling3 = false;
	}

}
