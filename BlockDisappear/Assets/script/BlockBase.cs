using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBase : MonoBehaviour {

	public Material[] _materials;

	private int colorIndex;

	private bool isDrop;

	private bool isMove;

	private int dropDistance;

	private int moveDistance;

	private Vector3 lastP;


	private float totalMove = 10f;

	public AnimationCurve AniX;

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

	public void drop(float timeDelta){
		if (isDrop) {
			if (timeDelta <= totalMove) {
				this.transform.position = lastP - new Vector3 (0, 0, AniX.Evaluate (timeDelta / totalMove) * dropDistance);
				if (timeDelta == totalMove) {
					isDrop = false;
					lastP = this.transform.position;
				}
			} 
		} 
	}

	public void move(float timeDelta){
		if(isMove){
			if (timeDelta <= totalMove) {
				this.transform.position = lastP - new Vector3 (AniX.Evaluate (timeDelta / totalMove) * moveDistance, 0,0);
				if (timeDelta == totalMove) {
					isMove = false;
				}
			} 
		}
	}

	public int getColorIndex(){
		return colorIndex;
	}

	public void dropBlock(int distance){
		isDrop = true;
		dropDistance = distance;
		//this.transform.position = this.transform.position - new Vector3 (0, 0, distance);
		lastP = this.transform.position;
	} 

	public void leftMoveBlock(int distance){
		isMove = true;
		moveDistance = distance;
		if (!isDrop) {
			lastP = this.transform.position;
		}
//		this.transform.position = this.transform.position - new Vector3 (distance, 0, 0	);
	}


}
