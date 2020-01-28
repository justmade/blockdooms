using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaneUI))] 
public class SenceUI : Editor {


	void OnSceneGUI() 
	{
		//得到test脚本的对象
		PlaneUI test = (PlaneUI) target;

		//绘制文本框
//		GUILayout.Label("我在编辑Scene视图");	
		//开始绘制GUI
		Handles.BeginGUI();

		//规定GUI显示区域
		GUILayout.BeginArea(new Rect(100, 100, 200, 100));

//		//GUI绘制一个按钮
//		if(GUILayout.Button("这是一个按钮!"))
//		{
//			Debug.Log("test");		
//		}
		//GUI绘制文本框
		GUILayout.Label("红色:1,绿:2,蓝:3");	

		GUILayout.EndArea();

		Handles.EndGUI();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
