using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour {
	public Text tipText;

	public void setText(string txt){
		tipText.text = txt;
	}

}
