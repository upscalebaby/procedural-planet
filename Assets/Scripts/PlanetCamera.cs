using UnityEngine;
using System.Collections;

public class PlanetCamera : MonoBehaviour {

    RenderTexture depthRT;
    public float cameraSpeed, scrollSpeed;

    public Transform planet;
    private bool rightClicked, leftClicked;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 input = new Vector3 (Input.GetAxis("Vertical"), Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Mouse ScrollWheel"));

		if (Input.GetMouseButtonDown (0))
			leftClicked = true;
		if (Input.GetMouseButtonUp (0))
            leftClicked = false;
		if(leftClicked)
			transform.RotateAround (Vector3.zero, Vector3.up , cameraSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown (1))
            rightClicked = true;
        if (Input.GetMouseButtonUp (1))
            rightClicked = false;
        if(rightClicked)
            transform.RotateAround (Vector3.zero, Vector3.right, cameraSpeed * Time.deltaTime);

		transform.Translate (0, 0, input.z * Time.deltaTime * scrollSpeed);

	}

    private void OnPreRender()
    {
        Shader.SetGlobalVector("_CamPos", this.transform.position);
        Shader.SetGlobalVector("_CamRight", this.transform.right);
        Shader.SetGlobalVector("_CamUp", this.transform.up);
        Shader.SetGlobalVector("_CamForward", this.transform.forward);
    }
}
