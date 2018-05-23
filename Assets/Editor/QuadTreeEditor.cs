using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuadTree))]

public class QuadTreeEditor : Editor{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();

        QuadTree script = (QuadTree)target;

        if(GUI.changed) {
            
        }


        if(GUILayout.Button("Create Planet")) {
            script.CreatePlanet ();
        }

        if(GUILayout.Button("Destroy Planets")) {
            foreach(Transform t in script.transform)
                GameObject.DestroyImmediate(t.gameObject);
        }

        if(GUILayout.Button("Split nodes")) {
            script.SplitNodes ();
        }

        if(GUILayout.Button("Generate Paths")) {
            script.GeneratePaths();
        }

    }
	
}
