using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaneGenerator))]

public class PlaneGeneratorEditor : Editor{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();

        PlaneGenerator script = (PlaneGenerator)target;

        if(GUI.changed) {
            
        }

        if(GUILayout.Button("Generate Plane")) {
            script.GeneratePlane ();
        }

        if(GUILayout.Button("Destroy Planes")) {
            //foreach(GameObject go in GameObject.FindGameObjectsWithTag("root"))
            //    GameObject.DestroyImmediate(go);    
        }

    }
	
}
