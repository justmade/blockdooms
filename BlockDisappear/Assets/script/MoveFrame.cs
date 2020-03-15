using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Assertions;
using System;
using System.IO;
using DG.Tweening;

public class MoveFrame : MonoBehaviour {

    private ParticleSystem arrowParticle;
    private GameObject pEffect;
    private float waitTime = 2.0f;
    private float timer = 0.0f;


    void Awake(){
    }

    public void setDir(string state){
        initParticle();
        float angle = 0f;
        if(state == LevelDataInfo.UP){
            angle = -90f;
        }else if(state == LevelDataInfo.DOWN){
            angle = 90f;
        }else if(state == LevelDataInfo.LEFT){
            angle = 180f;
        }else if(state == LevelDataInfo.RIGHT){
            angle = 0f;
        }  
        pEffect.transform.rotation = Quaternion.Euler (0f, angle, 0f);

        int childLens = pEffect.transform.childCount;
        for(int i=0;i<childLens;i++){
            GameObject child= pEffect.transform.GetChild(i).gameObject;
            ParticleSystem p = child.GetComponent<ParticleSystem> ();
            var main = p.main;
            main.startRotation = Mathf.PI * angle/180.0f;
        }
		arrowParticle.Play ();
    }

    void initParticle(){
        pEffect = Instantiate(Resources.Load("particle/Arrow", typeof( GameObject ) ), new Vector3 (1,1,1), Quaternion.Euler (0f, 0f, 0f)) as GameObject;
		pEffect.transform.parent = this.gameObject.transform;
		pEffect.transform.position = this.gameObject.transform.position;
		ParticleSystem p = pEffect.GetComponent<ParticleSystem> ();
        arrowParticle = p;
    }

    public void DestroyParticle(){
        Destroy(pEffect,1.0f);
    }

    void Update () {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            waitTime = waitTime +1f;
            timer = timer - waitTime;
            arrowParticle.Play ();
        }
    }


        
}