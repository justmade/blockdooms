using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

	// Use this for initialization
	public int ID{get;set;}

	public string LevelName {get;set;}

	public bool Completed {get;set;}

	public int Stars{get;set;}

	public bool Locked{get;set;}


	public Level(int id , string levelName , bool completed , int stars , bool locked){
		this.ID = id;
		this.LevelName = levelName;
		this.Completed = completed;
		this.Stars = stars;
		this.Locked = locked;
	}

	public void LevelComplete(){
		this.Completed = true;
	}

	public void LevelComplete(int stars){
		this.Completed = true;
		this.Stars = stars;
	}

	public void Lock(){
		this.Locked = true;
	}

	public void UnLock(){
		this.Locked = false;
	}

}
