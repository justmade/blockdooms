using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideDialogue : MonoBehaviour
{

    private Animator dialogueTransition;

    public GameObject dialogueUIView;

    private Animator UFOTransition;

    private GameObject tutorUFO;

    private GameObject gameUFO;

    private bool isOn = true;

    private int diaIndex = 0;

    private AnimatorStateInfo UFOAnimatorInfo;

    private int dialogueLength = 5;

    // Start is called before the first frame update

    // Update is called once per frame
    

    void Start() {
        
    }

    public void enableDialogue(){
        dialogueUIView.SetActive(false);
    }
     

    void initUFO(){
        tutorUFO = Instantiate(Resources.Load("UFO_level_tutor", typeof( GameObject ) ), 
        new Vector3 (3.5f,6.5f,23.9f), Quaternion.Euler (292f, -352f, 0f)) as GameObject;
		UFOTransition = tutorUFO.GetComponent<Animator>();
        dialogueUIView.SetActive(false);
        dialogueTransition = dialogueUIView.GetComponent<Animator>();
        tutorUFO.SetActive(false);
    }

    public void setGameUFO(GameObject _tutorUFO){
        initUFO();
        tutorUFO.SetActive(true);
        UFOTransition.SetTrigger("UFO_TUTOR");
        gameUFO = _tutorUFO;
        gameUFO.SetActive(false);
        dialogueUIView.SetActive(true);
    }
    
    void Update()
    {
        UFOAnimatorInfo = UFOTransition.GetCurrentAnimatorStateInfo(0);   
        if(UFOAnimatorInfo.normalizedTime >= 1.0f && diaIndex >= 5){
            Debug.LogFormat("UFO_complete");
            tutorUFO.SetActive(false);
            // Destroy(tutorUFO);
            gameUFO.SetActive(true);
            this.GetComponent<GuideDialogue>().enabled = false;
            LevelDataInfo.tutorFinished = true;
            dialogueUIView.SetActive(false);
            return;
        }
        if(Input.GetMouseButtonDown(0)){
            nextDialogue();
        }
    }

    void nextDialogue(){
        if(diaIndex < dialogueLength){
            Debug.LogFormat("nextDialogue!!!!");
            // if(isOn){
            //     dialogueTransition.SetTrigger("End");
            // }else{
            //     dialogueTransition.SetTrigger("Start");
            // }
            // dialogueTransition.SetTrigger("Start");
            isOn = !isOn;
            diaIndex ++;

            if(diaIndex == 5){
                dialogueTransition.SetTrigger("End");
                UFOTransition.SetTrigger("UFO_EXIT");

            }else{
                dialogueTransition.SetTrigger("Start");
            }
        }
       
    }
}
