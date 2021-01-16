using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SagaMapScene : MonoBehaviour {

	public Camera mainCamera;

	private GameObject LevelContainer;

	[SerializeField] private LevelController levelController;

	private GameObject[] allLevelBlock;

	//是否检测点击
	private bool touchEnable = true;

	private bool couldTouch = true;

	private int totalLevels = 12;

	private float UFOMoveSpeed = 12f;

	private Vector3 targetPos = new Vector3();
	private Quaternion turnRotation = new Quaternion();


    private GameObject UFO; 

	void Start () {
		initUFO();
		createLevels();
		
	}


    void initUFO(){
        UFO = Instantiate(Resources.Load( "UFO_level", typeof( GameObject ) ), 
		new Vector3 (-4.3f,11.6f,17.32f), Quaternion.Euler (326.2f, 0f, 5.34f)) as GameObject;
		UFOMoveSpeed = mainCamera.GetComponent<CameraOrbit>().MoveSpeed;

		if(!LevelDataInfo.tutorFinished){
			this.GetComponent<GuideDialogue>().setGameUFO(UFO);
		}else{
			this.GetComponent<GuideDialogue>().enableDialogue();
			this.GetComponent<GuideDialogue>().enabled = false;
		}
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
		initLevelBlocks();
		initCameraPos();
		mainCamera.GetComponent<CameraOrbit>().setLevelObjectCB(updateLevelBlocks);
	}



	void updateLevelBlocks(){
		for(int i=0;i<allLevelBlock.Length;i ++){
			Destroy(allLevelBlock[i]);
		}
		initLevelBlocks();
		LevelDataInfo.SelectLevelIndex = 0;
		initCameraPos();
	}


	void initLevelBlocks(){
		allLevelBlock = new GameObject[totalLevels];
		float R = 20;
		for(int i=0;i<totalLevels;i ++){
			float fx = Mathf.Sin(Mathf.PI * 2 * i / 12);
			float angle1 = (30+i*10) * Mathf.PI / 180;
			float angle2 = (90 + fx * 30) * Mathf.PI / 180;
			float px = R * Mathf.Sin(angle1) * Mathf.Cos(angle2);
			float py = R * Mathf.Sin(angle1) * Mathf.Sin(angle2);
			float pz = R * Mathf.Cos(angle1);
			Level level = levelController.levels[i + LevelDataInfo.chapter * 12];
			string metalName = "DefaultLevelsBt";
			if (level.Stars > 0 ){
				metalName = "PassedLevelsBt";
			}
			Quaternion turnRotation=  Quaternion.LookRotation(new Vector3(px,py,pz),Vector3.up);
			GameObject g =	Instantiate(Resources.Load( metalName, typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			g.transform.position = new Vector3(px,py,pz);
			g.transform.rotation = turnRotation;
			g.tag = "LevelBlock";
			allLevelBlock[i]=g;
		}
	}


	void initCameraPos(){
		Debug.LogFormat("SelectLevelIndex{0}",LevelDataInfo.SelectLevelIndex);
		if(LevelDataInfo.SelectLevelIndex == 12 && LevelDataInfo.chapter < LevelDataInfo.planetList.Length-1){
			mainCamera.GetComponent<CameraOrbit>().nextPlanet();
			
			mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[0].transform,
				0,false,true);
			getUFOTargetPos(0);
			setUFOPos(targetPos,turnRotation,false);

			
		}else if(LevelDataInfo.SelectLevelIndex > 0){
			mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[LevelDataInfo.SelectLevelIndex-1].transform,
				LevelDataInfo.SelectLevelIndex-1,false,true);
			getUFOTargetPos(LevelDataInfo.SelectLevelIndex-1);
			setUFOPos(targetPos,turnRotation,true);

			mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[LevelDataInfo.SelectLevelIndex].transform,
				LevelDataInfo.SelectLevelIndex,false);
			getUFOTargetPos(LevelDataInfo.SelectLevelIndex);
			setUFOPos(targetPos,turnRotation,false);

		}else if(LevelDataInfo.SelectLevelIndex == 0 ){
			mainCamera.GetComponent<CameraOrbit>().setCameraTarget(allLevelBlock[LevelDataInfo.SelectLevelIndex].transform,
				LevelDataInfo.SelectLevelIndex,true,true);

			getUFOTargetPos(LevelDataInfo.SelectLevelIndex);
			setUFOPos(targetPos,turnRotation,true);
			Debug.Log(targetPos);
		}

		

		mainCamera.GetComponent<CameraOrbit>().setTouchEnableState(OpenTouchEnable);
	}

	void getUFOTargetPos(int _index){
		targetPos = Vector3.up * 1.6f + allLevelBlock[_index].transform.position;
		turnRotation = Quaternion.LookRotation(targetPos,Vector3.up);
	}

	void setUFOPos(Vector3 targetPos ,Quaternion turnRotation,bool isImmediate){
		float journeyLength =  Vector3.Distance(targetPos, UFO.transform.position);
		float dur = journeyLength/UFOMoveSpeed;
		if(isImmediate){
			UFO.transform.position = targetPos;
			UFO.transform.rotation = turnRotation;
		}else{
			UFO.transform.DOJump(targetPos,1f,1,dur).SetEase(Ease.OutSine);
			UFO.transform.DORotateQuaternion(turnRotation,dur);
		}
		
		
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
						break;
					}
				}
				getUFOTargetPos(hitIndex);
				setUFOPos(targetPos,turnRotation,false);

				touchEnable = !mainCamera.GetComponent<CameraOrbit>().setCameraTarget(hit.collider.gameObject.transform,hitIndex);
			}
		}
	}
}
