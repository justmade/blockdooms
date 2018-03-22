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

	private float timeDelta = 0f;

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
		if (isDrop) {
			if (timeDelta <= totalMove) {
				this.transform.position = lastP - new Vector3 (0, 0, AniX.Evaluate (timeDelta / totalMove) * dropDistance);
				timeDelta += 1f;
			} else {
				isDrop = false;
				timeDelta = 0f;
				lastP = this.transform.position;
			}
		} else if(isMove){
			if (timeDelta <= totalMove) {
				this.transform.position = lastP - new Vector3 (AniX.Evaluate (timeDelta / totalMove) * moveDistance, 0,0);
				timeDelta += 1f;
			} else {
				isMove = false;
				timeDelta = 0f;
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
