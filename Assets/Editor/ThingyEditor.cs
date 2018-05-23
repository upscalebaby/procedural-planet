using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Thingy))]

public class ThingyEditor : Editor {
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

		Thingy script = (Thingy)target;

		if(GUI.changed) {
			//DeleteCubes ();
			//script.Generate ();
		}

		if(GUILayout.Button("Generate")) {
			DeleteCubes ();
			script.Generate ();
		}

		if(GUILayout.Button("Delete")) {
			DeleteCubes ();
				
		}
			
	}

	private void DeleteCubes() {
		GameObject[] cubeArray = GameObject.FindGameObjectsWithTag("cube");

		foreach(GameObject g in cubeArray) {
			DestroyImmediate (g);
		}
	}



}
