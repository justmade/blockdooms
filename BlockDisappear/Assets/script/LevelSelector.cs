using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using LitJson;
using System.IO;
using LFormat;

public class LevelSelector : MonoBehaviour {

	private List<string> files;

	// Use this for initialization
	void Start () {
		GetAllLevel ();
	}

	void GetAllLevel(){
		files = new List<string> ();
		foreach (var path in Directory.GetFiles(Application.dataPath+"/levels/")) {
			if (System.IO.Path.GetExtension (path) == ".txt") {
				string name =  (System.IO.Path.GetFileName(path));
				Debug.LogFormat ("file {0}", name);
				files.Add (name);
			}
		}
	}

	void OnGUI () 
	{


		//EditorGUILayout.VerticalScope;
		string[] floorNames = new string[files.Count];
		for (int i = 0; i < files.Count; i++) {
			floorNames [i] = (files[i]).ToString ();;
		}

		GUILayout.SelectionGrid (1, floorNames, 3);
	}


	// Update is called once per frame
	void Update () {
		
	}
}
