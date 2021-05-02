using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var dialogueUI = Instantiate(Resources.Load("UI/DialogueUIView")) as GameObject;

        GameObject canvas = GameObject.Find("MainSceneCanvans") as GameObject;
		dialogueUI.transform.SetParent(canvas.transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
