using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

	public List<Level> levels;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		if(FindObjectsOfType<LevelController>().Length > 1){
			Destroy(gameObject);
		}
		// levels = new List<Level>{
		// 	new Level(0,"Level-1",true,2,false),
		// 	new Level(1,"Level-2",false,0,false),
		// 	new Level(2,"Level-3",false,0,true),
		// 	new Level(3,"Level-4",false,0,true),
		// 	new Level(4,"Level-5",false,0,true),
		// 	new Level(5,"Level-6",false,0,true),
		// 	new Level(6,"Level-7",false,0,true),
		// 	new Level(7,"Level-8",false,0,true),
		// 	new Level(8,"Level-9",false,0,true),
		// 	new Level(9,"Level-10",false,0,true),
		// 	new Level(10,"Level-9",false,0,true),
		// 	new Level(11,"Level-10",false,0,true),
		// 	new Level(12,"Level-9",false,0,true),
		// 	new Level(13,"Level-10",false,0,true)
		// };
		// Debug.Log(levels[0].name);
		GetAllLevel();
	}

	void GetAllLevel(){
		levels = new List<Level>{};
		Object[] levelFiles = Resources.LoadAll("Levels", typeof(TextAsset));

		for(int i = 0 ; i < levelFiles.Length ; i++ ){
			// Level l = new Level(i+1 ,levelFiles[i].name , false,0,false);
			levels.Add(new Level(i+1 ,levelFiles[i].name , false,0,false));
			// Debug.Log(levels[i].name);
		}

	}

	public void LevelStart(string levelName){
		LevelDataInfo.selectLevelName = levelName;
		SceneManager.LoadScene("MainScene");

	}

	public void LevelComplete(string levelName){
	

		// Level level = levels.Find(i => i.LevelName == levelName);
		// level.LevelComplete();
		// Debug.Log(levels[0].Stars);
		// levels[0].LevelComplete();
	}

	public void LevelComplete(string levelName,int stars){
		levels.Find(i => i.LevelName == levelName).LevelComplete(stars);
	}
}
