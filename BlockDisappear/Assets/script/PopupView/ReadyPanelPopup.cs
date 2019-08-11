using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPanelPopup : MonoBehaviour {
	[SerializeField] private LevelController levelController;
	
	public Text tipText;

	private int currentLevel = 0;

    void Start () {
		

	}

	public void setLevel(int levelIndex){
		currentLevel = levelIndex;
		tipText.text = "Level "+ levelIndex;

	}

	public void startLevel(){
		if(currentLevel != 0){
			onSelectLevel(currentLevel);
		}
	}

	private void onSelectLevel(int levelIndex){
		Level level = levelController.levels[levelIndex-1];
		levelController.LevelStart(level.LevelName);

	}







}
