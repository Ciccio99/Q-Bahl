using UnityEngine;
using System.Collections;

public class TeleportPlayer : MonoBehaviour {

	public GameObject player, teleportPoint;
	public Camera myCamera;
	public Vector3 teleportPos, playerPos, cameraPos;


	// Use this for initialization
	void Start () {
		teleportPos = teleportPoint.transform.position;
		playerPos = player.transform.position;
		cameraPos = myCamera.transform.position;
	}
	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player")) {
			other.transform.position = teleportPos;
			myCamera.transform.position = teleportPos;
		}

	}
}
