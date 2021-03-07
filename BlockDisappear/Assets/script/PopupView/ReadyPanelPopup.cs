using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyPanelPopup : MonoBehaviour {
	[SerializeField] private LevelController levelController;
	
	public Text tipText;
	public GameObject gameObjText;


	private int currentLevel = 0;

    void Start () {
		

	}

	public void setLevel(int levelIndex){

		currentLevel = levelIndex;
		// tipText.text = "Level "+ levelIndex;
		int selectLevel = currentLevel + LevelDataInfo.chapter * 12;


		
		TextMeshProUGUI textmeshPro = gameObjText.GetComponent<TextMeshProUGUI>();
		textmeshPro.text = "Level "+ selectLevel;

		GameObject LockPanel = GameObject.Find("LockPanel");
		LockPanel.SetActive(true);
		GameObject ExplainText = LockPanel.transform.Find("ExplainText").gameObject;
		GameObject LockImage = LockPanel.transform.Find("LockImage").gameObject;

		Debug.LogFormat("setLevel levelIndex{0},{1},top{2}",levelIndex,selectLevel,LevelDataInfo.topLevel);

		TextMeshProUGUI lockPro =  ExplainText.GetComponent<TextMeshProUGUI>();
		lockPro.text = "Pass level "+(selectLevel-1)+" to unlock.";

		if(LevelDataInfo.topLevel < selectLevel){
			ExplainText.SetActive(true);
			LockImage.SetActive(true);
		}else{
			ExplainText.SetActive(false);
			LockImage.SetActive(false);
		}	

	}

	public void startLevel(){
		if(currentLevel != 0){
			onSelectLevel(currentLevel);
		}
	}

	private void onSelectLevel(int levelIndex){
		int selectLevel = levelIndex + LevelDataInfo.chapter * 12;
		Debug.LogFormat("onSelectLevel {0}",selectLevel);
		Level level = levelController.levels[selectLevel-1];
		LevelDataInfo.SelectLevelIndex = levelIndex-1;
		levelController.LevelStart(level.LevelName);
	}







}
