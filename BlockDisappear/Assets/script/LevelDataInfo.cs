using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelDataInfo  {
	public static string selectLevelName;

    private static int _selectLevelIndex = 0;

	public static int chapter = 0;

    public static int lastPage = 0;

	public static List<Level> levels;

	public static bool isTest = false;

	public static string UP = "u";

	public static string DOWN = "d";

	public static string LEFT = "l";

	public static string RIGHT = "r";

	public static string STOP = "s";

    public static int SelectLevelIndex { 

		get => _selectLevelIndex; 

		set => _selectLevelIndex = value; 

		}

    private static int _currentLevel = 0;

    public static int CurrentLevel { 
		get => _selectLevelIndex + chapter * 20  ;
		set => _currentLevel = value; }


	public static string reversDirect(string _d ){
		if(_d == LevelDataInfo.UP){
			return LevelDataInfo.DOWN;
		}else if(_d == LevelDataInfo.DOWN){
			return LevelDataInfo.UP;
		}else if(_d == LevelDataInfo.LEFT){
			return LevelDataInfo.RIGHT;
		}else if(_d == LevelDataInfo.RIGHT){
			return LevelDataInfo.LEFT;
		}  
		return null;
	}
}
