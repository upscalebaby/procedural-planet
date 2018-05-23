using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour {
    public int uPatchCount;
    public int vPatchCount;
    public int xVertCount;
    public int yVertCount;
    public float edgeLength;
    public bool flatShading;

    public void GeneratePlane() {
        GetComponent<MeshFilter>().sharedMesh = MeshGenerator.createMesh(flatShading, xVertCount, edgeLength).createMesh();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
