using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform targetTransform;

	private Vector3 _cameraOffset;

	public float rotateSpeed = 5.0f;

	private float mouseDuration = 0f;

	public bool startMove = false;

	private bool mouseIsDown = false;

	// Use this for initialization
	void Start () {
		_cameraOffset = transform.position - targetTransform.position;
		_cameraOffset = _cameraOffset/5;
	}

	
	public void setCameraTarget(Transform _targetTransform){
		targetTransform = _targetTransform;
		Vector3 dir = Vector3.Normalize(targetTransform.position);
		// startMove = true;
			Quaternion camTurnAngleY = Quaternion.AngleAxis (Input.GetAxis ("Mouse Y") * rotateSpeed , Vector3.left);
			Quaternion camTurnAngleX = Quaternion.AngleAxis (Input.GetAxis ("Mouse X") * rotateSpeed , Vector3.forward);

			Quaternion camTurnAngle = new Quaternion ();
			camTurnAngle.x = camTurnAngleY.x;
			camTurnAngle.z = camTurnAngleX.z;
			camTurnAngle.w = 1f;

			// _cameraOffset = camTurnAngle * _cameraOffset;
			// float R = 20f;
			// float angle1 = (180) * Mathf.PI / 180;
			// float angle2 = (0) * Mathf.PI / 180;
			// float px = R * Mathf.Sin(angle1) * Mathf.Cos(angle2);
			// float py = R * Mathf.Sin(angle1) * Mathf.Sin(angle2);
			// float pz = R * Mathf.Cos(angle1);

			// Vector3 dirR = new Vector3(px,py,pz);

			// Quaternion.Euler(0, 0, 角度)
		
			// Vector3 rotatedAngle = dir * 10;
			Vector3 rotatedAngle2 = Quaternion.Euler(-50, 0,0) * dir * 6;		

				
			Vector3 newPos = targetTransform.position + rotatedAngle2;
			transform.position = newPos;

			// Quaternion turnRotation=  Quaternion.LookRotation(new Vector3(px,py,pz),Vector3.up);

			transform.rotation = Quaternion.LookRotation(rotatedAngle2 *-1);
			// transform.LookAt (rotatedAngle2,Vector3.up);
			// transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x ,transform.rotation.eulerAngles.y,0);
			for(int i=0;i<36;i ++){

				float fx = Mathf.Sin(Mathf.PI * i/36)*20;
				float angle1 = (i*10) * Mathf.PI / 180; 
				Debug.LogFormat ("fx{0}" ,transform.localEulerAngles.y);
				float angle2 = (transform.localEulerAngles.y) * Mathf.PI / 180;
				
				float px = 7 * Mathf.Sin(angle1) * Mathf.Cos(angle2);
				float pz = 7 * Mathf.Sin(angle1) * Mathf.Sin(angle2);
				float py = 7 * Mathf.Cos(angle1);


				Vector3 rotatedAngle = Quaternion.Euler( -10 * i,
				0,
				 0 * i) * dir * 7;
				GameObject g = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				g.transform.position =  targetTransform.position + new Vector3(px,py,pz);;

				// g.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x -10 * i,
				// transform.rotation.eulerAngles.y,
				// transform.rotation.eulerAngles.z);
				// targetTransform.position + rotatedAngle - 
				g.transform.rotation =  Quaternion.LookRotation(rotatedAngle.normalized);
				Quaternion q = Quaternion.LookRotation(rotatedAngle.normalized);
				g.transform.rotation =  Quaternion.Euler(q.eulerAngles.x,180,q.eulerAngles.z);
			}
			
	}

	
	// Update is called once per frame
	void LateUpdate () {

		// if (Input.GetMouseButtonDown (0)) {
		// 	mouseIsDown = true;
		// }
		// if (mouseIsDown) {
		// 	mouseDuration += Time.deltaTime;
		// 	if (mouseDuration >= 15 * 0.016f ) {
		// 		startMove = true;
		// 	}

		// } 

		// if (Input.GetMouseButtonUp (0)) {
		// 	mouseDuration = 0f;
		// 	startMove = false;
		// 	mouseIsDown = false;
		// }


		if (startMove) {
			Quaternion camTurnAngleY = Quaternion.AngleAxis (Input.GetAxis ("Mouse Y") * rotateSpeed , Vector3.left);
			Quaternion camTurnAngleX = Quaternion.AngleAxis (Input.GetAxis ("Mouse X") * rotateSpeed , Vector3.forward);

			Quaternion camTurnAngle = new Quaternion ();
			camTurnAngle.x = camTurnAngleY.x;
			camTurnAngle.z = camTurnAngleX.z;
			camTurnAngle.w = 1f;

			_cameraOffset = camTurnAngle * _cameraOffset;

			Vector3 newPos = targetTransform.position + _cameraOffset;
			transform.position = Vector3.Slerp (transform.position, newPos, 0.1f);
			transform.LookAt (targetTransform,Vector3.forward);

		}
	}
}
