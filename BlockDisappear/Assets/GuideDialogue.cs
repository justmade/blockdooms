using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideDialogue : MonoBehaviour
{

    public Animator dialogueTransition;

    public Animator UFOTransition;

    private bool isOn = true;

    private int diaIndex = 0;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            nextDialogue();
        }
    }

    void nextDialogue(){
        Debug.LogFormat("nextDialogue!!!!");
        if(isOn){
            dialogueTransition.SetTrigger("End");
        }else{
            dialogueTransition.SetTrigger("Start");
        }
        isOn = !isOn;

        diaIndex ++;

        if(diaIndex == 5){
            UFOTransition.SetTrigger("UFO_EXIT");
        }
    }
}
