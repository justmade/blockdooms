using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditor : EditorWindow {

	[MenuItem ("Window/LevelEditor")]
	static void AddWindow ()
	{       
		//创建窗口
		Rect  wr = new Rect (0,0,500,500);
		LevelEditor window = (LevelEditor)EditorWindow.GetWindowWithRect (typeof (LevelEditor),wr,true,"LevelEditor ");	
		window.Show();

	}

	//输入文字的内容
	private string text;
	//选择贴图的对象
	private Texture texture;

	private Vector2Int mapSize;

	private Vector2Int lastMapSize;

	private int toolbarInt;

	public int selGridInt = 0;

	private int floor = 1; 

	public string[] selStrings = new string[] {"radio1", "radio2","radio3","radio4", "radio5", "radio6"};

	private bool isUpdate = false;

	private string[,] blockGrids;

	private string[,] lastGrids;

	private int totalGrids;

	private int currentFloor;

	public void Awake () 
	{
		//在资源中读取一张贴图
		texture = Resources.Load("1") as Texture;

		mapSize = new Vector2Int (2, 2);

		lastMapSize = new Vector2Int (2, 2);

		totalGrids = mapSize.x * mapSize.y;

		floor = 1;
		currentFloor = 0;

		blockGrids = new string[1, totalGrids];

		lastGrids = new string[1, totalGrids];

		for (int k = 0; k < totalGrids; k++) {
			blockGrids[currentFloor,k] = "1";
			lastGrids [currentFloor,k] = "1";
		}

		//mapSize = EditorGUILayout.Vector2IntField("Map Size:", mapSize);
	}



	//绘制窗口时调用
	void OnGUI () 
	{
		mapSize = EditorGUILayout.Vector2IntField("Map Size:", mapSize);
	
		floor = EditorGUILayout.IntField ("层数", floor);

		if (floor > 0) {
			string[] floorNames = new string[floor];
			for (int i = 0; i < floor; i++) {
				floorNames [i] = (i+1).ToString ();;
			}

			GUILayout.BeginVertical("Box");

			selGridInt = GUILayout.SelectionGrid(selGridInt, floorNames, 10);
			GUILayout.EndVertical ();
		}

		//更新网格数据
		if (isUpdate && lastMapSize != mapSize) {
			totalGrids = mapSize.x * mapSize.y;
			isUpdate = false;
			blockGrids = new string[floor,totalGrids];

			int lastIndex = 0;

			if (mapSize.x >= lastMapSize.x) {
				for (int k = 0; k < totalGrids; k++) {
					if (lastIndex >= lastMapSize.x * lastMapSize.y) {
						blockGrids [currentFloor, k] = "-1";
					}

					else if (k % (mapSize.x) >= (lastMapSize.x)) {
						blockGrids [currentFloor, k] = "-1";
					} 
					else {
						blockGrids [currentFloor, k] = lastGrids[currentFloor, lastIndex];
						lastIndex++;
					}
				}
			}else if(mapSize.x < lastMapSize.x){
				for (int k = 0; k < totalGrids; k++) {
					if (lastIndex >= lastMapSize.x * lastMapSize.y) {
						blockGrids [currentFloor, k] = "-1";
					}
					blockGrids [currentFloor, k] = lastGrids[currentFloor, lastIndex];
					lastIndex++;
					if ((k+1) % (mapSize.x) == 0) {
						lastIndex += (lastMapSize.x - mapSize.x);
					} 
				}
			}

			lastGrids = blockGrids;
			lastMapSize = mapSize;
		}
			
		for(int i=0;i<totalGrids;i++){
			if (i %  lastMapSize.x == 0) {
				EditorGUILayout.BeginHorizontal (GUILayout.Height (30));
			}
			lastGrids[currentFloor,i] = EditorGUILayout.TextField("",lastGrids[currentFloor,i], GUILayout.Width (40),  GUILayout.Height (30));
			if ((i+1) %  lastMapSize.x == 0) {
				EditorGUILayout.EndHorizontal ();
			}
		}
			
		if (GUILayout.Button ("Apply", GUILayout.Width (200))) {
		
			updateSetting ();
		}

//		if(GUILayout.Button("打开通知",GUILayout.Width(200)))
//		{
//			//打开一个通知栏
//			this.ShowNotification(new GUIContent("This is a Notification"));
//		}
//
//		if(GUILayout.Button("关闭通知",GUILayout.Width(200)))
//		{
//			//关闭通知栏
//			this.RemoveNotification();
//		}
//
//		//文本框显示鼠标在窗口的位置
//		EditorGUILayout.LabelField ("鼠标在窗口的位置", Event.current.mousePosition.ToString ());
//
//		//选择贴图
//		texture =  EditorGUILayout.ObjectField("添加贴图",texture,typeof(Texture),true) as Texture;
//
//		if(GUILayout.Button("关闭窗口",GUILayout.Width(200)))
//		{
//			//关闭窗口
//			this.Close();
//		}

	}

	void updateSetting(){
		isUpdate = true;
	}

	//更新
	void Update()
	{

	}

	void OnFocus()
	{
		Debug.Log("当窗口获得焦点时调用一次");
	}

	void OnLostFocus()
	{
		Debug.Log("当窗口丢失焦点时调用一次");
	}

	void OnHierarchyChange()
	{
		Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
	}

	void OnProjectChange()
	{
		Debug.Log("当Project视图中的资源发生改变时调用一次");
	}

	void OnInspectorUpdate()
	{
		//Debug.Log("窗口面板的更新");
		//这里开启窗口的重绘，不然窗口信息不会刷新
		this.Repaint();
	}

	void OnSelectionChange()
	{
		//当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
		foreach(Transform t in Selection.transforms)
		{
			//有可能是多选，这里开启一个循环打印选中游戏对象的名称
			Debug.Log("OnSelectionChange" + t.name);
		}
	}

	void OnDestroy()
	{
		Debug.Log("当窗口关闭时调用");
	}
}
