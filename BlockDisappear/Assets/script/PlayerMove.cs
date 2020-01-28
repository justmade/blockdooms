using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	private Vector3 targetPosition = new Vector3(0f,1f,0f);

	private float mapWidth = 4.5f;

	private bool isMoving = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void FixedUpdate()
	{
		getPlayerDirection ();
	}

	private void getPlayerDirection()
	{
		if (!isMoving) {
			if (Input.GetKey (KeyCode.W)) {
				targetPosition = new Vector3 (-mapWidth, 1f, transform.position.z);
			} else if (Input.GetKey (KeyCode.S)) {
				targetPosition = new Vector3 (mapWidth, 1f, transform.position.z);
			} 

			if (Input.GetKey (KeyCode.A)) {
				targetPosition = new Vector3 (transform.position.x, 1f, -mapWidth);
			} else if (Input.GetKey (KeyCode.D)) {
				targetPosition = new Vector3 (transform.position.x, 1f, mapWidth);
			}
		}


		if (targetPosition != transform.position && targetPosition != null && isMoving == false) {
			isMoving = true;
		}

		if (targetPosition == transform.position) {
			Debug.Log ("stop");
			isMoving = false;
		}

		if (isMoving) {
			float step = 20.0f * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, targetPosition, step);
		}

	}




}
