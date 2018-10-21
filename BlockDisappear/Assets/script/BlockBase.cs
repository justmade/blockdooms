using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Awake(){
		_materials = elements._materials;
		int index = Mathf.FloorToInt(Random.Range(0f,_materials.Length));
		colorIndex = index;
	}

	public void setColor(int i , int centreIndex=-1){
		
		// if (centreIndex == -1) {
		// 	centreIndex = i;
		// 	Transform centre = this.gameObject.transform.Find ("Cube");
		// 	centre.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		// } else {
		// 	Transform centre = this.gameObject.transform.Find ("Cube");
		// 	centre.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		// }


		// setMaterial (_materials [centreIndex], "Cube");
		// setMaterial (_materials [i], "Cube_Up");
		// setMaterial (_materials [i], "Cube_Down");
		// setMaterial (_materials [i], "Cube_Right");
		// setMaterial (_materials [i], "Cube_Left");
		colorIndex = i;
	}

	private void setMaterial(Material mat , string cubeName){
		Material material = new Material(mat);
		material.CopyPropertiesFromMaterial (mat);
		Transform centre = this.gameObject.transform.Find (cubeName);
		centre.gameObject.GetComponent<Renderer> ().material = material;
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
