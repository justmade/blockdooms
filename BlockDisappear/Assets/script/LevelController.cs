using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;
using System.IO;
using LFormat;

public class LevelController : MonoBehaviour {

	public List<Level> levels = new List<Level>{};

	public bool isTest = true;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		if(FindObjectsOfType<LevelController>().Length > 1){
			Destroy(gameObject);
		}
		LevelDataInfo.isTest = isTest;
		if(isTest){
			GetAllLevel();
		}else{
			GetLevelConfig();
		}	
		
	}
	
	//读取配置好的 levelconfig
	void GetLevelConfig(){
		if(LevelDataInfo.levels == null){
			var config = Resources.Load<TextAsset>("levelconfig");
			string _levelData = config.text;
			JsonData levelObj = JsonMapper.ToObject(_levelData);
			
			for(int i = 1; i <= levelObj.Count ; i ++){
				var levelData = levelObj[i.ToString()];
				levels.Add(new Level(i ,levelData["Name"].ToString(), false,0,false));
				LevelDataInfo.levels = levels;
			}
		} else{
			levels = LevelDataInfo.levels;
		}
		
	}

	void GetAllLevel(){
		if(LevelDataInfo.levels == null){
			Object[] levelFiles = Resources.LoadAll("Levels", typeof(TextAsset));
			for(int i = 0 ; i < levelFiles.Length ; i++ ){
				// Debug.LogFormat("Level Name{0}",levelFiles[i].name);
				levels.Add(new Level(i+1 ,levelFiles[i].name , false,0,false));
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
		LevelDataInfo.SelectLevelIndex ++ ;

		LevelDataInfo.topLevel = getLevelIndexByLevelName(levelName)+2 > LevelDataInfo.topLevel 
			? getLevelIndexByLevelName(levelName)+2 : LevelDataInfo.topLevel;
	}

	public void LevelComplete(string levelName,int stars){
		levels.Find(i => i.LevelName == levelName).LevelComplete(stars);
		LevelDataInfo.SelectLevelIndex ++ ;
		LevelDataInfo.topLevel = getLevelIndexByLevelName(levelName)+2 > LevelDataInfo.topLevel 
			? getLevelIndexByLevelName(levelName)+2 : LevelDataInfo.topLevel;

		Debug.LogFormat("LevelComplete{0},{1}",stars,LevelDataInfo.topLevel);
	}

	private int getLevelIndexByLevelName(string _name){
		for (int i = 0; i < levels.Count; i++)
		{
			if(levels[i].LevelName == _name){
				return i;
			}
		}
		return -1;
	}
}
