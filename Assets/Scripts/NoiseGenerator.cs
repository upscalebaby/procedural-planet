using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoiseGenerator : MonoBehaviour {

	[Header("HeightMap")]
	public int seed;
	public float heightMultiplier;
	public AnimationCurve heightCurve;
	public UnityEngine.Gradient gradient;
	public float gain;
	[Range(1, 10)]
	public int octaves;
    [Range(0.00001f, 10f)]
	public float frequency;
    public float warpAmp;
	public float lacunarity;
    public FastNoise.NoiseType noiseType;
    public FastNoise.FractalType fractalType;
    public FastNoise.CellularDistanceFunction distFunction;
    public FastNoise.CellularReturnType cellularReturnType;
    public Queue<float> amount;
    public Queue<Vector3> direction;
    public Texture2D texture;
    public GameObject preview;

    public float maxAltitude = float.MinValue;
    public float minAltitude = float.MaxValue;

    private FastNoise noiseGenerator;

    public void UpdatePreviewTexture() {
        // Set up noise
        FastNoise noise = new FastNoise();
        noise.SetNoiseType(noiseType);
        noise.SetFractalType(fractalType);
        noise.SetCellularDistanceFunction(distFunction);
        noise.SetCellularReturnType(cellularReturnType);
        noise.SetPositionWarpAmp(warpAmp);

        noise.SetFrequency(frequency);
        noise.SetFractalGain(gain);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalOctaves(octaves); 

        for(int y = 0; y < 512; y++) {
            for(int x = 0; x < 512; x++) {
                float xf = (float ) x;
                float yf = (float ) y;
                noise.PositionWarp(ref xf, ref yf);
                //texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, noise.GetNoise(xf, yf)));
            }
        }

        //texture.Apply();

        //preview.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
    }

    public void ApplyNoiseToChild(GameObject patch) {
        MeshFilter meshFilter = patch.GetComponent<MeshFilter>();
        Vector3[] vertices = meshFilter.sharedMesh.vertices;
        Vector3[] normals = meshFilter.sharedMesh.normals;

        Color[] colorMap = new Color[vertices.Length];

        noiseGenerator.SetFrequency(frequency);
        noiseGenerator.SetFractalGain(gain);
        noiseGenerator.SetNoiseType(noiseType);
        noiseGenerator.SetPositionWarpAmp(warpAmp);
        noiseGenerator.SetFractalLacunarity(lacunarity);
        noiseGenerator.SetFractalOctaves(octaves);
        noiseGenerator.SetNoiseType(noiseType);
        noiseGenerator.SetFractalType(fractalType);
        noiseGenerator.SetCellularDistanceFunction(distFunction);
        noiseGenerator.SetCellularReturnType(cellularReturnType);

        // Sample noise function
        for (int i = 0; i < vertices.Length; i++) {
            //int heightOffset = 1;
            for(int j = 0; j < vertices.Length; j++) {
                float magnitude = noiseGenerator.GetSimplexFractal(vertices[i].x, vertices[i].y, vertices[i].z);
                vertices[i] = Vector3.MoveTowards(vertices[i], normals[i].normalized, magnitude * heightMultiplier);
                colorMap[i] = gradient.Evaluate((magnitude + 1) * 0.5f);
            }
        }

        meshFilter.sharedMesh.vertices = vertices;
        meshFilter.sharedMesh.colors = colorMap;
        meshFilter.sharedMesh.RecalculateNormals();
    }
   
	public void ApplyNoise(int dir)
    {
        // Set up noise
        FastNoise noise = new FastNoise();
        noise.SetFrequency(frequency);
        noise.SetFractalGain(gain);
        noise.SetNoiseType(noiseType);
        noise.SetPositionWarpAmp(warpAmp);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalOctaves(octaves);
        noise.SetNoiseType(noiseType);
        noise.SetFractalType(fractalType);
        noise.SetCellularDistanceFunction(distFunction);
        noise.SetCellularReturnType(cellularReturnType);

        GameObject[] patches = GameObject.FindGameObjectsWithTag("patch");

        // Apply noise to the meshes
        foreach(GameObject p in patches) {
            MeshFilter meshFilter = p.GetComponent<MeshFilter>();
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            Vector3[] normals = meshFilter.sharedMesh.normals;

            Color[] colorMap = new Color[vertices.Length];

            // Sample noise function
            for (int i = 0; i < vertices.Length; i++) {
                float magnitude = noise.GetSimplex(vertices[i].x, vertices[i].y, vertices[i].z);
                //float magnitude = (float )rigged.GetValue(vertices[i].x / scale, vertices[i].y / scale, vertices[i].z / scale);
                vertices[i] = Vector3.MoveTowards(vertices[i], vertices [i] + normals [i] * 999999f, magnitude * heightMultiplier * dir);
                colorMap[i] = gradient.Evaluate((magnitude + 1) * 0.5f);

                if(Vector3.Magnitude(vertices[i]) > maxAltitude) {
                    maxAltitude =  Vector3.Magnitude(vertices[i]);
                }

                if(Vector3.Magnitude(vertices[i]) < minAltitude) {
                    minAltitude =  Vector3.Magnitude(vertices[i]);
                }
            }

            // Set shader properties
            gameObject.GetComponent<PlanetMesh>().patchMaterial.SetFloat("_MaxAltitude", maxAltitude);
            gameObject.GetComponent<PlanetMesh>().patchMaterial.SetFloat("_MinAltitude", maxAltitude);

            meshFilter.sharedMesh.vertices = vertices;
            meshFilter.sharedMesh.colors = colorMap;
            meshFilter.sharedMesh.RecalculateNormals();
        }

	}

    public void PullVertices() {
        GameObject[] patches = GameObject.FindGameObjectsWithTag("patch");

        foreach(GameObject p in patches) {
            MeshFilter meshFilter = p.GetComponent<MeshFilter>();
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            Vector3[] normals = meshFilter.sharedMesh.normals;

            // Pull vertices
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = Vector3.MoveTowards(vertices[i], normals[i] * -1, heightMultiplier);
            }

            meshFilter.sharedMesh.vertices = vertices;
            meshFilter.sharedMesh.RecalculateNormals();
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
