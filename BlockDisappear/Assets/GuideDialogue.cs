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
     

    void initUFO(string UFOPATH){
        string u_path = "UFO_level_tutor";
        if(UFOPATH != null){
            u_path = UFOPATH;
        }else{
            
        }
        Debug.LogFormat("u_path {0}",u_path);
        tutorUFO = Instantiate(Resources.Load(u_path, typeof( GameObject ) ), 
            new Vector3 (0,0,0), Quaternion.Euler (292f, -352f, 0f)) as GameObject;
		UFOTransition = tutorUFO.GetComponent<Animator>();
        dialogueUIView.SetActive(false);
        dialogueTransition = dialogueUIView.GetComponent<Animator>();
        tutorUFO.SetActive(false);
    }

    public void setGameUFO(GameObject _tutorUFO,GameObject UFOContainer=null,string UFOPATH=null){
        Debug.LogFormat("UFOPATH1 {0}",UFOPATH);
        initUFO(UFOPATH);
        if(UFOContainer != null){
            tutorUFO.transform.SetParent(UFOContainer.transform);
            tutorUFO.transform.localPosition = Vector3.zero;

        }
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
