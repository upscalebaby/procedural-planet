using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CubeSphere))]

public class CubeSphereEditor : Editor {
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

		CubeSphere script = (CubeSphere)target;

		if(GUI.changed) {
			script.Generate ();
		}


		if(GUILayout.Button("Generate Sphere")) {
			script.Generate ();
		}

	}



}
