using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideDialogue : MonoBehaviour
{

    public Animator dialogueTransition;

    public Animator UFOTransition;

    private GameObject tutorUFO;

    private GameObject gameUFO;

    private bool isOn = true;

    private int diaIndex = 0;

    private AnimatorStateInfo UFOAnimatorInfo;
    // Start is called before the first frame update

    // Update is called once per frame

    void Start() {
        tutorUFO = Instantiate(Resources.Load( "UFO_level", typeof( GameObject ) ), 
        new Vector3 (1,1,1), Quaternion.Euler (250f, 175f, 0f)) as GameObject;
		
        // tutorUFO.SetActive(false);
    }

    public void setGameUFO(GameObject _tutorUFO){
        tutorUFO.SetActive(true);
        gameUFO = _tutorUFO;
        gameUFO.SetActive(false);
    }
    
    void Update()
    {
        UFOAnimatorInfo = UFOTransition.GetCurrentAnimatorStateInfo(0);   
        if(UFOAnimatorInfo.normalizedTime >= 1.0f){
            Debug.LogFormat("UFO_complete");
            tutorUFO.SetActive(false);
            // Destroy(tutorUFO);
            gameUFO.SetActive(true);
            this.GetComponent<GuideDialogue>().enabled = false;
            LevelDataInfo.tutorFinished = true;
            return;
        }
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
