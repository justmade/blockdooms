using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject m_BlockPrefabs;

	public List<GameObject> allBlocks;

	public List<int> allDisappearIndex;

	// Use this for initialization
	void Start () {
		initAllBlock ();
	}
	
	// Update is called once per frame
	void Update () {
		touchBlock ();
	}

	void initAllBlock(){
		allBlocks = new List<GameObject> ();
		Vector3 v = new Vector3 (0,1,0);
		Quaternion turnRotation= Quaternion.Euler (0f, 0f, 0f);
		GameObject container = GameObject.Find("BlockContainer");
		for (int i = 0; i < 25; i++){			
			v.x = Mathf.Ceil (i / 5) * 1.0f + 0.5f;
			v.z = i%5 * 1.0f+0.5f ;
			// ... create them, set their player number and references needed for control.
			GameObject block = 
				Instantiate(m_BlockPrefabs, v, turnRotation) as GameObject;
			allBlocks.Add (block);
			block.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			block.tag = "Block";
			block.transform.parent = container.transform;
		}

		container.transform.position = new Vector3(-(5 * 1.0f) / 2,0,-(5 * 1.0f) / 2);
	}

	void touchBlock(){
		if (Input.GetMouseButton(0)) {

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
			RaycastHit hit;

			if(Physics.Raycast (ray,out hit))    //如果真的发生了碰撞，ray这条射线在hit点与别的物体碰撞了
			{
				allDisappearIndex.Clear ();
				if (hit.collider.gameObject.tag == "Block") {
					Transform t = hit.collider.gameObject.transform;
					float px = t.localPosition.x - 0.5f;
					float pz = t.localPosition.z - 0.5f;
					int ix = Mathf.CeilToInt (px / 1);
					int iy = Mathf.CeilToInt(pz % 5);
					int index = ix * 5 + iy;

					BlockBase bb = hit.collider.GetComponent<BlockBase> ();

					addDisappearIndex (index);
					findNeighbour (index, bb.getColorIndex (),0);
				}

				foreach (int element in allDisappearIndex) 
				{
					Debug.Log ("element:");
					Debug.Log (element);

				}


			}
		}


	}



	void findNeighbour(int index , int color , int currentDir)
	{
		int rightIndex = index + 5;

		int leftIndex = index - 5;

		int downIndex = -1;
		if (index % 5 != 0) {
			downIndex = index - 1;
		}

		int upIndex = -1;
		if ((index + 1) % 5 != 0) {
			upIndex = index + 1;
		}

		if (rightIndex < allBlocks.Count && currentDir != 8) {
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

	bool addDisappearIndex(int index){
		if (allDisappearIndex.IndexOf (index) == -1) {
			allDisappearIndex.Add (index);
			return true;
		}
		return false;
	}

	int getBlockColor(int index){
		BlockBase block = allBlocks[index].GetComponent<BlockBase> ();
		return block.getColorIndex ();
	}





}
