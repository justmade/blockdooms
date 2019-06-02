using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SagaMapScene : MonoBehaviour {

	public Camera mainCamera;

	private GameObject LevelContainer;

	void Start () {
		createLevels();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp(0)) {
			touchLevel();
		}
	}

	void createLevels(){
		LevelContainer = GameObject.Find("Sphere");
		float R = 20;
		for(int i=0;i<12;i ++){
			float fx = Mathf.Sin(Mathf.PI * 2 * i / 12);
			float angle1 = (30+i*10) * Mathf.PI / 180;
			float angle2 = (90 + fx * 30) * Mathf.PI / 180;
			float px = R * Mathf.Sin(angle1) * Mathf.Cos(angle2);
			float py = R * Mathf.Sin(angle1) * Mathf.Sin(angle2);
			float pz = R * Mathf.Cos(angle1);


			Quaternion turnRotation=  Quaternion.LookRotation(new Vector3(px,py,pz),Vector3.up);
			GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
			g.transform.position = new Vector3(px,py,pz);
			g.transform.rotation = turnRotation;
			// g.transform.parent = LevelContainer.transform;
			g.tag = "LevelBlock";


		}
	}

	void touchLevel(){
		Ray ray;
		ray = mainCamera.ScreenPointToRay (Input.mousePosition);  
		RaycastHit hit;
		if(Physics.Raycast (ray,out hit))    //如果真的发生了碰撞，ray这条射线在hit点与别的物体碰撞了
		{
			if (hit.collider.gameObject.tag == "LevelBlock") {
				mainCamera.GetComponent<CameraOrbit>().setCameraTarget(hit.collider.gameObject.transform);
			}
		}
	}
}
