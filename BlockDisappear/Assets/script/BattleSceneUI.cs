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

        GameObject UFOContainer = new GameObject();

        GameObject UFO = Instantiate(Resources.Load( "UFO_level", typeof( GameObject ) ), 
		new Vector3 (-4.3f,11.6f,17.32f), Quaternion.Euler (326.2f, 0f, 5.34f)) as GameObject;

        canvas.GetComponent<GuideDialogue>().dialogueUIView = dialogueUI;
        canvas.GetComponent<GuideDialogue>().setGameUFO(UFO,UFOContainer);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
