using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour {
    public int xVertices, yVertices, xPatchCount, yPatchCount;
    public float radius;
    public Transform target;
    public float distance;

    private int maxDepth = 0;
    private List<QuadTreeNode> nodes = new List<QuadTreeNode>();
    private PlanetMesh meshGenerator;

    public void Start() {
        
    }

    public void Update() {
        // Try to split nodes
        SplitNodes();

    }

    public void CreatePlanet() {
        // script for mesh generation
        meshGenerator = GetComponent<PlanetMesh>();

        // Setup parameters for our planet patches
        meshGenerator.uPatchCount = xPatchCount;
        meshGenerator.vPatchCount = yPatchCount;
        meshGenerator.radius = radius;
        meshGenerator.xVertCount = xVertices;
        meshGenerator.yVertCount = yVertices;

        // Build one patch for each cardinal direction using meshGenerator
        for(int i = 0; i < 6; i++) {
            QuadTreeNode root = new QuadTreeNode(i, meshGenerator);

            // Parent all meshpatches under one GO
            root.meshPatch.transform.parent = this.gameObject.transform;

            // Add node to our list of nodes
            nodes.Add(root);
        }

        this.maxDepth = 0;
    }

    public void SplitNodes() {
        // Find closest node
        QuadTreeNode closest = null;
        if(nodes.Count > 0)
            closest = nodes.ToArray()[0];

        foreach(QuadTreeNode node in nodes.ToArray()) {
            // Distance to camera
            float distanceToCamera = Vector3.Magnitude(Camera.main.transform.position - node.position);

            // Criteria for splitting node: MAX_DEPTH is here
            if(distanceToCamera * (node.depth + 1) < distance && node.depth < 2 && node.meshPatch != null && node.meshPatch.activeInHierarchy)
                SplitNode(node);

            if(distanceToCamera < Vector3.Magnitude(closest.position - Camera.main.transform.position))
                closest = node;
        }

        //Debug.Log("Closest node is: " + closest.direction + "_" + closest.depth + "_" + closest.childNumber);
    }

    // leaf becomes parent QuadTreeNode that lacks meshPatch
    public void SplitNode(QuadTreeNode leaf) {
        // Create root GO and parent correctly
        GameObject newRoot = new GameObject();
        newRoot.tag = "root";
        newRoot.name = "Root_" + PlanetMesh.patches[leaf.direction].name + "_" + leaf.u + "_" + leaf.v;
        newRoot.transform.parent = leaf.meshPatch.transform.parent;

        // increase patchcount based on depth
        meshGenerator.uPatchCount = (int ) Mathf.Pow(2.0f, (float )leaf.depth + 1); // LOOK AT THIS
        meshGenerator.vPatchCount = meshGenerator.uPatchCount;
        meshGenerator.xVertCount = (int ) (((float )meshGenerator.xVertCount));
        meshGenerator.yVertCount = (int ) (((float )meshGenerator.yVertCount));

        // Create child nodes and mesh patches
        int i = 0;
        int offset = meshGenerator.uPatchCount / (leaf.depth + 1);
        for(int u = 0; u < 2; u++) {
            for(int v = 0; v < 2; v++) {
                // Create node
                leaf.children[i] = new QuadTreeNode(i, leaf, meshGenerator);

                // Create appropriate mesh based on the splitted nodes childnumber
                switch(leaf.childNumber) {
                    case 0:
                        leaf.children[i].meshPatch = meshGenerator.GeneratePatch(leaf.direction, u, v);
                        break;
                    case 1:
                        leaf.children[i].meshPatch = meshGenerator.GeneratePatch(leaf.direction, u, v + offset);
                        break;
                    case 2:
                        leaf.children[i].meshPatch = meshGenerator.GeneratePatch(leaf.direction, u + offset, v);
                        break;
                    case 3:
                        leaf.children[i].meshPatch = meshGenerator.GeneratePatch(leaf.direction, u + offset, v + offset);
                        break;
                }

                // Setup child array of new node and tag GO
                leaf.children[i].meshPatch.transform.parent = newRoot.transform;
                leaf.children[i].meshPatch.tag = "patch";

                // Set position of node
                Mesh mesh = leaf.children[i].meshPatch.GetComponent<MeshFilter>().sharedMesh;
                Vector3 worldPos = leaf.children[i].meshPatch.transform.TransformPoint(mesh.vertices[meshGenerator.yVertCount * (meshGenerator.yVertCount / 2) + meshGenerator.xVertCount / 2]);
                leaf.children[i].position = worldPos;

                // Add to flat node list
                nodes.Add(leaf.children[i]);
                i++;
            }
        }

        // Disable the splitted node's meshPatch GO 
        leaf.meshPatch.SetActive(false);
        //GameObject.DestroyImmediate(leaf.meshPatch);

        this.maxDepth++;

    }

    public void GeneratePaths() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("patch")) {
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;

            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            int length = mesh.triangles.Length;

            for (int i = 0; i < length; i += 3) {
                Vector3 surfaceNormal = (vertices[triangles[i + 1]] - vertices[triangles[i]]) - (vertices[triangles[i + 2]] - vertices[triangles[i]]);
                Vector3 dir = Vector3.Normalize(vertices[triangles[i]] - Vector3.zero);
                float a = Vector3.Dot(surfaceNormal, dir);

                if (a < 5f && a > -5f) {
                    vertices[triangles[i]] -= Vector3.Normalize(surfaceNormal) * 20;
                    vertices[triangles[i + 1]] -= Vector3.Normalize(surfaceNormal) * 20;
                    vertices[triangles[i + 2]] -= Vector3.Normalize(surfaceNormal) * 20;
                }
            }

            //go.GetComponent<MeshRenderer>().sharedMaterial.SetVectorArray("_RoadVertices", )

            // assign new arrays
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

        }
    }

}
