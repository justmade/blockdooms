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
        addPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
    }

    void addPoint(){
        onState = Sprite.Create(state_on,new Rect(0.0f, 0.0f,iconSize.x, iconSize.y),new Vector2(0.5f,0.5f));//注意居中显示采用0.5f值  //创建一个精灵(图片，纹理，二维浮点型坐标)
        offState = Sprite.Create(state_off,new Rect(0.0f, 0.0f,iconSize.x, iconSize.y),new Vector2(0.5f,0.5f));
        points = new GameObject[LevelDataInfo.planetList.Length];
        for (int i = 0; i < LevelDataInfo.planetList.Length; i++)
        {
            GameObject pointObj = new GameObject();
            Image spr = pointObj.AddComponent<Image>(); 
            
            
            spr.sprite = offState; 
            pointObj.transform.parent = this.gameObject.transform;
            pointObj.transform.localPosition = new Vector3(i *iconSize.x,0,0);
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
    }
}
