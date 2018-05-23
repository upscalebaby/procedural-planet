using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetMesh))]

public class PlanetMeshEditor : Editor{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();

        PlanetMesh script = (PlanetMesh)target;

        if(GUI.changed) {
            
        }

        if(GUILayout.Button("Create Planet")) {
            script.GeneratePatches (script.uPatchCount, script.vPatchCount);
        }

    }
	
}
