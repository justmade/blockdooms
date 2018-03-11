using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBase : MonoBehaviour {

	public Material[] _materials;

	private int colorIndex;

	public Vector3 targetPosition;
	// Use this for initialization
	void Awake(){
		int index = Mathf.FloorToInt(Random.Range(0f,_materials.Length));
		colorIndex = index;
	}

	void Start () {

		this.GetComponent<Renderer> ().material = _materials [colorIndex];

	}
	
	// Update is called once per frame
	void Update () {
//		if (targetPosition != Vector3.zero) {
//			Vector3 v = new Vector3 (0, 0, 0.2f);
//			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref v, 0.1f);
//		}

	}

	public int getColorIndex(){
		return colorIndex;
	}

	public void dropBlock(int distance){
		this.transform.position = this.transform.position - new Vector3 (0, 0, distance);
	} 

	public void leftMoveBlock(int distance){
		this.transform.position = this.transform.position - new Vector3 (distance, 0, 0);
	}
}
