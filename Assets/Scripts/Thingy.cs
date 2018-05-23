using UnityEngine;
using System.Collections;
using LibNoise.Unity.Generator;

public class Thingy : MonoBehaviour {

	public float frequency;
	public float lacunarity;
	public float persistence;
	public int octaves;
	public int seed;
	public LibNoise.Unity.QualityMode quality;
	[Space(10)]
	public Gradient gradient;
	public int spaceSizeX;
	public int spaceSizeY;
	public int spaceSizeZ;
	public int stepSize;
	public float min;
	public float max;
	public float scaleX;
	public float scaleY;
	public float scaleZ;

	// Range of perlin.Getvalue is about -1.5f and 1.5
	public void Generate() {
		Perlin perlin = new Perlin (frequency, lacunarity, persistence, octaves, seed, quality);

		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cube.tag = "cube";

		for(int y = 0; y < spaceSizeY; y+=stepSize) {
			for(int x = 0; x < spaceSizeX; x+=stepSize) {
				for(int z = 0; z < spaceSizeZ; z+=stepSize) {
					float magnitude = (float )perlin.GetValue ((double)x, (double)y, (double)z);

					if(magnitude > min && magnitude < max) {
						GameObject cubeCopy = (GameObject )GameObject.Instantiate (cube, new Vector3((float ) x, (float ) y, (float ) z), Quaternion.identity);
						cubeCopy.transform.localScale = new Vector3 (scaleX, scaleY, scaleZ);

						//Color col = gradient.Evaluate ((float )y / 10f);
						//cubeCopy.GetComponent<MeshRenderer>().material.color = col;

						cubeCopy.tag = "cube";
					
						//MeshRenderer rend = cube.GetComponent<MeshRenderer> ();
						//rend.sharedMaterial.color = new Vector4 (Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);

					}

				}
			}
		}


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
