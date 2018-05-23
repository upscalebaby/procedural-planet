using UnityEngine;
using System.Collections;

public struct PatchConfig
{
	public string name;
	public Vector3 uAxis;
	public Vector3 vAxis;
	public Vector3 height;

	public PatchConfig(string aName, Vector3 aUAxis, Vector3 aVAxis)
	{
		name = aName;
		uAxis = aUAxis;
		vAxis = aVAxis;
		height = Vector3.Cross(vAxis, uAxis);
	}
}

public class PlanetMesh : MonoBehaviour
{
    public int uPatchCount;
    public int vPatchCount;
    public int xVertCount;
    public int yVertCount;
    public float radius;
    public bool flatShading;

	public static PatchConfig[] patches = new PatchConfig[]
	{
		new PatchConfig("top", Vector3.right, Vector3.forward),
		new PatchConfig("bottom", Vector3.left, Vector3.forward),
		new PatchConfig("left", Vector3.up, Vector3.forward),
		new PatchConfig("right", Vector3.down, Vector3.forward),
		new PatchConfig("front", Vector3.right, Vector3.down),
		new PatchConfig("back", Vector3.right, Vector3.up)
	};

	public Material patchMaterial;

    // Lazy abstraction
    public GameObject GeneratePatch(int direction, int u, int v) {
        GameObject patch = GeneratePatch(patches[direction], u, v);
        return patch;
    }

	public GameObject GeneratePatch(PatchConfig aConf, int u, int v) {
        // Create new GO and name it
		GameObject patch = new GameObject("Patch_" + aConf.name + "_" + u + "_" + v);

        // Setup mesh-filter/renderer/material for GO
		MeshFilter mf = patch.AddComponent<MeshFilter>();
		MeshRenderer rend = patch.AddComponent<MeshRenderer>();
		rend.sharedMaterial = patchMaterial;
		Mesh m = mf.sharedMesh = new Mesh();

        // Parent GO and setup position and rotation
		patch.transform.parent = transform;
		patch.transform.localEulerAngles = Vector3.zero;
		patch.transform.localPosition = Vector3.zero;

        // Setup mesh info
		Vector2 UVstep = new Vector2(1f / uPatchCount, 1f / vPatchCount);
		Vector2 step = new Vector2(UVstep.x / (xVertCount-1), UVstep.y / (yVertCount-1));
		Vector2 offset = new Vector3((-0.5f + u * UVstep.x), (-0.5f + v * UVstep.y));
		Vector3[] vertices = new Vector3[xVertCount * yVertCount];
		Vector3[] normals = new Vector3[vertices.Length];
		Vector2[] uvs = new Vector2[vertices.Length];

        // Create meshdata and populate it with vertices, uvs and normals
        MeshData meshdata = new MeshData(flatShading, yVertCount);
        int vertexIndex = 0;

		for (int y = 0; y < yVertCount; y++)
		{
			for (int x = 0; x < xVertCount; x++)
			{
                // Define vertex position and normal
				Vector2 p = offset + new Vector2(x * step.x, y * step.y);
				uvs[vertexIndex] = p + Vector2.one*0.5f;
				Vector3 vec = aConf.uAxis * p.x + aConf.vAxis * p.y + aConf.height*0.5f;
				vec = vec.normalized;

				normals[vertexIndex] = vec;
                vertices[vertexIndex] = vec * radius;

                // Define triangles
                if(x < xVertCount-1 && y < yVertCount-1) {   // guard to ignore right bottom edge and far right edge
                    meshdata.AddTriangle(vertexIndex + xVertCount, vertexIndex + xVertCount + 1, vertexIndex);
                    meshdata.AddTriangle(vertexIndex + 1, vertexIndex, vertexIndex + xVertCount + 1);
                }

                vertexIndex++;
			}
		}

        // Assign meshdata to Mesh
		m.vertices = vertices;
		m.normals = normals;
		m.uv = uvs;
        m.triangles = meshdata.triangles;
        m.RecalculateNormals();
		m.RecalculateBounds();

        return patch;
	}

    public void GeneratePatches(int uPatchCount, int vPatchCount)
    {
        // Create planet GO and parent it to the current GO
        GameObject planet = new GameObject("Patches");
        planet.tag = "waterpatches";
        planet.transform.parent = transform;

        // Create meshes and colliders - child them to the parent go
        for(int i = 0; i < 6; i++)  {
            for(int u = 0; u < uPatchCount; u++)    {
                for(int v = 0; v < vPatchCount; v++)    {
                    GameObject patch = GeneratePatch(patches[i], u, v);
                    //patch.SetActive(false);
                    patch.transform.parent = planet.transform;
                    patch.tag = "waterpatch";
                    MeshCollider meshCollider = patch.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = patch.GetComponent<MeshFilter>().sharedMesh;
                }
            }
        }

    }

    public class MeshData {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        public bool flatShading;

        int triangleIndex;

        public MeshData(bool flatShading, int size) {
            this.flatShading = flatShading;
            int nrOfVertices = size * size;
            int nrOfTriangles = (size - 1) * (size - 1) * 6;

            vertices = new Vector3[nrOfVertices];
            uvs = new Vector2[size * size];
            triangles = new int[nrOfTriangles];
        }

        public void AddTriangle(int a, int b, int c) {
            triangles [triangleIndex] = a;
            triangles [triangleIndex + 1] = b;
            triangles [triangleIndex + 2] = c;

            triangleIndex += 3;
        }

        public Mesh createMesh() {
            Mesh mesh = new Mesh ();
            if (flatShading)
                FlatShading();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals ();
            return mesh;
        }

        void FlatShading() {
            Vector3[] flatShadedVertices = new Vector3[triangles.Length];
            Vector2[] flatShadedUVs = new Vector2[triangles.Length];

            for(int i = 0; i < triangles.Length; i++) {
                flatShadedVertices[i] = vertices[triangles[i]];
                flatShadedUVs[i] = uvs[triangles[i]];
                triangles[i] = i;
            }

            vertices = flatShadedVertices;
            uvs = flatShadedUVs;
        }

    }

}
