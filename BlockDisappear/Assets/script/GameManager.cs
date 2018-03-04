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
		
	}

	void initAllBlock(){
		allBlocks = new List<GameObject> ();
		Vector3 v = new Vector3 (0,1,0);
		Quaternion turnRotation= Quaternion.Euler (0f, 0f, 0f);
		for (int i = 0; i < 9; i++)
		{
			v.x = Mathf.Ceil (i / 3) * 1.1f;
			v.z = i%3 * 1.1f ;
			// ... create them, set their player number and references needed for control.
			GameObject block = 
				Instantiate(m_BlockPrefabs, v, turnRotation) as GameObject;
			allBlocks.Add (block);
			block.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);

		}	
	}
}
