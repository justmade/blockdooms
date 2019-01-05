using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using LitJson;
using System.IO;
using LFormat;
using UnityEditor;

public class GameManager : MonoBehaviour {

	public GameObject m_BlockPrefabs;

	public Elements elementConfig;

	//所有的block对象
	public GameObject[,] allBlocks;
	//记录每次的删除序列
	private List<BlockState> removeSeuqence;
	//需要撤销的数组
	private List<BlockState> redoSeuqence;

	public List<GameObject> dropBlocks;
	public List<GameObject> moveBlocks;

	//记录格子的状态
	public BlockState[,] blockStates;

	//每一层最上面格子的颜色
	public BlockState[] topBlockSates;

	public Text m_MessageText; 

	public Text debug_msg_1; 

	public Text debug_msg_2; 

	private GameObject container;

	public Button addFloorBtn;

	public Button addUpBtn;
	public Button redoButton;

	int currentFloor;

	public List<int> allDisappearIndex;

	//宽度
	private int B_Width = 0;
	//判断是否还有格子可以消除
	private bool checkSameColor;

	public GameObject boomParticle;

	private bool isPlaying;

	private bool needDestory;

	private float totalMove = 10f;

	private float deltaTime = 0f;

	private bool isMainC = true;

    public Camera mainCamera;

	public Camera sideCamera;

    private float rotateAngle = 80f;

    private float rotateFrame = 30;

    private int currentRotateFrame = -1;

    private bool isElevate = false;

    private int cameraMove = 0;

	private int generateFloors = 3;

	private int blocksLeftCounts;

	private int B_Height = 10;

	private int levelFloor = 1;

	private List<int> loadGridData;

	private List<string> files;

	public TextAsset txtAsset;

	public GameObject LevelUI;

	private GameObject LevelUIObj;

	private string currentLevelName;

	//找到宝箱和钥匙
	private bool findTreasureKey;
	//记录操作序列
	private int gameStep = 0;

	private LevelController levelController;

    // Use this for initialization

	void Awake(){
		Application.targetFrameRate = 60;
	}

    void Start () {
		// LevelUIObj = Instantiate(LevelUI) as GameObject;
		// LevelUIObj.GetComponent<LevelSelector> ().gm = this;
		// addFloorBtn.enabled = false;
		// addUpBtn.enabled = false;
		// redoButton.enabled = false;
		// addFloorBtn.gameObject.SetActive (false);
		// addUpBtn.gameObject.SetActive (false);
		// redoButton.gameObject.SetActive (false);
		levelController = FindObjectOfType<LevelController>();
		GameStart(LevelDataInfo.selectLevelName);
	}

	void GameStart(string _s){
		// Destroy (LevelUIObj);
		addFloorBtn.gameObject.SetActive (true);
		addUpBtn.gameObject.SetActive (true);
		redoButton.gameObject.SetActive(true);
		addFloorBtn.enabled = true;
		addUpBtn.enabled = true;
		redoButton.enabled = true;
		currentLevelName = _s;
//		Debug.Log(this.gameObject.name + " Get: "+_s);

//		if (txtAsset) {
//			solveLevelData (txtAsset.text);
//		}
		InitGame(currentLevelName);
	}

	void InitGame(string _s){
		removeSeuqence = new List<BlockState>();
		redoSeuqence = new List<BlockState>();
		currentLevelName = _s;
		gameStep = 0 ;	
		loadLevelData (_s);

		initAllBlock ();
		Button btn = addFloorBtn.GetComponent<Button>();
		btn.onClick.AddListener(onBack);
		Button upBtn = addUpBtn.GetComponent<Button>();
		upBtn.onClick.AddListener(onRetry);
		Button redoBtn = redoButton.GetComponent<Button>();
		redoBtn.onClick.AddListener(onRedo);

		mainCamera.enabled = isMainC;
		sideCamera.enabled = !isMainC;
		onUp ();
	}
		

	void loadLevelData(string _filePath){
		
//		StreamReader sr = File.OpenText(_filePath);  
		var textFile = Resources.Load<TextAsset>("Levels/"+_filePath);


//		string _levelData = sr.ReadToEnd ();

		string _levelData = textFile.text;
		solveLevelData (_levelData);
	}

	void solveLevelData(string data){
		Debug.Log(data);
		LevelFormat loadLevel = JsonMapper.ToObject<LevelFormat>(data);

		int[] g =  (loadLevel.gridInGame);

		B_Width = loadLevel.sizeX;
		levelFloor = loadLevel.floor;
		generateFloors = levelFloor - 1;

		loadGridData = new List<int> (g);

		Debug.LogFormat ("debug in gridingame {0}" , loadGridData.Count);
	}

	void onBack(){
		// Application.LoadLevel(Application.loadedLevel);
		SceneManager.LoadScene("LevelSelect");
		
		
	}

	IEnumerator finishLevel(){
		yield return new WaitForSeconds(0.7f);
		SceneManager.LoadScene("LevelSelect");
		levelController.LevelComplete(currentLevelName,3);
	}

	void onRetry(){
		Button btn = addFloorBtn.GetComponent<Button>();
		btn.onClick.RemoveAllListeners();
		Button upBtn = addUpBtn.GetComponent<Button>();
		upBtn.onClick.RemoveAllListeners();
		Button redoBtn = redoButton.GetComponent<Button>();
		redoBtn.onClick.RemoveAllListeners();
		destoryAllBlocks ();
		InitGame(currentLevelName);
	}

	void onRedo(){
		redoSeuqence.Clear();
		if(removeSeuqence.Count > 0){
			BlockState blockStep = removeSeuqence[removeSeuqence.Count-1];
			removeSeuqence.RemoveAt(removeSeuqence.Count-1);
			redoSeuqence.Add(blockStep);
			for(int i = removeSeuqence.Count - 1 ; i >=0 ; i--){
				if(blockStep.step == removeSeuqence[i].step){
					redoSeuqence.Add(removeSeuqence[i]);
					removeSeuqence.RemoveAt(i);
				}
			}
		}
		Debug.LogFormat("redoSeuqence {0},",redoSeuqence.Count);
		redoBlock();
	}

	void destoryAllBlocks(){
		Resources.UnloadUnusedAssets ();
		foreach (Transform child in container.transform)
		{
			Destroy (child.gameObject);
		}
		Destroy (container);
	}

	void onAddFloor(){
		Application.LoadLevel(Application.loadedLevel);
    }


	void onUp(){
		currentRotateFrame = 0;
		cameraMove = 1;
	}

	// Update is called once per frame
	void Update () {
		if (isPlaying == false) {
			touchBlock ();
		} 

		if (needDestory == true) {
			isPlaying = true;
			//找到钥匙和宝箱之后 先播放block的消失动画，在去消除key和treasure的block
			if (findTreasureKey) {
				int counts = B_Width * B_Width;
				for (int i = 0; i < counts; i++) {
					for (int k = 0; k < currentFloor + 1; k++) {
						if (allBlocks [k, i]) {
							BlockBase bBase = allBlocks [k, i].GetComponent<BlockBase> ();
							if (bBase.isPlayingAimation) {
								redoButton.gameObject.SetActive (false);
								return;
							}
						}
					}

				}
				checkAllBlocks ();
			} else {
				redoButton.gameObject.SetActive (true);
				isPlaying = false;
			}
			if (deltaTime > 2 * totalMove) {
				needDestory = false;
				deltaTime = 0f;
				//isPlaying = false;
			} else {
				//isPlaying = true;
				deltaTime++;
				if (deltaTime <= totalMove) {
					if (dropBlocks.Count == 0) {
						deltaTime = totalMove;
					}
					//droping (deltaTime);
				} else if (deltaTime > totalMove) {
					//moving (deltaTime - totalMove);
				}
			}

		}else if (cameraMove == 1 && currentRotateFrame >= 0 && currentRotateFrame <= rotateFrame) {
			
            currentRotateFrame++;
			// Debug.LogFormat("currentRotateFrame {0}",currentRotateFrame);
            mainCamera.transform.RotateAround(Vector3.zero, Vector3.left, rotateAngle / rotateFrame);

        }

		if (currentRotateFrame == rotateFrame) {
			currentRotateFrame = -1;
			isElevate = true;
			deltaTime = 0f;
			cameraMove = 0;
			generateBlock(currentFloor + 1);
			generateFloors--;
		}

        if (isElevate) {

            if (deltaTime > totalMove)
            {
				if (generateFloors > 0) {
					deltaTime = 0f;
					currentRotateFrame = Mathf.FloorToInt(rotateFrame);
				} else {
					isElevate = false;
					deltaTime = 0f;
					currentRotateFrame = 0;
					cameraMove = -1;
				}
            }
            else {
                deltaTime++;
                elevateBlocks(deltaTime);
            }
        }else if (cameraMove == -1 && currentRotateFrame >= 0 && currentRotateFrame <= rotateFrame)
        {
            currentRotateFrame++;
            mainCamera.transform.RotateAround(Vector3.zero, Vector3.left, -rotateAngle / rotateFrame);
            if (currentRotateFrame == rotateFrame)
            {
                currentRotateFrame = -1;
                deltaTime = 0f;
                cameraMove = 0;
            }
        }
    }

	//检查同一格子下层的block是否存在
	int checkBlockBlockFloor(int blockIndex){
		int counts = B_Width * B_Width;
		for(int i = 0 ; i < levelFloor ; i++){
			if(loadGridData[0 + i * counts] != -1){
				return loadGridData[0 + i * counts];
			}
			allBlocks[0,blockIndex] = null;
		}
		return -1;
	}

	Object GetBlockPrefabByID(int blockColorID){
		Object blockPreb;
		if(blockColorID == elementConfig.Blue){
			blockPreb = Resources.Load( "VoxelBlockBlue", typeof( GameObject ) );
		}else if(blockColorID == elementConfig.Green){
			blockPreb = Resources.Load( "VoxelBlockGreen", typeof( GameObject ) );
		}else if(blockColorID == elementConfig.Red){
			blockPreb = Resources.Load( "VoxelBlockRed", typeof( GameObject ) );
		}else if(blockColorID == elementConfig.Yellow){
			blockPreb = Resources.Load( "VoxelBlockYellow", typeof( GameObject ) );
		}else if(blockColorID == elementConfig.Treasure){
			blockPreb = Resources.Load( "VoxelBlackTLeft", typeof( GameObject ) );
		}else if(blockColorID == elementConfig.Key){
			blockPreb = Resources.Load( "VoxelBlackTRight", typeof( GameObject ) );
		}else{
			blockPreb = Resources.Load( "VoxelBlackTRight", typeof( GameObject ) );
		}

		return blockPreb;
	}

	void initAllBlock(){
		blocksLeftCounts = 0;
		blockStates = new BlockState[B_Height, B_Width * B_Width];
		topBlockSates = new BlockState[B_Width * B_Width];
		currentFloor = 0;
		allBlocks = new GameObject[B_Height, B_Width * B_Width];
		Vector3 v = new Vector3 (0,1,0);
		Quaternion turnRotation= Quaternion.Euler (0f, 0f, 0f);
		container =  new GameObject ("block container");
		int counts = B_Width * B_Width;
		for (int i = 0; i < counts; i++){	
			if(loadGridData[0] != -1){
				v.x = Mathf.Ceil (i / B_Width) * 1.2f + 0.5f;
				v.z = i % B_Width * 1.2f+0.5f ;
				// ... create them, set their player number and references needed for control.
				// Object prefab = Resources.Load("VoxelBlockGreen", typeof( GameObject ));
				GameObject block = 
					Instantiate(GetBlockPrefabByID(loadGridData[0]), v, turnRotation) as GameObject;
				allBlocks[0,i] = block;
				block.transform.localScale = new Vector3 (1f, 1f, 1f);
				block.tag = "Block";
				block.transform.parent = container.transform;
				BlockBase bBase = block.GetComponent<BlockBase> ();
				if(loadGridData.Count > counts){
					bBase.setColor (loadGridData[0],loadGridData[counts]);
				}else{
					bBase.setColor (loadGridData[0],-1);
				}
				loadGridData.RemoveAt (0);
				int color = bBase.getColorIndex ();
				blockStates [0, i] = new BlockState ();
				blockStates [0,i].color = color;
				blockStates [0,i].floor = currentFloor;
				topBlockSates[i] = new BlockState ();
				topBlockSates[i].color = -1;
				if(color != elementConfig.Unlock){
					// Debug.LogFormat ("color,{0},{1}", color,blocksLeftCounts);
					blocksLeftCounts++;
				}
			}else{
				allBlocks[0,i] = null;
				loadGridData.RemoveAt (0);
				blockStates [0, i] = new BlockState ();
				blockStates [0,i].color = -1;
				blockStates [0,i].floor = currentFloor;
			}
			// updateLeftText ();
		}

		container.transform.position = new Vector3(-(B_Width * 1.0f) / 2,0,-(B_Width * 1.0f) / 2);
	}

	void addBoomParticle(Vector3 v,int color){
		v.y++;
		GameObject boom = 
			Instantiate(boomParticle, v, Quaternion.Euler (0f, 0f, 0f)) as GameObject;
		boom.transform.parent = container.transform;
		ParticleSystem p = boom.GetComponent<ParticleSystem> ();
		boom.GetComponent<Renderer>().material.color = getColorByID(color);
		p.Play ();
		Destroy(p,p.startLifetime); 
	}

	Color getColorByID(int colorID){
		Color newColor;
		if(colorID == elementConfig.Red){
			newColor = new Color(240/255f, 92/255f, 66/255f, 1f);
			Debug.Log(newColor.ToString());
		}else if(colorID == elementConfig.Green){
			newColor = new Color(0/255f, 132/255f ,83/255f, 1f);
		}else if(colorID == elementConfig.Blue){
			newColor = new Color(59/255f, 85/255f, 120/255f, 1f);
		}else if(colorID == elementConfig.Yellow){
			newColor = new Color(205/255f, 164/255f, 0/255f, 1f);
		}else{
			newColor = Color.white;
		}
		return newColor;
	}


    void elevateBlocks(float deltaTime) {
        int counts = B_Width * B_Width;
        for (int i = 0; i < (currentFloor+1); i++)
        {
            for (int j = 0; j < counts; j++)
            {
                if (allBlocks[i, j])
                {
                    BlockBase blockBase = allBlocks[i, j].GetComponent<BlockBase>();
                    blockBase.elevate(deltaTime);
                }
            }
        }
    }

	
	//生产层数 只会检查当前层的下一层
	void generateBlock(int floor){
		int counts = B_Width * B_Width;
		currentFloor = floor;
		// Debug.LogFormat ("currentFloor {0}", currentFloor);

		Quaternion turnRotation= Quaternion.Euler (0f, 0f, 0f);
		Vector3 v = new Vector3 (0,1,0);
		for (int i = 0; i < counts; i++){
			if(loadGridData.Count <= 0){
				return;
			}
			if(loadGridData[0] != -1){
				v.x = Mathf.Ceil (i / B_Width) * 1.2f + 0.5f;
				v.z = i % B_Width * 1.2f+0.5f ;
				// ... create them, set their player number and references needed for control.
				GameObject block = 
					Instantiate(GetBlockPrefabByID(loadGridData[0]), Vector3.zero, turnRotation) as GameObject;
				block.transform.parent = container.transform;
				block.transform.localPosition = v;
				block.transform.localScale = new Vector3 (0.0f, 0.0f, 0.0f);
				block.tag = "Block";
				allBlocks[currentFloor,i] = block;
				BlockBase bBase = block.GetComponent<BlockBase> ();
				bBase.setColor (loadGridData[0]);
				loadGridData.RemoveAt (0);
				int color = bBase.getColorIndex ();
				if(blockStates [0,i].color == -1){
					blockStates [0, i].color = color;
					blockStates [0 ,i].floor = currentFloor;
					bBase.playAmplify = true;
				}
				if(color!=0){
					blocksLeftCounts++;
				}
			}else{
				allBlocks[currentFloor,i] = null;
				loadGridData.RemoveAt (0);
				blockStates [0, i].color = -1;
			}
			

		}
		updateLeftText ();
		// Lightmapping.BakeAsync();
		// Lightmapping.
		// Lightmapping.Bake();
	}

	void touchBlock(){
		if (mainCamera.GetComponent<CameraFollow> ().startMove == true) {
			return;
		}

		if (Input.GetMouseButtonUp(0)) {
			Ray ray;
			if (isMainC) {
				ray = mainCamera.ScreenPointToRay (Input.mousePosition);  
			} else {
				ray = sideCamera.ScreenPointToRay (Input.mousePosition);  
			}

			RaycastHit hit;

			if(Physics.Raycast (ray,out hit))    //如果真的发生了碰撞，ray这条射线在hit点与别的物体碰撞了
			{
				allDisappearIndex.Clear ();
				checkSameColor = false;
				if (hit.collider.gameObject.tag == "Block") {
					BlockBase bb = hit.collider.GetComponent<BlockBase> ();
					if (bb.getColorIndex() == 0) {
						return;					
					}

					// bb.tapEffect();
					// return;/
					BlockState bs = findBlockIndex (hit.collider.gameObject);
					Debug.LogFormat("hit {0},color{1}",bs.originalIndex,bb.getColorIndex ());
					addDisappearIndex (bs.originalIndex);
					if (isMainC) {
						findNeighbour (bs.originalIndex, bb.getColorIndex (), 0);
					} else {
						int topIndex = originalIndex2TopInde (bs.originalIndex , bs.floor);
						findTopNeighbour(topIndex, bb.getColorIndex (), 0);
					}
				}

				checkAllBlocks ();

//				findTopBlock ();
				if (checkSameColor == false) {
//                    currentRotateFrame = 0;
//                    cameraMove = 1;
						m_MessageText.text = "Gameover！ 剩余方块：" + blocksLeftCounts;
					if(blocksLeftCounts == 0){
						m_MessageText.text = "成功！！！";
						StartCoroutine(finishLevel());
					}
				} else {
					// m_MessageText.text = "消除";
				}
			}
		}
	}

	//检测已经更新所有的block
	private void checkAllBlocks(){
		removeBlocks ();
		//dropBlock ();
		//leftMoveBlock ();
		updateBlockState ();
		findHorizontalConnect ();
		findVerticalConnect ();
	}


	void updateLeftText(){
		m_MessageText.text = "剩余方块：" + blocksLeftCounts;
		if(blocksLeftCounts == 0){
			m_MessageText.text = "成功！！！";
			StartCoroutine(finishLevel());
		}
	}



	//在数组中找到对应的下标
	BlockState findBlockIndex(GameObject bBase){
		for (int i = 0; i < currentFloor + 1; i++) {
			for( int j = 0 ; j < allBlocks.GetLength(1) ;j ++){
				if (bBase == allBlocks [i, j]) {
					BlockState bs = new BlockState ();
					bs.originalIndex = j;
					bs.floor = i;
					return bs;
				}
			}
		}
		return new BlockState();
	}

	//获取每一次最上面的方块 从上到下，从左到右
	void findTopBlock(){
		int index = 0;
		string top = "";
		for (int i = 0; i < currentFloor + 1; i++) {
			for(int j = B_Width -1  ; j >= 0 ;j --){
				int colum = j * B_Width;
				int topColor = -1;	
				int oriIndex = -1;
				int floor = -1;
				for (int k = B_Width-1; k >= 0; k--) {
					int bIndex = colum + k;
					GameObject block = allBlocks [i, bIndex];
					if (block) {
						BlockBase blockBase = block.GetComponent<BlockBase>();
						topColor = blockBase.getColorIndex ();
						oriIndex = bIndex;
						floor = i;
						break;
					}
				}
				//Debug.Log (index);
				topBlockSates[index].floor = floor;
				topBlockSates[index].color = topColor;
				topBlockSates[index].originalIndex = oriIndex;
				index++;
				top = top + topColor;
			}
			top += "\n";
		}
		Debug.LogFormat ("top {0}", top);
	}
		

	string printBlock(){
		string d = "";
		for (int i = B_Width-1; i >=0; i--) {
			for (int j = 0; j < B_Width; j++) {
				int aIndex = i + B_Width * j;
				d += blockStates [0, aIndex].color;

			}
			d += "\n";
		}
		return d;
//		Debug.LogFormat ("{0}", d);
	}

	//递归查找格子四周的同色目标 currentDir上次的位置，放置来回寻找堆栈溢出 1：上 2：下 4：右 8：左
	void findNeighbour(int index , int color , int currentDir)
	{
		int rightIndex = index + B_Width;

		int leftIndex = index - B_Width;
	

		int downIndex = -1;
		if (index % B_Width != 0) {
			downIndex = index - 1;
		}

		int upIndex = -1;
		if ((index + 1) % B_Width != 0) {
			upIndex = index + 1;
		}

		if (rightIndex < B_Width * B_Width && currentDir != 8) {
			if (getBlockColor (rightIndex) == color) {
				if (addDisappearIndex (rightIndex)) {
					findNeighbour (rightIndex, color , 4);
				}
			}
		}

		if (leftIndex >= 0 && currentDir != 4) {
			if (getBlockColor (leftIndex) == color) {
				if(addDisappearIndex(leftIndex)){
					findNeighbour (leftIndex, color,8);
				}
			}
		}

		if (downIndex >= 0 && getBlockColor (downIndex) == color && currentDir != 1) {
			if (addDisappearIndex (downIndex)) {
				findNeighbour (downIndex, color,2);
			}
		}

		if (upIndex >= 0 && getBlockColor (upIndex) == color && currentDir != 2) {
			if (addDisappearIndex (upIndex)) {
				findNeighbour (upIndex, color,1);
			}
		}

		return;
	}

	//递归查找顶部视角格子四周的同色目标 currentDir上次的位置，放置来回寻找堆栈溢出 1：上 2：下 4：右 8：左
	void findTopNeighbour(int index , int color , int currentDir)
	{
		Debug.LogFormat ("findTopNeighbour {0}" , index);
		int rightIndex = -1;
		if ((index + 1) % B_Width != 0) {
			rightIndex = index + 1;
		}
			
		int leftIndex = -1;
		if (index % B_Width != 0) {
			leftIndex = index - 1;
		}
			
		int downIndex = index + B_Width;

		int upIndex = index - B_Width;
	
		if (rightIndex >= 0  && currentDir != 8) {
			if (getTopBlockColor (rightIndex) == color) {
				if (addDisappearIndex (topIndex2originalIndex(rightIndex))) {
					findTopNeighbour (rightIndex, color , 4);
				}
			}
		}

		if (leftIndex >= 0 && currentDir != 4) {
			if (getTopBlockColor (leftIndex) == color) {
				if(addDisappearIndex(topIndex2originalIndex(leftIndex))){
					findTopNeighbour (leftIndex, color,8);
				}
			}
		}

		if (downIndex <  B_Width * B_Width && getTopBlockColor (downIndex) == color && currentDir != 1) {
			if (addDisappearIndex (topIndex2originalIndex(downIndex))) {
				findTopNeighbour (downIndex, color,2);
			}
		}

		if (upIndex >= 0 && getTopBlockColor (upIndex) == color && currentDir != 2) {
			if (addDisappearIndex (topIndex2originalIndex(upIndex))) {
				findTopNeighbour (upIndex, color,1);
			}
		}

		return;
	}

	//将top的下标转换成原始的下标
	int topIndex2originalIndex(int index)
	{
		return topBlockSates [index].originalIndex;
	}

	//将原始下标转换成top下标
	int originalIndex2TopInde(int index , int floor){
		for (int i = 0; i < topBlockSates.Length; i++) {
			if (topBlockSates [i].originalIndex == index 
				&& topBlockSates [i].floor == floor) {
				return i;
			}
		}
		return -1;
	}


	//添加要消除的目标下标
	bool addDisappearIndex(int index){
		if (allDisappearIndex.IndexOf (index) == -1) {
			allDisappearIndex.Add (index);
			return true;
		}
		return false;
	}

	//获取blockStates中的block颜色
	int getBlockColor(int index){
		return blockStates[0,index].color;
	}

	//获取topBlocks中的block颜色
	int getTopBlockColor(int index){

		Debug.LogFormat ("topBlockSates [index].color {0} , {1}", index, topBlockSates [index].color);

		return topBlockSates [index].color;
	}

	void removeBlocks(){
		if (allDisappearIndex.Count > 1) {
			//如果需要消除的block是宝箱和钥匙，那么他们应该被算在上一步一起
			if(!findTreasureKey){
				gameStep ++;
			}
			blocksLeftCounts -= allDisappearIndex.Count;
			updateLeftText ();
			needDestory = true;
//			Debug.LogFormat ("remove {0}", allDisappearIndex.Count);
			foreach (int element in allDisappearIndex) 
			{
				Debug.Log(element);
				int index = element;
				for (int i = 0; i < currentFloor + 1; i++) {
					GameObject block = allBlocks [i,index];
					if (block) {
						block.GetComponent<BlockBase>().tapEffect();
						//Destroy (block);
						//Debug.LogFormat ("removeIndex {0} , {1}", i, index);
						BlockState recordStep = new BlockState();
						recordStep.color = blockStates [0,index].color;
						recordStep.floor = i;
						recordStep.step = gameStep;
						recordStep.originalIndex = index;
						removeSeuqence.Add(recordStep);

						allBlocks [i, index] = null;
						blockStates [0,index].color = -1;
						// addBoomParticle (block.transform.position,recordStep.color);
						break;
					}
				}
			}
		}
	}

	void redoBlock(){
		foreach(BlockState bs in redoSeuqence){
			
			int oIndex = bs.originalIndex;
			int currentColor = blockStates [0,oIndex].color;
			blockStates [0,bs.originalIndex].color = bs.color;
			blockStates [0,bs.originalIndex].floor = bs.floor;

			// if(allBlocks[bs.floor+1,oIndex] != null){
			// 	allBlocks[bs.floor+1,oIndex].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
			// }	
			
			Vector3 v = new Vector3 (0,2,0);
			Quaternion turnRotation= Quaternion.Euler (0f, 0f, 0f);

			v.x = Mathf.Ceil (oIndex / B_Width) * 1.2f + 0.5f;
			v.z = oIndex % B_Width * 1.2f+0.5f ;
			// v.y = 2 - bs.floor;
			Vector3 v2 =  container.transform.InverseTransformVector(v);
			// ... create them, set their player number and references needed for control.
			GameObject block = 
				Instantiate(GetBlockPrefabByID(bs.color),  Vector3.zero, turnRotation) as GameObject;
			allBlocks [bs.floor,oIndex] = block;
			block.transform.localScale = new Vector3 (1f, 1f, 1f);
			block.tag = "Block";
			block.transform.parent = container.transform;
			block.transform.localPosition = v;
			// block.transform.position = v; 
			blocksLeftCounts ++;
			updateLeftText();
			BlockBase bBase = block.GetComponent<BlockBase> ();
			if(currentColor !=-1){
				bBase.setColor (bs.color,currentColor);
				allBlocks [1,oIndex].transform.localScale = new Vector3 (0f, 0f, 0f);
			}else{
				bBase.setColor(bs.color);
			}		
		}
	}

	//下落格子
	void dropBlock(){
		dropBlocks.Clear ();
		for (int i = 0; i < B_Width; i++) {
			int colum = i * B_Width;
			int empty = 0;
			int floor = -1;
			for (int j = 0; j < B_Width; j++) {
				int bIndex = colum + j;
				if (blockStates[0,bIndex].color == -1) {
					if (floor != -1 && blockStates [0, bIndex].floor != floor) {
						empty = 0;
					}
					empty++;
					floor = blockStates [0, bIndex].floor;

				} else if(floor == blockStates [0, bIndex].floor){
					int temp = blockStates [0,bIndex].color;
					//这里需要判断消失的方块上面是是否可以下落，才可以减。如果是同一层的下落，不是的就用下一层
					if (empty > 0) {
//						Debug.LogFormat ("empty {0}", empty);
						//int temp = blockStates [bIndex];
						blockStates [0,bIndex].color = -1;
						blockStates [0,bIndex - empty].color = temp;
						blockStates [0,bIndex - empty].floor = floor;

//						for (int k = 0; k < 1; k++) {
							if (allBlocks[floor,bIndex]) {
								BlockBase bBase = allBlocks[floor,bIndex].GetComponent<BlockBase>();
								bBase.dropBlock (empty);
								GameObject block = allBlocks [floor,bIndex];
								allBlocks [floor,bIndex] = null;
								allBlocks [floor,bIndex - empty] = block; 
								dropBlocks.Add (block);
//								break;
							}

//						}
					}
				}
			}
		}
		updateBlockState ();
	}


	//补充新block
	void updateBlockState(){
		int counts = B_Width * B_Width;
		for (int i = 0; i < counts; i++) {
			if (blockStates [0,i].color == -1) {
				for (int k = 0; k < currentFloor + 1; k++) {
					
					if (allBlocks [k, i]) {
						BlockBase bBase = allBlocks[k,i].GetComponent<BlockBase>();
						int color = bBase.getColorIndex ();
						//Debug.LogFormat ("update {0} , {1}" , i , color);
						blockStates [0, i].color = color;
						blockStates [0, i].floor = k;
						bBase.setPlayAmplify (true);
						break;

					}
				
				}
			}

		}
	}

	void droping(float delatTime){
		foreach (GameObject element in dropBlocks) {
			BlockBase bBase = element.GetComponent<BlockBase>();
			bBase.drop (delatTime);
		}
	}

	void moving(float delatTime){
		foreach (GameObject element in moveBlocks) {
			BlockBase bBase = element.GetComponent<BlockBase>();
			bBase.move (delatTime);
		}
	}



	//向左靠拢格子
	void leftMoveBlock(){
		moveBlocks.Clear ();
		int line = 0;
		int empty = 0;
		int floor = -1;
		for (int j = 0; j < B_Width; j++) {
			int bIndex = line + j * B_Width;
			if (blockStates[0,bIndex].color == -1) {
				empty++;

			} else {
				if (empty > 0) {
					//Debug.LogFormat ("empty {0}", empty);
					for (int i = 0; i < B_Width; i++) {
						int lIndex = bIndex + i;
						int temp = blockStates[0,lIndex].color;
						floor = blockStates [0, lIndex].floor;
						if (temp != -1) {
							blockStates[0,lIndex].color = -1;
							blockStates[0,lIndex - empty * B_Width].color = temp;
							blockStates [0, lIndex - empty * B_Width].floor = floor;
							for (int k = 0; k < currentFloor + 1; k++) {
								if (allBlocks[k,lIndex]) {
									BlockBase bBase = allBlocks[k,lIndex].GetComponent<BlockBase>();
									bBase.leftMoveBlock (empty);
									GameObject block = allBlocks [k,lIndex];
									allBlocks [k,lIndex] = null;
									allBlocks [k,lIndex - empty * B_Width] = block;
									moveBlocks.Add (block);
//									break;
								}
							}

						}
					}
				}
			}
		}
	}
	//横向搜索是否存在可以消除的格子
	void findHorizontalConnect(){
		findTreasureKey = false;
		allDisappearIndex.Clear ();
		int keyIndex = -1;
		int treasureIndex = -1;
		for (int i = 0; i < B_Width; i++) {
			int lastColor = -1;
			for (int j = 0; j < B_Width; j++) {
				int bIndex = i + j * B_Width;
				int temp = blockStates[0,bIndex].color;
				if (lastColor == temp && temp != -1) {
					//Debug.LogFormat ("hor{0},{1}",bIndex,lastColor);
					checkSameColor = true;
				} else {
					lastColor = temp;
				}

				if (temp == elementConfig.Key) {
					keyIndex = bIndex;
				}

				if (temp == elementConfig.Treasure) {
					treasureIndex = bIndex;
				}
			}
		}
		//是否有可消除的key和treasure
		if (keyIndex != -1 && treasureIndex != -1) {
			addDisappearIndex (keyIndex);
			addDisappearIndex (treasureIndex);
			findTreasureKey = true;
		}

	}

	void findVerticalConnect(){
		if (checkSameColor) {
			return;
		}
		for (int i = 0; i < B_Width; i++) {
			int lastColor = -1;
			int colum = i * B_Width;
			for (int j = 0; j < B_Width; j++) {
				int bIndex = colum + j;
				int temp = blockStates[0,bIndex].color;
				if (lastColor == temp && temp != -1) {
					Debug.LogFormat ("vertical{0},{1}",bIndex,lastColor);
					checkSameColor = true;
					return;
				} else {
					lastColor = temp;
				}
			}
		}
	}

}
