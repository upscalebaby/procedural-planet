using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;
    public float speed;
	public float jumpForce;
    private bool spaceDown;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        float moveX= Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveX, 0, moveZ);
		rb.AddForce(movement * speed);

		if (Input.GetKeyDown ("space")) {
			Vector3 jump = new Vector3(0, Input.GetAxis("Jump"), 0);
			rb.AddForce (jump * jumpForce);
		}
        


       
    }

    
}
