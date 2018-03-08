using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBase : MonoBehaviour {

	public Material[] _materials;

	private int colorIndex;
	// Use this for initialization
	void Start () {
		int index = Mathf.FloorToInt(Random.Range(0f,_materials.Length));
		this.GetComponent<Renderer> ().material = _materials [index];
		colorIndex = index;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getColorIndex(){
		return colorIndex;
	}
}
