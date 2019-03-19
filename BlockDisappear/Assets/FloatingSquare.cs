using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class FloatingSquare : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 startPos = this.transform.localPosition;
		Vector3 toPos = new Vector3(startPos.x, startPos.y+0.2f,startPos.z);

		float dur = 3f + UnityEngine.Random.Range(1f,3f);
		Tween myTween = transform.DOLocalMove(toPos, dur);
		myTween.SetDelay(UnityEngine.Random.Range(0f,2f));
		myTween.SetEase(Ease.OutSine);
		myTween.SetLoops(-1,LoopType.Yoyo);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
