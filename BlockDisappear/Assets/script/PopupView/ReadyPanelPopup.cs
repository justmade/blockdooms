using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPanelPopup : MonoBehaviour {
	public Text tipText;

    void Start () {
		

	}

	public void setLevel(int levelIndex){
		tipText.text = "Level "+ levelIndex;
	}

}
