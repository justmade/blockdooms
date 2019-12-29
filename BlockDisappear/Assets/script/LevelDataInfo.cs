﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelDataInfo  {
	public static string selectLevelName;

    private static int _selectLevelIndex = 0;

	public static int chapter = 1;

    public static int lastPage = 0;

	public static List<Level> levels;

    public static int SelectLevelIndex { 

		get => _selectLevelIndex; 

		set => _selectLevelIndex = value; 

		}

    private static int _currentLevel = 0;

    public static int CurrentLevel { 
		get => _selectLevelIndex + chapter * 20  ;
		set => _currentLevel = value; }
}
