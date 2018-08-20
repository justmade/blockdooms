using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using System.IO;
using LFormat;

public class LevelSelector : MonoBehaviour {

	private List<string> files;

	private float stageW;
	private float stageH;

	private int selectIndex;

	private int lastSelectIndex;

	public GameManager gm;


	// Use this for initialization
	void Start () {
		selectIndex = -1;
		lastSelectIndex = -1;
		float leftBorder;
		float rightBorder;
		float topBorder;
		float downBorder;

		//the up right corner
		Vector3 cornerPos=Camera.main.ViewportToWorldPoint(new Vector3(1f,1f,Mathf.Abs(Camera.main.transform.position.z)));

		leftBorder=Camera.main.transform.position.x-(cornerPos.x-Camera.main.transform.position.x);
		rightBorder=cornerPos.x;
		topBorder=cornerPos.y;
		downBorder=Camera.main.transform.position.y-(cornerPos.y-Camera.main.transform.position.y);

		stageW=rightBorder-leftBorder;
		stageH=topBorder-downBorder;

		Debug.LogFormat ("stageW {0},{1}", stageW, stageH);


		GetAllLevel ();
	}

	void GetAllLevel(){
		files = new List<string> ();
		foreach (var path in Directory.GetFiles("./levels/")) {
			if (System.IO.Path.GetExtension (path) == ".txt") {
				string name =  (System.IO.Path.GetFileName(path));
				Debug.LogFormat ("file {0}", name);
				files.Add (name);
			}
		}
	}

	void OnGUI () 
	{
		GUI.BeginGroup(new Rect(Screen.width/2 - 600/2,150,600,600));
		string[] floorNames = new string[files.Count];
		for (int i = 0; i < files.Count; i++) {
			floorNames [i] = (files[i]).ToString ();
		}
		selectIndex = GUILayout.SelectionGrid (selectIndex, floorNames, 3,GUILayout.Width(600),GUILayout.Height(200));
		GUI.EndGroup();
	}


	// Update is called once per frame
	void Update () {
		if (lastSelectIndex != selectIndex) {
			lastSelectIndex = selectIndex;
			Debug.Log ("select");
			gm.SendMessageUpwards("GameStart", (files[lastSelectIndex]).ToString(), SendMessageOptions.DontRequireReceiver);
		}
	}
}
