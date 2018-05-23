using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise.Unity.Generator;
using LibNoise.Unity;

public class Planet : MonoBehaviour {

	[Header("HeightMap")]
	public int seed;
	public QualityMode quality;
	public float heightMultiplier;
	public AnimationCurve heightCurve;
	public UnityEngine.Gradient gradient;
	public float scale;
	[Range(1, 10)]
	public int octaves;
	public float frequency;
	[Range(0f, 1f)]
	public float persistence;
	public float lacunarity;
    public RiggedMultifractal rigged;
    public Perlin perlin;
    public Billow billow;

    public enum NoiseType{
        Ridged, Perlin, Billow, Voronoi
    }

    public void CreateMesh() {
        CreateHeightMap();
        Mesh mesh = MeshGenerator.createMesh(false, 100, 50, new float[100, 100]).createMesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;

    }

	public void CreateHeightMap() {
		// Set up noise
        rigged = new RiggedMultifractal (frequency, lacunarity, octaves, seed, quality);
        perlin = new Perlin (frequency, lacunarity, persistence, octaves, seed, quality);
        billow = new Billow (frequency, lacunarity, persistence, octaves, seed, quality);

		//ModuleBase moduleBase = perlin;

		// Apply noise to the meshes
		foreach(Transform child in transform) {
			MeshFilter meshFilter = child.GetComponent<MeshFilter> ();
			Vector3[] vertices = meshFilter.sharedMesh.vertices;
			Vector3[] normals = meshFilter.sharedMesh.normals;

			Color[] colorMap = new Color[vertices.Length];

			for(int i = 0; i < vertices.Length; i++) {
                float magnitude = (float )perlin.GetValue(vertices[i].x / scale, vertices[i].y / scale, vertices[i].z / scale);
                //float magnitude = (float )rigged.GetValue(vertices[i].x / scale, vertices[i].y / scale, vertices[i].z / scale);
				vertices [i] = Vector3.MoveTowards (vertices [i], (vertices [i] + normals [i]), magnitude * heightMultiplier);
                colorMap [i] = gradient.Evaluate((magnitude + 1) * 0.5f);
			}

			meshFilter.sharedMesh.vertices = vertices;
			meshFilter.sharedMesh.colors = colorMap;
			meshFilter.sharedMesh.RecalculateNormals ();

		}

			
	}

    public void DestroyMesh () {
        foreach(Transform child in transform) {
            GameObject.DestroyImmediate(child.gameObject);

        }
    }

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}
}
