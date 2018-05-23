using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public GameObject player;
	public Vector3 offset;
	public float rotationAmount;

	private bool qDown;
	private bool eDown;

	private Vector3 offsetX;
	private Vector3 offsetY;

	// Use this for initialization
	void Start () {
		offsetX = new Vector3 (0, 1, 2);
		offsetY = new Vector3 (0, 0, 2);
	}
	
	// Update is called once per frame
	void Update () {




	}

	void LateUpdate () {
		offsetX = Quaternion.AngleAxis (Input.GetAxis("RotRight") * rotationAmount, Vector3.up) * offsetX;
		offsetY = Quaternion.AngleAxis (Input.GetAxis("RotLeft") * rotationAmount, Vector3.up) * offsetY;

		transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 10f, player.transform.position.z);
		transform.LookAt (player.transform.position);
	}
}