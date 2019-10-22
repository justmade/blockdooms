using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SagaMapScene : MonoBehaviour {

	public Camera mainCamera;

	private GameObject LevelContainer;

	[SerializeField] private LevelController levelController;

	private GameObject[] allLevelBlock;

	//是否检测点击
	private bool touchEnable = true;

	private bool couldTouch = true;

	private int totalLevels = 12;
	void Start () {
		createLevels();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)  && !touchEnable){
			couldTouch = false;
		}else if(Input.GetMouseButtonDown(0)  && touchEnable){
			couldTouch = true;
		}
		if (Input.GetMouseButtonUp(0) && couldTouch) {
			touchLevel();
		}
	}

	void createLevels(){
		LevelContainer = GameObject.Find("Sphere");
		allLevelBlock = new GameObject[totalLevels];
		float R = 20;
		for(int i=0;i<totalLevels;i ++){
			float fx = Mathf.Sin(Mathf.PI * 2 * i / 12);
			float angle1 = (30+i*10) * Mathf.PI / 180;
			float angle2 = (90 + fx * 30) * Mathf.PI / 180;
			float px = R * Mathf.Sin(angle1) * Mathf.Cos(angle2);
			float py = R * Mathf.Sin(angle1) * Mathf.Sin(angle2);
			float pz = R * Mathf.Cos(angle1);
			Level level = levelController.levels[i];
			string metalName = "DefaultLevelsBt";
			if (level.Stars > 0 ){
				metalName = "PassedLevelsBt";
			}

			Quaternion turnRotation=  Quaternion.LookRotation(new Vector3(px,py,pz),Vector3.up);
			// GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
			// GameObject g =	Instantiate(Resources.Load( "UFO_level", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			GameObject g =	Instantiate(Resources.Load( metalName, typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			// block.transform.localScale = new Vector3 (-blockScale, blockScale, blockScale);
			g.transform.position = new Vector3(px,py,pz);
			g.transform.rotation = turnRotation;
			// g.transform.parent = LevelContainer.transform;
			g.tag = "LevelBlock";
			allLevelBlock[i]=g;

		}
		initCameraPos();

	}

	void initCameraPos(){
		// for(int i= 0 ;i <LevelDataInfo.levels.Count;i++){
		// 	Level level = LevelDataInfo.levels[i];
		// 	if (level.LevelName == LevelDataInfo.selectLevelName){
		// 		Debug.LogFormat("selectLevelName222 {0}",level.LevelName);
		// 		Debug.LogFormat("cur {0}",i);
		// 		mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[i].transform,i,true);
		// 		mainCamera.GetComponent<CameraOrbit>().setTouchEnableState(OpenTouchEnable);
		// 		return;
		// 	}
		// }
		if(LevelDataInfo.selectLevelIndex > 0){
			mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[LevelDataInfo.selectLevelIndex-1].transform,
				LevelDataInfo.selectLevelIndex-1,true,true);

			mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[LevelDataInfo.selectLevelIndex].transform,
				LevelDataInfo.selectLevelIndex,true);
		}else if(LevelDataInfo.selectLevelIndex == 0 ){
			mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[LevelDataInfo.selectLevelIndex].transform,
				LevelDataInfo.selectLevelIndex,true,true);
		}

		

		mainCamera.GetComponent<CameraOrbit>().setTouchEnableState(OpenTouchEnable);
	}

	void OpenTouchEnable(){
		touchEnable = true;
	}

	void touchLevel(){
		Ray ray;
		ray = mainCamera.ScreenPointToRay (Input.mousePosition);  
		RaycastHit hit;
		if(Physics.Raycast (ray,out hit))    //如果真的发生了碰撞，ray这条射线在hit点与别的物体碰撞了
		{
			if (hit.collider.gameObject.tag == "LevelBlock") {
				int hitIndex = 0;
				for(int i=0;i<totalLevels;i ++){
					if(allLevelBlock[i] == hit.collider.gameObject){
						hitIndex = i;
					}
				}
				touchEnable = !mainCamera.GetComponent<CameraOrbit>().setCameraTarget(hit.collider.gameObject.transform,hitIndex);
			}
		}
	}
}
