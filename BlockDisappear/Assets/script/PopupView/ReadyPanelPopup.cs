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


		Debug.LogFormat("setLevel {0},{1}",levelIndex,LevelDataInfo.chapter);
		TextMeshProUGUI textmeshPro = gameObjText.GetComponent<TextMeshProUGUI>();
		textmeshPro.text = "Level "+ selectLevel;

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
