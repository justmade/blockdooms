using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

	public List<Level> levels = new List<Level>{};

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		if(FindObjectsOfType<LevelController>().Length > 1){
			Destroy(gameObject);
		}
		GetAllLevel();
	}

	void GetAllLevel(){
		if(LevelDataInfo.levels == null){
			Object[] levelFiles = Resources.LoadAll("Levels", typeof(TextAsset));
			for(int i = 0 ; i < levelFiles.Length ; i++ ){
				// Level l = new Level(i+1 ,levelFiles[i].name , false,0,false);
				levels.Add(new Level(i+1 ,levelFiles[i].name , false,0,false));
				// Debug.Log(levels[i].name);
				LevelDataInfo.levels = levels;
			}
		} else{
			levels = LevelDataInfo.levels;
		}
	}

	public void LevelStart(string levelName){
		LevelDataInfo.selectLevelName = levelName;
		SceneManager.LoadScene("MainScene");

	}

	public void LevelComplete(string levelName){
		Level level = levels.Find(i => i.LevelName == levelName);
		level.LevelComplete();
	}

	public void LevelComplete(string levelName,int stars){
		Debug.LogFormat("LevelComplete{0}",stars);
		levels.Find(i => i.LevelName == levelName).LevelComplete(stars);
	}
}
