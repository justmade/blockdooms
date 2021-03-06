﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Assertions;
using System;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class BlockBase : MonoBehaviour {

	public Elements elements;

	private Material[] _materials;

	private int colorIndex;

	private bool isDrop;

	private bool isMove;

	private int dropDistance;

	private float moveDistance;

	private Vector3 lastP;


	private float totalMove = 10f;

	public AnimationCurve AniX;

	public AnimationCurve AniScale;

    private Vector3 lpos = Vector3.zero;

    public Vector3 targetPosition;

	public bool playAmplify = false;

	private float amplifyFrames = 0;

	public float amplifyTime = 40f;

	public float startTime = 20f;

	public bool isPlayingAimation = false;

	private GameObject centerBlock;

	private GameObject centerBodyBlock;

	private Renderer centerRender;

	private Elements elementConfig;

	public bool isSingleBlock;

	public int centreColor = -1;

	private float blockScale = 0.96f;

	private string aniState = "idle";

	private Vector3 moveTargetPos;

	private List<string> actionList;

	public delegate void RemoveBlockCB();

	public RemoveBlockCB removeBlockCB;

	// Use this for initialization
	void Awake(){
		if(elements != null){
			_materials = elements._materials;
			int index = Mathf.FloorToInt(UnityEngine.Random.Range(0f,_materials.Length));
			colorIndex = index;
		}else{
			colorIndex = 0;
		}
		elementConfig = new Elements();

		actionList = new List<string>();
	}

	public void setColor(int i , int centreIndex=-1){
		
		if (centreIndex == -1) {
			isSingleBlock = true;
			centreIndex = i;
			centerBodyBlock = AddCenterBodyBlock(centreIndex);
		} else {
			isSingleBlock = false;
			setCenterColor(centreIndex,-1);
			centreColor = centreIndex;
		}
		colorIndex = i;


		
	}

	private GameObject AddCenterBlock(int colorID){
		
		GameObject block;
		if(colorID == elementConfig.Treasure){
			block =	Instantiate(Resources.Load( "BlockRightCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (-blockScale, blockScale, blockScale);
		}else if(colorID == elementConfig.Key){
			block = Instantiate(Resources.Load( "BlockRightCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}else{
			block = Instantiate(Resources.Load( "VoxelBlackCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}	
		block.transform.parent = this.gameObject.transform;
		block.transform.position = this.gameObject.transform.position + new Vector3 (0,0.0f,0) ;
		return block;
	}


	private GameObject AddCenterBodyBlock(int colorID){
		GameObject block;
		if(colorID == elementConfig.Red){
			block =	Instantiate(Resources.Load( "RedCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}else if(colorID == elementConfig.Blue){
			block = Instantiate(Resources.Load( "BlueCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}else if(colorID == elementConfig.Yellow){
			block = Instantiate(Resources.Load( "YellowCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}else if(colorID == elementConfig.Green){
			block = Instantiate(Resources.Load( "GreenCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}else if(colorID == elementConfig.Treasure){
			block = Instantiate(Resources.Load( "LeftCenter2", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}else if(colorID == elementConfig.Key){
			block = Instantiate(Resources.Load( "RightCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}else if(colorID == elementConfig.White){
			block = Instantiate(Resources.Load( "WhiteCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}
		else{
			block = Instantiate(Resources.Load( "GreenCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		}
		// block.transform.localScale = new Vector3 (blockScale, blockScale, blockScale);
		block.transform.parent = this.gameObject.transform;
		block.transform.position = this.gameObject.transform.position + new Vector3 (0,0.1f,0) ;
		return block;
	}

	private void setCenterColor(int colorID,int BodyColor){

		if(colorID == -1){
			// AddCenterBodyBlock(BodyColor);
			return;
		}
		VoxelImporter.VoxelObject vo = AddCenterBlock(colorID).GetComponent<VoxelImporter.VoxelObject>();
		vo.GetComponent<Renderer>().materials[0].EnableKeyword("_EmissionColor");
		vo.GetComponent<Renderer>().materials[0].EnableKeyword("_Color");

		Color newColor; 
		if(colorID == elementConfig.Red){
			newColor = new Color(255/255f, 94/255f, 0/255f, 1f);
		}else if(colorID == elementConfig.Green){
			newColor = new Color(87/255f, 197/255f ,29/255f, 1f);
		}else if(colorID == elementConfig.Blue){
			newColor = new Color(0/255f, 67/255f, 178/255f, 1f);
		}else if(colorID == elementConfig.Yellow){
			newColor = new Color(234/255f, 183/255f, 0/255f, 1f);
		}else if(colorIndex == elementConfig.White){
			newColor = new Color(219/255f, 223/255f, 220/255f, 1f);
		}
		else{  
			newColor = Color.white;
		}
		vo.GetComponent<Renderer>().materials[0].color = newColor;
		vo.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", newColor);
		vo.GetComponent<Renderer>().materials[0].SetColor("_Color",newColor);
		centerRender = vo.GetComponent<Renderer>();
 	}


	private void setMaterial(Material mat , string cubeName){
		// Material material = new Material(mat);
		// material.CopyPropertiesFromMaterial (mat);
		// Transform centre = this.gameObject.transform.Find (cubeName);
		// gameObject.GetComponent<Renderer>().materials[2].SetColor("_EMISSION", new Color(0.0927F, 0.4852F, 0.2416F, 0.42F));
		VoxelImporter.VoxelObject vo = this.GetComponent<VoxelImporter.VoxelObject>();
		// vo.voxelFilePath = ;
		// vo.materialData[0].material = material;
		// vo.materials[0] = material;
		// vo.GetComponent
		// vo.GetComponent<Renderer>().materials[1].SetColor("_EMISSION", new Color(0.927F, 0.4852F, 0.2416F, 0.42F));
		vo.GetComponent<Renderer>().materials[1].EnableKeyword("_EmissionColor");
		vo.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", Color.yellow);
		vo.GetComponent<Renderer>().materials[1].color = Color.green;
		
	}


	public void setEditorColor(int i){
		_materials = elements._materials;
		Debug.LogFormat ("setColor {0}",i);
		Material material = new Material(Shader.Find("Transparent/Diffuse"));
		material.CopyPropertiesFromMaterial (_materials [i]);
		GetComponent<Renderer>().material = material;
		colorIndex = i;
	}

	void Start () {
//		Material material = new Material(Shader.Find("Transparent/Diffuse"));
//		material.color = Color.green;
//		//material.SetVector("_Color",new Vector4(1,1,1,1));
//		GetComponent<Renderer>().material = material;

	}
	
	// Update is called once per frame
	void Update () {
		// if(centerRender != null){
		// 	Color c = Color.Lerp(new Color(240/255f, 99/255f, 72/255f, 1f),Color.white, Mathf.PingPong(Time.time, 1));
		// 	centerRender.materials[0].color = c;
		// 	centerRender.materials[0].SetColor("_EmissionColor", c);
		// 	centerRender.materials[0].SetColor("_Color",c);	
			
		// }
		amplify ();
	}

	public void setPlayAmplify(bool _state){
		aniState = BlockAnimationState.AMPLIFY;
		playAmplify = _state;
		isPlayingAimation = true;
		actionList.Add(BlockAnimationState.AMPLIFY);
	}

	public void amplifyForBegin(){
		
		actionList.Add(BlockAnimationState.AMPLIFY);
	}

    public void elevate(float timeDelta) {
        //lpos = this.transform.localPosition;
        if (timeDelta == 1f) {
           lpos = this.transform.localPosition;
        }
        if (timeDelta <= totalMove)
        {
            this.transform.localPosition = lpos + new Vector3(0, AniX.Evaluate(timeDelta / totalMove) * 1,0);
           // Debug.Log(this.transform.localPosition);
        }
    }

	public void hideObject(){
		Color c = this.GetComponent<Renderer> ().sharedMaterial.color;
//		this.GetComponent<Renderer> ().material.color.a = 0f;
		gameObject.GetComponent<Renderer> ().sharedMaterial.color  = new Color(c.r,c.g,c.b,0.5f);
	}

	public void displayObject(){
		Color c = this.GetComponent<Renderer> ().sharedMaterial.color;
		gameObject.GetComponent<Renderer> ().sharedMaterial.color  = new Color(c.r,c.g,c.b,1.0f);
	}

	public void tapEffect(){
		String particleName = "BlockTapBlue";
		bool needBoomEffect = false;
		if(isSingleBlock){
			if(colorIndex == elementConfig.Red){
				particleName = "BlockTapRed";
			}else if(colorIndex == elementConfig.Blue){
				particleName = "BlockTapBlue";
			}else if(colorIndex == elementConfig.Yellow){
				particleName = "BlockTapYellow";
			}else if(colorIndex == elementConfig.Green){
				particleName = "BlockTapGreen";
			}else if(colorIndex == elementConfig.Key){
				particleName = "BlockTapRight";
				needBoomEffect = true;
			}else if(colorIndex == elementConfig.Treasure){
				particleName = "BlockTapLeft";
				needBoomEffect = true;
			} 
		}else{
			if(colorIndex == elementConfig.Key){
				particleName = "BlockTapTreasure";
				needBoomEffect = true;
			}else if(colorIndex == elementConfig.Treasure){
				particleName = "BlockTapTreasure";
				needBoomEffect = true;
			}else{
				particleName = "BlockTap";
			}
			
		}
		GameObject pEffect = Instantiate(Resources.Load("particle/"+particleName, typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
		pEffect.transform.parent = this.gameObject.transform;
		pEffect.transform.position = this.gameObject.transform.position + new Vector3 (0,0.3f,0) ;
		ParticleSystem p = pEffect.GetComponent<ParticleSystem> ();
		p.Play ();
		aniState = BlockAnimationState.TAP;
		Destroy(p,0.5f); 
		DestroyBlock(needBoomEffect);
	}

	//被删除的block 不在数组中需要另外的回调
	public void setRemoveCB(RemoveBlockCB cb){
		this.removeBlockCB = cb;
	}

	public void DestroyBlock(Boolean needBoomEffect){
		StartCoroutine(DestroyByTime());
		if(needBoomEffect)
			StartCoroutine(addBoomParticle(colorIndex));
	}

	public void colorConvertEfx(){
		String particleName = "ChangeEffect";
		GameObject pEffect = Instantiate(Resources.Load("particle/"+particleName, typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
		pEffect.transform.parent = this.gameObject.transform;
		pEffect.transform.position = this.gameObject.transform.position + new Vector3 (0,0.3f,0) ;
		ParticleSystem p = pEffect.GetComponent<ParticleSystem> ();
		p.Play ();
		// GameObject q = Instantiate(Resources.Load("particle/CircleExpendEfx", typeof( GameObject ) ), new Vector3 (10,10,10), Quaternion.Euler (90f, 0f, 0f)) as GameObject;
		// q.transform.parent = this.gameObject.transform;
		// q.transform.position = this.gameObject.transform.position + new Vector3 (0,1.3f,0) ;
		// q.transform.localScale = new Vector3 (5.5f, 5.5f,5.5f);
	}
	
	IEnumerator DestroyByTime(){
		yield return new WaitForSeconds(0.5f);
		if( removeBlockCB != null ){
			this.removeBlockCB();
		}
		aniState = BlockAnimationState.IDLE;
		Destroy(this.gameObject);
	}

public void DestroyImmediately(){
		aniState = BlockAnimationState.IDLE;
		Destroy(this.gameObject);
	}


	IEnumerator addBoomParticle(int color){
		yield return new WaitForSeconds(0.5f);
		GameObject pEffect = Instantiate(Resources.Load("particle/BlockBoom", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
		pEffect.transform.parent = this.gameObject.transform.parent.transform;
		pEffect.transform.position = this.gameObject.transform.position + new Vector3 (0,1f,0) ;
		ParticleSystem p = pEffect.GetComponent<ParticleSystem> ();
		pEffect.GetComponent<Renderer>().material.color = getColorByID(color);
		p.Play ();
		// this.gameObject.h
		Destroy(pEffect,1.0f);
		
	}



	Color getColorByID(int colorID){
		Color newColor;
		if(colorID == elementConfig.Red){
			newColor = new Color(255/255f, 94/255f, 0/255f, 1f);
			Debug.Log(newColor.ToString());
		}else if(colorID == elementConfig.Green){
			newColor = new Color(87/255f, 197/255f ,29/255f, 1f);
		}else if(colorID == elementConfig.Blue){
			newColor = new Color(0/255f, 67/255f, 178/255f, 1f);
		}else if(colorID == elementConfig.Yellow){
			newColor = new Color(234/255f, 183/255f, 0/255f, 1f);
		}else if(colorIndex == elementConfig.Key){
			newColor = new Color(225/255f, 159/255f, 135/255f, 1f);
		}else if(colorIndex == elementConfig.Treasure){
			newColor = new Color(192/255f, 108/255f, 132/255f, 1f);
		}else if(colorIndex == elementConfig.White){
			newColor = new Color(219/255f, 223/255f, 220/255f, 1f);
		}
		else{
			newColor = Color.white;
		}
		return newColor;
	}

	public void amplify(){
		if (playAmplify) {
			isPlayingAimation = true;
			amplifyFrames++;

			if(amplifyFrames <= startTime){
				centerBodyBlock.transform.parent = this.gameObject.transform;
				this.transform.localScale =  new Vector3 (0.4f, 0.4f, 0.4f);
				this.centerBodyBlock.transform.localScale =  new Vector3 (1f, 1f, 1f);
				centerBodyBlock.transform.position = this.gameObject.transform.position + new Vector3 (0,0.05f,0) ;
			}	
			else if (amplifyFrames < amplifyTime && amplifyFrames > startTime) {
				float value = 0.40f + AniScale.Evaluate ((amplifyFrames-startTime) / (amplifyTime-startTime)) * 0.56f;
				this.transform.localScale =  new Vector3 (value, value, value);
			}else if (amplifyFrames == amplifyTime) {
				centerBodyBlock.transform.localScale =  new Vector3 (1f, 1f, 1f);
				amplifyFrames = 0;
				playAmplify = false;
				isPlayingAimation = false;
				aniState = BlockAnimationState.IDLE;

				actionList.RemoveAt(0);
			}
		}
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

	public void horizontalMove(int distance){
		
		moveDistance = distance *1.2f;
		actionList.Add(BlockAnimationState.MOVE);
		// this.transform.position = this.transform.position + new Vector3 (moveDistance, 0, 0	);
		moveTargetPos = this.transform.position + new Vector3 (moveDistance, 0, 0);
	}

	public void verticalMoving(int distance){
		moveDistance = distance *1.2f;
		actionList.Add(BlockAnimationState.MOVE);
		moveTargetPos = this.transform.position + new Vector3 (0, 0, moveDistance);
	}

	public void startBlockMove(){
		if(actionList.Count > 0 && actionList[0]==BlockAnimationState.MOVE){
			aniState = BlockAnimationState.MOVE;
			this.transform.DOJump(moveTargetPos,1f,1,0.1f).SetEase(Ease.OutSine).OnComplete(moveComplete);	
		}
	}

	private void moveComplete(){
		aniState = BlockAnimationState.IDLE;
		actionList.RemoveAt(0);
	}

	public string getActionList(){
		return aniState;
	}

	public string getCurrentAction(){
		if(actionList.Count > 0){
			return actionList[0];

		}
		return BlockAnimationState.IDLE;
	}

	public string getAniState(){
		return aniState;
	}

}
