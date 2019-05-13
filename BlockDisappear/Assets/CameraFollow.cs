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

			_cameraOffset = camTurnAngle * _cameraOffset;

			Vector3 newPos = targetTransform.position + _cameraOffset;
			transform.position = newPos;
			transform.LookAt (targetTransform,Vector3.forward);
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
