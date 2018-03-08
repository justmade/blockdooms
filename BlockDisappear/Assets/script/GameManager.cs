using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject m_BlockPrefabs;

	public List<GameObject> allBlocks;

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
				if (hit.collider.gameObject.tag == "Block") {
					Transform t = hit.collider.gameObject.transform;
					float px = t.localPosition.x - 0.5f;
					float pz = t.localPosition.z - 0.5f;
					int ix = Mathf.CeilToInt (px / 1);
					int iy = Mathf.CeilToInt(pz % 5);
					int index = ix * 5 + iy;
					Debug.Log (index);

					BlockBase bb = hit.collider.GetComponent<BlockBase> ();

					Debug.Log (bb.getColorIndex ());
				}
				

			}
		}

	}
}
