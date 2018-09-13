using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour {
	public Level level;
	public Text LevelIDText;
	public Text LevelNameText;
	private Transform starParent;
	public GameObject lockImage;
	private Image[] stars;


	void Awake()
	{
		starParent = transform.Find("stars").transform;
		stars  = starParent.GetComponentsInChildren<Image>();
	}

	public void SetStars(int starsNum)
	{
		for(int i = 0 ; i < starsNum ; i++){
			this.stars[i].color = Color.white;
		}
	}
	

}
