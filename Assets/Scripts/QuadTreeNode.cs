using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeNode {
    public QuadTreeNode parent;
    public QuadTreeNode[] children = new QuadTreeNode[4];
    public int depth;
    public int direction;
    public GameObject meshPatch;
    public Vector3 position;
    public int u, v;
    public int childNumber;

    // Root node constructor //
    public QuadTreeNode(int direction, PlanetMesh meshGenerator) {
        this.depth = 0;
        this.direction = direction;
        this.u = 0;
        this.v = 0;

        // Crate patch, name it and parent it to root
        this.meshPatch = CreatePatch(meshGenerator);
        this.meshPatch.name = "Patch_" + PlanetMesh.patches[direction].name + "_" + u + "_" + v;
        this.meshPatch.tag = "patch";

    }

    // Node Splitting Constructor - doesn't create meshPatch GO, just updates the model //
	public QuadTreeNode(int childNr, QuadTreeNode parent, PlanetMesh meshGenerator) {
        this.parent = parent;
        this.direction = parent.direction;
        this.depth = parent.depth + 1;
        this.childNumber = childNr;
    }

    private GameObject CreatePatch(PlanetMesh meshGenerator) {
        // Generate mesh with u,v from class state
        GameObject patch = meshGenerator.GeneratePatch(this.direction, this.u, this.v);

        // Set position of node
        Mesh mesh = patch.GetComponent<MeshFilter>().sharedMesh;
        Vector3 worldPos = patch.transform.TransformPoint(mesh.vertices[meshGenerator.yVertCount * (meshGenerator.yVertCount / 2) + meshGenerator.xVertCount / 2]);
        this.position = worldPos;

        return patch;
    }

}
