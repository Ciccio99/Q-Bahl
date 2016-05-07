using UnityEngine;
using System.Collections;

public class BadDoodSwipeColScript : MonoBehaviour {

	bool hit;
	MasterHandScript.BossState currState;

	void Start () {
		hit = false;
		currState = transform.parent.GetComponent<MasterHandScript> ().getCurrState ();
	}

	void OnTriggerEnter (Collider col) {
		currState = transform.parent.GetComponent<MasterHandScript> ().getCurrState ();
		if (col.gameObject.CompareTag ("Player") && currState == MasterHandScript.BossState.SWIPE) {
			Debug.Log (col);
			col.gameObject.GetComponent<PlayerHealth> ().TakeDamage (15);
			if (col.gameObject.GetComponent<Rigidbody> () != null) {
				col.gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (0f, 100f * GetComponent<MasterHandScript> ().moveMult, 0));
			}
			hit = true;
			Invoke ("resetHit", 5);
		}
	}

	void resetHit () {
		hit = false;
	}
}
