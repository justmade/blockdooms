using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using LitJson;
using System.IO;
using LFormat;
public class LevelEditor : EditorWindow {




	[MenuItem ("Window/LevelEditor")]
	static void AddWindow ()
	{       
		//创建窗口
		Rect  wr = new Rect (0,0,500,800);
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

	private GameObject[,] allBlock;

	private string[,] blockGrids;

	private string[,] lastGrids;

	private string[] moveGrids;

	private string[] lastMoveGrids;

	private int totalGrids;

	private int selectedFloor;

	private int currentFloor;

	private int totalFloors;

	private GameObject blockContainer;

	private string fileName = "";

	private List<string> files;

	private TextAsset txtAsset;

	public void Awake () 
	{
		//在资源中读取一张贴图
		texture = Resources.Load("1") as Texture;

		mapSize = new Vector2Int (2, 2);

		lastMapSize = new Vector2Int (2, 2);

		totalGrids = mapSize.x * mapSize.y;

		txtAsset = new TextAsset ();

		floor = 10;
		selectedFloor = 0;
		currentFloor = 0;
		totalFloors = 10;

		blockGrids = new string[totalFloors, totalGrids];
		allBlock = new GameObject[totalFloors, totalGrids];

		moveGrids = new string[totalGrids];
		lastMoveGrids = new string[totalGrids];

		lastGrids = new string[totalFloors, totalGrids];

		for (int f = 0; f < totalFloors; f++) {
			for (int k = 0; k < totalGrids; k++) {
				blockGrids[f,k] = "1";
				lastGrids [f,k] = "1";
				moveGrids[k] = "s";
				lastMoveGrids[k] = "s";
			}
		}
		blockContainer = new GameObject ("block container");
	}



	//绘制窗口时调用
	void OnGUI () 
	{
		mapSize = EditorGUILayout.Vector2IntField("Map Size:", mapSize);
	
		GUI.SetNextControlName("FloorText");
		floor = EditorGUILayout.IntField ("层数", floor);

		if (floor > 0) {
			string[] floorNames = new string[floor];
			for (int i = 0; i < floor; i++) {
				floorNames [i] = (i+1).ToString ();;
			}

			GUILayout.BeginVertical("Box");

			selectedFloor = GUILayout.SelectionGrid (currentFloor, floorNames, 10);

			if (currentFloor != selectedFloor) {
				currentFloor = selectedFloor;
				GUI.FocusControl ("FloorText");

				for (int k = 0; k < floor; k++) {
					if ((k) != currentFloor) {
						for (int j = 0; j < totalGrids; j++) {
							Debug.LogFormat ("{0},{1}", k, currentFloor);
							if(allBlock [k, j] != null){
								BlockBase bb = allBlock [k, j].GetComponent<BlockBase> ();
								bb.hideObject();
							}
							
						}
					
					} else {
						for (int j = 0; j < totalGrids; j++) {
							Debug.LogFormat ("display{0},{1}", k, currentFloor);
							if(allBlock [k, j] != null){
								BlockBase bb = allBlock [k, j].GetComponent<BlockBase> ();
								bb.displayObject();
							}
						}
					}
				}

			}
		
			GUILayout.EndVertical ();
		}

		//更新网格数据
		if (isUpdate) {
			totalGrids = mapSize.x * mapSize.y;
			isUpdate = false;
			blockGrids = new string[totalFloors,totalGrids];
			moveGrids = new string[totalGrids];
			allBlock = new GameObject[totalFloors,totalGrids];

			int lastIndex = 0;

			if (mapSize.x >= lastMapSize.x) {
				for (int f = 0; f < totalFloors; f++) {
					lastIndex = 0;
					for (int k = 0; k < totalGrids; k++) {
						if (lastIndex >= lastMapSize.x * lastMapSize.y) {
							blockGrids [f, k] = "-1";
							moveGrids[k] = "s";
						}

						else if (k % (mapSize.x) >= (lastMapSize.x)) {
							blockGrids [f, k] = "-1";
							moveGrids[k] = "s";
						} 
						else {
							//Debug.LogFormat ("lastIndex ,k,f  {0} {1} {2}", f,k,JsonMapper.ToJson(blockGrids));
							blockGrids [f, k] = lastGrids[f, lastIndex];
							moveGrids[k] = lastMoveGrids[lastIndex];
							lastIndex++;
						}
					}
				}
			}else if(mapSize.x < lastMapSize.x){
				for(int f = 0 ; f < totalFloors ; f++){
					lastIndex = 0;
					for(int k = 0; k < totalGrids; k++) {
						if (lastIndex >= lastMapSize.x * lastMapSize.y) {
							blockGrids [f, k] = "-1";
							moveGrids [k] = "s";
						}
						Debug.LogFormat ("lastIndex ,k,f  {0} {1} {2}", f,k,JsonMapper.ToJson(blockGrids));
						blockGrids [f, k] = lastGrids[f, lastIndex];
						moveGrids[k] = lastMoveGrids[lastIndex];
						lastIndex++;
						if ((k+1) % (mapSize.x) == 0) {
							lastIndex += (lastMapSize.x - mapSize.x);
						} 
					}
				}

			}
			lastGrids = blockGrids;
			lastMoveGrids = moveGrids;
			lastMapSize = mapSize;
			Debug.Log (blockGrids);
			addBlocks ();
		}
			
		for(int i=0;i<totalGrids;i++){
			if (i %  lastMapSize.x == 0) {
				EditorGUILayout.BeginHorizontal (GUILayout.Height (30));
			}
			if (lastGrids != null) {
//				Debug.LogFormat ("{0},{1}", currentFloor, i);
				lastGrids[currentFloor,i] = EditorGUILayout.TextField("",lastGrids[currentFloor,i], GUILayout.Width (40),  GUILayout.Height (30));
				if ((i+1) %  lastMapSize.x == 0) {
					EditorGUILayout.EndHorizontal ();
				}
			}

			

		}

		EditorGUILayout.LabelField("移动地格");
		for(int i=0;i<totalGrids;i++){
			if (i %  lastMapSize.x == 0) {
					EditorGUILayout.BeginHorizontal (GUILayout.Height (30));
			}
			if (lastMoveGrids != null) {
	//				Debug.LogFormat ("{0},{1}", currentFloor, i);
				lastMoveGrids[i] = EditorGUILayout.TextField("",lastMoveGrids[i], GUILayout.Width (40),  GUILayout.Height (30));
				if ((i+1) %  lastMapSize.x == 0) {
					EditorGUILayout.EndHorizontal ();
				}
			}
		}

			
		if (GUILayout.Button ("Apply", GUILayout.Width (200))) {
		
			updateSetting ();
		}

		if(GUILayout.Button ("Clear", GUILayout.Width (200))) {

			OnDestroy ();
		}


		EditorGUILayout.BeginHorizontal (GUILayout.Height (30));
		if(GUILayout.Button ("Export-anther-json", GUILayout.Width (200), GUILayout.Height (20))) {

			exportToJson ();
		}
		fileName = EditorGUILayout.TextField("",fileName, GUILayout.Width (200),  GUILayout.Height (20));
		EditorGUILayout.EndHorizontal ();

		if(GUILayout.Button ("Export-json", GUILayout.Width (200))) {

			exportToJson();
		}

//		if(GUILayout.Button ("Import-json", GUILayout.Width (200))) {
//
//			importJson();
//		}

	
		TextAsset newTxtAsset = EditorGUILayout.ObjectField ("选择关卡", txtAsset, typeof(TextAsset), true) as TextAsset;
		if (newTxtAsset != txtAsset) {
			ReadTextAsset (newTxtAsset);
		}
	}

	void ReadTextAsset(TextAsset txt){
		string s = txt.text;
		txtAsset = txt;
		solveLevelData(s);
		isUpdate = true;

	}

	void exportToJson(){
		LevelFormat lf = new LevelFormat ();
		lf.grid =  JsonMapper.ToJson(lastGrids);
		lf.moveGrid =  JsonMapper.ToJson(lastMoveGrids);
		lf.floor = floor;
		lf.sizeX = (mapSize.x);
		lf.sizeY = (mapSize.y);
		lf.fileName = fileName;

		List<int> termsList = new List<int>();

		for (int k = floor - 1; k >= 0; k--) {
			for (int j = 0; j < mapSize.x; j++) {
				for (int i = mapSize.y-1; i >= 0; i--) {
					termsList.Add (int.Parse(lastGrids [k, i * mapSize.x + j  ]));
				}
			}
		}

		List<string> moveList = new List<string>();

		for (int j = 0; j < mapSize.x; j++) {
				for (int i = mapSize.y-1; i >= 0; i--) {
					Debug.LogFormat("lastMoveGrids {0},",lastMoveGrids [i * mapSize.x + j  ]);
					moveList.Add (lastMoveGrids [i * mapSize.x + j ]);
				}
			}

		lf.gridInGame = termsList.ToArray ();
		lf.moveGridInGame = moveList.ToArray ();

		// Debug.Log(JsonMapper.ToJson(lf));

		CreateFile (Application.dataPath+"/Resources/levels/"+fileName+".txt", JsonMapper.ToJson(lf));
		AssetDatabase.Refresh ();
	}

	public void CreateFile (string _filePath ,string _data)
	{
		StreamWriter sw;
		FileInfo fi = new FileInfo (_filePath);
			//创建文件
		sw = fi.CreateText ();
		sw.WriteLine (_data);//以行的形式写入
		sw.Close ();//关闭流
		sw.Dispose ();//销毁流

		this.ShowNotification(new GUIContent("File Saved Completed"));
	}

	void importJson(){
		ReadFile (Application.dataPath + "/levels/level.txt");
	}

	private void ReadFile(string _filePath){
	
		StreamReader sr = File.OpenText(_filePath);  
		string s = sr.ReadToEnd ();
		Debug.LogFormat ("reading {0}", s);
		solveLevelData(s);
		isUpdate = true;
	}

	void solveLevelData(string _levelData){
		LevelFormat loadLevel = JsonMapper.ToObject<LevelFormat>(_levelData);
		if(loadLevel == null) {
			return;
		}
		if (loadLevel.fileName != null && loadLevel.fileName != "") {
			fileName = loadLevel.fileName;
		}


		string[] g = JsonMapper.ToObject<string[]> (loadLevel.grid);
		
		string[] move = null;
		if(loadLevel.moveGrid != null){
			move = JsonMapper.ToObject<string[]> (loadLevel.moveGrid);
		}
		

		mapSize = new Vector2Int (loadLevel.sizeX, loadLevel.sizeY);
		lastMapSize = new Vector2Int (loadLevel.sizeX, loadLevel.sizeY);
		floor = loadLevel.floor;
		totalGrids = mapSize.x * mapSize.y;

		lastGrids = new string[totalFloors,totalGrids];
		blockGrids = new string[totalFloors,totalGrids];
		moveGrids = new string[totalGrids];
		lastMoveGrids = new string[totalGrids];

		for (int f = 0; f < totalFloors; f++) {
			for (int k = 0; k < totalGrids; k++) {
				blockGrids[f,k] = "1";
				lastGrids [f,k] = "1";
				moveGrids[k] = "s";
				lastMoveGrids[k] = "s";
			}
		}

		Debug.Log (totalGrids);
		for (int i = 0; i < floor; i++) {
			for (int j = 0; j < totalGrids; j++) {
				lastGrids [i, j] = g [i * totalGrids + j];
				blockGrids [i, j] = g [i * totalGrids + j]; 
				if(move != null){
					moveGrids[j] = move [j];
					lastMoveGrids[j] = move [j];
				}

			}
		}
	}


	void updateSetting(){
		isUpdate = true;
		GUI.FocusControl ("FloorText");

	}

	void addBlocks(){
		OnDestroy ();
		blockContainer = new GameObject ("block container");
		for (int i = 0; i < floor; i++) {
			Vector3 v = new Vector3 (0,i+1,0);
			Quaternion turnRotation= Quaternion.Euler (0f, 0f, 0f);
			for (int j = 0; j < totalGrids; j++) {
				if(int.Parse (lastGrids[i, j]) >= 0){
					v.x =  (j % lastMapSize.x) * 1.0f + 0.5f;
					v.z = -Mathf.Ceil(j / lastMapSize.x) * 1.0f+0.5f ;
					Object blockPreb = Resources.Load( "BlockBase", typeof( GameObject ) );
					GameObject block = Instantiate( blockPreb,v ,turnRotation) as GameObject;	
					block.transform.parent = blockContainer.transform;
					block.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
					BlockBase bBase = block.GetComponent<BlockBase> ();
					int color = bBase.getColorIndex ();
					int value = int.Parse (lastGrids [i, j]);
					// if (value < 0) {
					// 	value = 0;
					// }
					allBlock [i, j] = block;
					bBase.setEditorColor (value);
					Debug.Log (color);
				}else{
					allBlock [i, j] = null;
				}
			}
		}
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

		if (blockContainer) {
			foreach (Transform child in blockContainer.transform)
			{

				DestroyImmediate (child.gameObject);
			}
			DestroyImmediate (blockContainer);
		}

	}
}
