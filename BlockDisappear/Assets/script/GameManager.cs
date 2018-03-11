using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject m_BlockPrefabs;

	//所有的block对象
	public GameObject[,] allBlocks;
	//记录格子的状态
	public int[,] blockStates;

	public List<int> allDisappearIndex;

	//宽度
	public int B_Width = 5;
	//判断是否还有格子可以消除
	private bool checkSameColor;

	// Use this for initialization
	void Start () {
		initAllBlock ();
	}
	
	// Update is called once per frame
	void Update () {
		touchBlock ();
	}

	void initAllBlock(){
		blockStates = new int[B_Width, B_Width * B_Width];
		allBlocks = new GameObject[B_Width, B_Width * B_Width];
		Vector3 v = new Vector3 (0,1,0);
		Quaternion turnRotation= Quaternion.Euler (0f, 0f, 0f);
		GameObject container = GameObject.Find("BlockContainer");
		int counts = B_Width * B_Width;
		for (int i = 0; i < counts; i++){			
			v.x = Mathf.Ceil (i / B_Width) * 1.0f + 0.5f;
			v.z = i % B_Width * 1.0f+0.5f ;
			// ... create them, set their player number and references needed for control.
			GameObject block = 
				Instantiate(m_BlockPrefabs, v, turnRotation) as GameObject;
			allBlocks[0,i] = block;
			block.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			block.tag = "Block";
			block.transform.parent = container.transform;
			BlockBase bBase = block.GetComponent<BlockBase> ();
			int color = bBase.getColorIndex ();
			blockStates [0,i] = color;
		}

		container.transform.position = new Vector3(-(B_Width * 1.0f) / 2,0,-(B_Width * 1.0f) / 2);
	}

	void touchBlock(){
		if (Input.GetMouseButtonUp(0)) {

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
			RaycastHit hit;

			if(Physics.Raycast (ray,out hit))    //如果真的发生了碰撞，ray这条射线在hit点与别的物体碰撞了
			{
				allDisappearIndex.Clear ();
				checkSameColor = false;
				if (hit.collider.gameObject.tag == "Block") {
					Transform t = hit.collider.gameObject.transform;
					float px = t.localPosition.x - 0.5f;
					float pz = t.localPosition.z - 0.5f;
					int ix = Mathf.CeilToInt (px / 1);
					int iy = Mathf.CeilToInt(pz % B_Width);
					int index = ix * B_Width + iy;

					BlockBase bb = hit.collider.GetComponent<BlockBase> ();

					addDisappearIndex (index);
					findNeighbour (index, bb.getColorIndex (),0);
				}

				foreach (int element in allDisappearIndex) 
				{
//					Debug.LogFormat ("ele , {0}",element);
				}
				removeBlocks ();
				dropBlock ();
				leftMoveBlock ();
				findHorizontalConnect ();
				Debug.LogFormat ("same color {0}", checkSameColor);
			}
		}


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

		if (rightIndex < allBlocks.GetLength(1) && currentDir != 8) {
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

	//添加要消除的目标下标
	bool addDisappearIndex(int index){
		if (allDisappearIndex.IndexOf (index) == -1) {
			allDisappearIndex.Add (index);
			return true;
		}
		return false;
	}

	int getBlockColor(int index){
		return blockStates[0,index];
	}

	void removeBlocks(){
		if (allDisappearIndex.Count > 1) {
//			Debug.LogFormat ("remove {0}", allDisappearIndex.Count);
			foreach (int element in allDisappearIndex) 
			{
				int index = element;
				GameObject block = allBlocks [0,index];
				Destroy (block);
				blockStates [0,index] = -1;
			}
		}
	}

	//下落格子
	void dropBlock(){
		for (int i = 0; i < B_Width; i++) {
			int colum = i * B_Width;
			int empty = 0;
			int lastColor = -1;
			for (int j = 0; j < B_Width; j++) {
				int bIndex = colum + j;
				if (blockStates[0,bIndex] == -1) {
					empty++;
				} else {
					int temp = blockStates [0,bIndex];
					if (lastColor == temp) {
						Debug.LogFormat ("drop{0},{1}",bIndex,lastColor);
						checkSameColor = true;
					} else if(checkSameColor == false){
						lastColor = temp;
						Debug.LogFormat ("last{0},{1}",bIndex,lastColor);
					}
					if (empty > 0) {
//						Debug.LogFormat ("empty {0}", empty);
						//int temp = blockStates [bIndex];
						blockStates [0,bIndex] = -1;
						blockStates [0,bIndex - empty] = temp;
						BlockBase bBase = allBlocks[0,bIndex].GetComponent<BlockBase>();
						bBase.dropBlock (empty);
						GameObject block = allBlocks [0,bIndex];
						allBlocks [0,bIndex] = null;
						allBlocks [0,bIndex - empty] = block; 
					}
				}
			}
		}



	}

	//向左靠拢格子
	void leftMoveBlock(){
		int line = 0;
		int empty = 0;
		for (int j = 0; j < B_Width; j++) {
			int bIndex = line + j * B_Width;
			if (blockStates[0,bIndex] == -1) {
				empty++;
			} else {
				if (empty > 0) {
					//Debug.LogFormat ("empty {0}", empty);
					for (int i = 0; i < B_Width; i++) {
						int lIndex = bIndex + i;
						int temp = blockStates[0,lIndex];
						if (temp != -1) {
							blockStates[0,lIndex] = -1;
							blockStates[0,lIndex - empty * B_Width] = temp;
							BlockBase bBase = allBlocks[0,lIndex].GetComponent<BlockBase>();
							bBase.leftMoveBlock (empty);
							GameObject block = allBlocks [0,lIndex];
							allBlocks [0,lIndex] = null;
							allBlocks [0,lIndex - empty * B_Width] = block; 
						}
					}
				}
			}
		}
	}
	//横向搜索是否存在可以消除的格子
	void findHorizontalConnect(){
		for (int i = 0; i < B_Width; i++) {
			int lastColor = -1;
			for (int j = 0; j < B_Width; j++) {
				int bIndex = i + j * B_Width;
				int temp = blockStates[0,bIndex];
				if (lastColor == temp && temp != -1) {
					Debug.LogFormat ("hor{0},{1}",bIndex,lastColor);
					checkSameColor = true;
					return;
				} else {
					lastColor = temp;
				}
			}
		}
	}

}
