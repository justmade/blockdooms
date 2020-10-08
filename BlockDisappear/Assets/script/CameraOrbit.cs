using UnityEngine;
using System.Collections;
using Doozy.Engine.UI;

public class CameraOrbit : MonoBehaviour 
{
    [SerializeField] ReadyPanelPopup readyPanel;
    
    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;

    protected Vector3 _LocalRotation;
    protected float _CameraDistance = 5f;

    public float MouseSensitivity = 4f;
    public float ScrollSensitvity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;
    public float MoveSpeed = 12f;

    public bool CameraDisabled = false;

    protected Transform targetTsf =null ;

    public Transform[] views;

    public UIView rPanel;

    private float startTime;

    private float journeyLength;

    private Vector3 startPos;

    private Vector3 startEular;

    private float fracJourney = 1.0f;
    
    private int currengLevelIndex = -1;

    private bool notOpenPanel = false;

    //设置点击的回调
    public delegate void VoidDelegate();
    VoidDelegate setTouch;

    public delegate void VoidLevelObjDelegate();
    VoidLevelObjDelegate levelBlockCB;

    private Material _mainMaterial;

    private bool startChange = false;
    private int maskAmplify = 1;
    private float maskWaiting = 0.8f;
    private float maskWaitingTime = 0.8f;
    private GameObject myPlanet;

    // Use this for initialization
    void Start() {
        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;
        readyPanel.gameObject.SetActive(false);

        initShader();
        initPlanet();
    }
     public int _smoothLength = 20;
    //遮罩混合颜色
    public Color _darkColor = Color.black;

    protected Camera _mainCamera;

    Vector4[] _itemDatas;
    int _tmpScreenHeight;
    float maskRadius = Screen.width/2;

    void initShader(){
         _mainMaterial = new Material(Shader.Find("MyShader/DarkEffect"));
         _mainCamera = GetComponent<Camera>();

    }

    void initPlanet(){
        string planetName = LevelDataInfo.planetList[LevelDataInfo.chapter];
        myPlanet = Instantiate(Resources.Load( "PlanetObjs/"+planetName, typeof( GameObject ) ), 
            new Vector3 (0,0,0), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
        myPlanet.transform.localScale = new Vector3 (0.17f, 0.17f, 0.17f);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(startChange == false){
            Graphics.Blit(source, destination);
            return;
        }
        _tmpScreenHeight = Screen.height;

        float px = _mainCamera.WorldToScreenPoint(new Vector3(0,0,0)).x;
        float py = _tmpScreenHeight/2;
        _itemDatas = new Vector4[1];
        _itemDatas[0] = new Vector4(px,py,maskRadius,0f);

        _mainMaterial.SetInt("_SmoothLength", _smoothLength);
        _mainMaterial.SetColor("_DarkColor", _darkColor);
        _mainMaterial.SetInt("_ItemCnt", 1);
        _mainMaterial.SetVectorArray("_Item", _itemDatas);

        Graphics.Blit(source, destination, _mainMaterial);
    }


    public bool setCameraTarget(Transform _targetTransform,int _hitIndex,bool _notOpenPanel=false, bool isImmediate = false){
        notOpenPanel = _notOpenPanel;
        if(readyPanel.gameObject.activeSelf){
            return false;
        }
        if(currengLevelIndex != _hitIndex){
            currengLevelIndex = _hitIndex;
        }else{
            PopupReadyPanel(currengLevelIndex+1);
            return true;
        }
        // targetTsf = _targetTransform ;
        Debug.LogFormat ("_hitIndex {0}", _hitIndex);
        targetTsf = views[_hitIndex];
        Debug.LogFormat ("targetTsf {0}", targetTsf.position);
        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;
        startTime = Time.time;
        journeyLength =  Vector3.Distance(this.transform.parent.transform.position , targetTsf.position);
        startPos = this.transform.parent.transform.position;
        startEular = this.transform.parent.transform.rotation.eulerAngles;
        if(isImmediate){
            setCameraPosImmediately();
        }else{
            fracJourney = 0.0f;
        }
        return true;
    }

    void PopupReadyPanel(int level){
        if(!notOpenPanel){
            rPanel.gameObject.SetActive(true);
            rPanel.GetComponent<ReadyPanelPopup>().setLevel(level);
            // rPanel.setLevel(level);
            rPanel.Show();
            // readyPanel.gameObject.SetActive(true);
            // readyPanel.setLevel(level);
        }
    }

    public void StartLevel(){
        Debug.LogFormat ("StartLevel {0}", "StartLevel");
        // readyPanel.gameObject.SetActive(false);
        setTouch();
        // readyPanel.startLevel();
        rPanel.Hide();
        rPanel.GetComponent<ReadyPanelPopup>().startLevel();
        
    }

    public void CloseReadyPanel(){
        Debug.LogFormat ("CloseReadyPanel {0}", "CloseReadyPanel");
        // readyPanel.gameObject.SetActive(false);
        rPanel.Hide();
        setTouch();
        // gameObject.SendMessage
    }

    public void setTouchEnableState(VoidDelegate callback){
        setTouch = callback;
    }

    public void setLevelObjectCB(VoidLevelObjDelegate callback){
        levelBlockCB = callback;
    }


    public void setCameraPosImmediately(){
         this.transform.parent.transform.position = Vector3.Lerp(startPos, targetTsf.position,1);
            Vector3 currentAngle = new Vector3(
                Mathf.LerpAngle(startEular.x, targetTsf.rotation.eulerAngles.x, 1),
                Mathf.LerpAngle(startEular.y, targetTsf.rotation.eulerAngles.y, 1),
                Mathf.LerpAngle(startEular.z, targetTsf.rotation.eulerAngles.z, 1));

            this.transform.parent.transform.eulerAngles = currentAngle;

            this._XForm_Camera.localPosition = new Vector3(0f, 0f,  this._CameraDistance * -1f);
        
    }

    public void nextPlanet(){
        if(!startChange && LevelDataInfo.chapter < LevelDataInfo.planetList.Length-1){
            LevelDataInfo.chapter ++;
            LevelDataInfo.chapter = Mathf.Min(LevelDataInfo.chapter,4);
            Debug.LogFormat("nextLevelDataInfo.chapter {0}",LevelDataInfo.chapter);
            changeLevelScene();
        }

    }

    public void perPlanet(){
        if(!startChange && LevelDataInfo.chapter > 0 ){
            LevelDataInfo.chapter --;
            LevelDataInfo.chapter = Mathf.Max(LevelDataInfo.chapter,0);
            Debug.LogFormat("perLevelDataInfo.chapter {0}",LevelDataInfo.chapter);
            changeLevelScene();
        }
    }

    public void changeLevelScene(){
        startChange = true;
    }

    void changeNewPlanet(){
        Destroy(myPlanet);

        string planetName = LevelDataInfo.planetList[LevelDataInfo.chapter];
        myPlanet = Instantiate(Resources.Load( "PlanetObjs/"+planetName, typeof( GameObject ) ), 
            new Vector3 (0,0,0), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
        myPlanet.transform.localScale = new Vector3 (0.17f, 0.17f, 0.17f);
    }

    void loadingMask(){
         if(startChange){
            if(maskRadius < 0){
                maskAmplify = -1;

                if(maskWaiting == maskWaitingTime){
                    levelBlockCB();
                    changeNewPlanet();
                }

                maskWaiting -= Time.deltaTime;
                if(maskWaiting <= 0f){
                    maskWaiting = 0f;
                }
            }
            if(maskWaiting <= 0 && maskAmplify == -1 && maskRadius >= Screen.width/2){
                maskWaiting = maskWaitingTime;
                maskAmplify = 1;
                startChange = false;
                maskRadius = Screen.width/2;
            }

            if(maskWaiting == maskWaitingTime || maskWaiting == 0f){
                maskRadius -= (Screen.width/40) * maskAmplify;
            }
        } 
    }

    void LateUpdate() {
        loadingMask();
        // return;
        if(targetTsf != null && fracJourney < 1.0f){

            float distCovered = (Time.time - startTime) * MoveSpeed;
            fracJourney = distCovered / journeyLength;
            // fracJourney = 1.0f;

           

            this.transform.parent.transform.position = Vector3.Lerp(startPos, targetTsf.position,fracJourney);
            Vector3 currentAngle = new Vector3(
                Mathf.LerpAngle(startEular.x, targetTsf.rotation.eulerAngles.x, fracJourney),
                Mathf.LerpAngle(startEular.y, targetTsf.rotation.eulerAngles.y, fracJourney),
                Mathf.LerpAngle(startEular.z, targetTsf.rotation.eulerAngles.z, fracJourney));

            this.transform.parent.transform.eulerAngles = currentAngle;

            if(fracJourney >= 1.0f){
                PopupReadyPanel(currengLevelIndex+1);
            }

        }
           
        if (Input.GetKeyDown(KeyCode.LeftShift))
            CameraDisabled = !CameraDisabled;

        if (!CameraDisabled)
        {

            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

                ScrollAmount *= (this._CameraDistance * 0.3f);

                this._CameraDistance += ScrollAmount * -1f;

                this._CameraDistance = Mathf.Clamp(this._CameraDistance, 1.5f, 5f);
            }

            if ( this._XForm_Camera.localPosition.z != this._CameraDistance * -1f )
            {
                this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
            }

        } 

        //Actual Camera Rig Transformations
        // Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        // this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

        
    }
}