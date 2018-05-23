using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(NoiseGenerator))]

public class NoiseGeneratorEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

		NoiseGenerator script = (NoiseGenerator)target;

        if(GUI.changed)
            script.UpdatePreviewTexture();

        if(GUILayout.Button("Update preview")) {
            script.UpdatePreviewTexture();
        }

        if(GUILayout.Button("Apply noise")) {
            script.ApplyNoise (1);
        }

        if(GUILayout.Button("Subtract noise")) {
            script.ApplyNoise (-1);
        }

        if(GUILayout.Button("Pull vertices")) {
            script.PullVertices();
        }



	}
    
}
