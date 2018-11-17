using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Assertions;
using System;
using System.IO;

public class BlockBase : MonoBehaviour {

	public Elements elements;

	private Material[] _materials;

	private int colorIndex;

	private bool isDrop;

	private bool isMove;

	private int dropDistance;

	private int moveDistance;

	private Vector3 lastP;


	private float totalMove = 10f;

	public AnimationCurve AniX;

	public AnimationCurve AniScale;

    private Vector3 lpos = Vector3.zero;

    public Vector3 targetPosition;

	public bool playAmplify = false;

	private float amplifyFrames = 0;

	private float amplifyTime = 20f;

	public bool isPlayingAimation = false;

	private GameObject centerBlock;

	// Use this for initialization
	void Awake(){
		if(elements != null){
			_materials = elements._materials;
			int index = Mathf.FloorToInt(UnityEngine.Random.Range(0f,_materials.Length));
			colorIndex = index;
		}else{
			colorIndex = 0;
		}
	}

	public void setColor(int i , int centreIndex=-1){
		
		if (centreIndex == -1) {
			centreIndex = i;
			setCenterColor(-1);
		} else {
			setCenterColor(centreIndex);
		}
		colorIndex = i;
	}

	private GameObject AddCenterBlock(int colorID){
		Elements elementConfig = new Elements();
		GameObject block;
		if(colorID == elementConfig.Treasure){
			block =	Instantiate(Resources.Load( "VoxelBlackTLeft", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (1/3f, 1/3f, 1/3f);
		}else if(colorID == elementConfig.Key){
			block = Instantiate(Resources.Load( "VoxelBlackTRight", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
			block.transform.localScale = new Vector3 (1/3f, 1/3f, 1/3f);
		}else{
			block = Instantiate(Resources.Load( "VoxelBlackCenter", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
		block.transform.localScale = new Vector3 (1f, 1f, 1f);
		}	
		block.transform.parent = this.gameObject.transform;
		block.transform.position = this.gameObject.transform.position + new Vector3 (0,0.2f,0) ;
		return block;
	}

	private void setCenterColor(int colorID){

		if(colorID == -1){
			return;
		}
		VoxelImporter.VoxelObject vo = AddCenterBlock(colorID).GetComponent<VoxelImporter.VoxelObject>();
		Elements elementConfig = new Elements();
		vo.GetComponent<Renderer>().materials[0].EnableKeyword("_EmissionColor");
		vo.GetComponent<Renderer>().materials[0].EnableKeyword("_Color");

		Color newColor; 
		if(colorID == elementConfig.Red){
			newColor = new Color(240/255f, 92/255f, 66/255f, 1f);
			Debug.Log(newColor.ToString());
		}else if(colorID == elementConfig.Green){
			newColor = new Color(0/255f, 132/255f ,83/255f, 1f);
		}else if(colorID == elementConfig.Blue){
			newColor = new Color(59/255f, 85/255f, 120/255f, 1f);
		}else if(colorID == elementConfig.Yellow){
			newColor = new Color(240/255f, 120/255f, 24/255f, 1f);
		}else{
			newColor = Color.white;
		}
		vo.GetComponent<Renderer>().materials[0].color = newColor;
		vo.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", newColor);
		vo.GetComponent<Renderer>().materials[0].SetColor("_Color",newColor);
		
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
		amplify ();
	}

	public void setPlayAmplify(bool _state){
		playAmplify = _state;
		isPlayingAimation = true;
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

	public void amplify(){
		if (playAmplify) {
			isPlayingAimation = true;
			amplifyFrames++;
			if (amplifyFrames <= amplifyTime) {
				float value = 0.5f + AniScale.Evaluate (amplifyFrames / amplifyTime) * 0.5f;
				this.transform.localScale =  new Vector3 (value, value, value);
			}
			if (amplifyFrames == amplifyTime) {
				amplifyFrames = 0;
				playAmplify = false;
				isPlayingAimation = false;
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

	public void leftMoveBlock(int distance){
		isMove = true;
		moveDistance = distance;
		if (!isDrop) {
			lastP = this.transform.position;
		}
//		this.transform.position = this.transform.position - new Vector3 (distance, 0, 0	);
	}


}
