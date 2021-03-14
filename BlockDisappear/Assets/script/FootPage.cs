using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FootPage : MonoBehaviour
{
    public Texture2D state_off;
    public Texture2D state_on;
    public Vector2 iconSize;
    // Start is called before the first frame update
    Sprite onState;
    Sprite offState;
    GameObject[] points;

    void Start()
    {
        checkBTNState();
        addPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {

    }


    void checkBTNState(){
        int unlockPage = Mathf.FloorToInt(LevelDataInfo.topLevel /12);
        GameObject btnRight = GameObject.Find("Button - Right");
        GameObject btnLeft = GameObject.Find("Button - Left");

        GameObject newLabel = btnRight.transform.Find("NewLabel").gameObject;
        if(unlockPage <= LevelDataInfo.chapter){
            btnRight.GetComponent<Image>().color =  new Vector4(1.0f, 1.0f, 1.0f,0.1f);
            btnRight.GetComponent<Button>().interactable = false;

            newLabel.SetActive(false);
        }else{
            btnRight.GetComponent<Image>().color =  new Vector4(1.0f, 1.0f, 1.0f,1.0f);
            btnRight.GetComponent<Button>().interactable = true;

            if(LevelDataInfo.topLevel %12 == 1 && LevelDataInfo.topLevel>=12){
                newLabel.SetActive(true);
            }else{
                newLabel.SetActive(false);
            }
        }

        if(LevelDataInfo.chapter == 0){
            btnLeft.GetComponent<Image>().color =  new Vector4(1.0f, 1.0f, 1.0f,0.1f);
            btnLeft.GetComponent<Button>().interactable = false;
        }else{
            btnLeft.GetComponent<Image>().color =  new Vector4(1.0f, 1.0f, 1.0f,1.0f);
            btnLeft.GetComponent<Button>().interactable = true;
        }

        

        
		Debug.LogFormat("checkBTNState{0},{1},{2}",LevelDataInfo.topLevel,unlockPage,LevelDataInfo.chapter);

    }

    void addPoint(){
        float totalW = -LevelDataInfo.planetList.Length * iconSize.x/2;
        onState = Sprite.Create(state_on,new Rect(0.0f, 0.0f,iconSize.x, iconSize.y),new Vector2(0.5f,0.5f));//注意居中显示采用0.5f值  //创建一个精灵(图片，纹理，二维浮点型坐标)
        offState = Sprite.Create(state_off,new Rect(0.0f, 0.0f,iconSize.x, iconSize.y),new Vector2(0.5f,0.5f));
        points = new GameObject[LevelDataInfo.planetList.Length];

        for (int i = 0; i < LevelDataInfo.planetList.Length; i++)
        {
            GameObject pointObj = new GameObject();
            Image spr = pointObj.AddComponent<Image>(); 
            
            
            spr.sprite = offState; 
            pointObj.transform.parent = this.gameObject.transform;
            pointObj.transform.localPosition = new Vector3(totalW + i *iconSize.x,0,0);
            RectTransform rectTrans = pointObj.GetComponent<RectTransform>();
            rectTrans.sizeDelta = iconSize;
            pointObj.SetActive(true); //Activate the GameObject

            points[i] = pointObj;

        }
        updatePointState();
        
    }

    public void updatePointState(){
        int index = LevelDataInfo.chapter;
        for (int i = 0; i < points.Length; i++)
        {
            points[i].GetComponent<Image>().sprite = offState; 
        }
        points[index].GetComponent<Image>().sprite = onState; 
        checkBTNState();
    }
}
