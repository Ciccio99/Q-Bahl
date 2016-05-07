using UnityEngine;
using System.Collections;

public class MovePlatform : MonoBehaviour {


	public float platVel;
	public GameObject movePoint;
	public bool movePlat, stopped, reversed;
	public Rigidbody platBody;


	public void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.CompareTag ("Player") && stopped == false) {
			movePlat = true;
		}
	}
	// Use this for initialization
	void Start () {
			platBody = GetComponent<Rigidbody>();
		    stopped = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x == 16) {
			movePlat = false;
		}
		if (movePlat == true && !stopped && !reversed) {
			platBody.MovePosition (transform.position + new Vector3 (.1f, 0, 0));
			}
		if (movePlat == true && !stopped && reversed) {
			platBody.MovePosition (transform.position + new Vector3 (-.1f, 0, 0));
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("Stahp")) {
			movePlat = false;
			stopped = true;
			reversed = true;
			Invoke ("reset", 2f);
		}

		if (other.gameObject.CompareTag ("Stahp2")) {
			movePlat = false;
			reversed = false;
			//stopped = true;
		}

	}

	public void reset()
	{
		movePlat = true;
		stopped = false;
	}
}
