using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


	public Transform from;
	public Transform to;

	private float timeCount = 0.0f;
	public bool newPlayer=false;

	public Transform startPos;
	public Transform endPos;
	public float duration = 1.0f;
	public float speed;

	float startTime;
	Vector3 centerPoint;
	Vector3 startRelCenter;
	Vector3 endRelCenter;
    // Update is called once per frame
    void Update()
    {
		/*
		GetCenter(Vector3.up);
		float fracComplete = (Time.time - startTime) / duration * speed;
		transform.position = Vector3.Slerp (startRelCenter, endRelCenter, fracComplete*speed);
		transform.position += centerPoint;
		*/
		if (newPlayer) {
			//Rotate ();
			transform.rotation = Quaternion.Slerp (from.rotation, to.rotation, timeCount);
			transform.position = Vector3.Slerp (from.position, to.position, timeCount);
			timeCount += Time.deltaTime;
			if (transform.rotation == to.rotation) {
				//newPlayer = false;
			}
		}
	}

	//gets center point
	public void GetCenter(Vector3 direction){
		centerPoint = (startPos.position + endPos.position) * .5f;
		centerPoint -= direction;
		startRelCenter = startPos.position - centerPoint;
		endRelCenter = endPos.position - centerPoint;
	}

	public void Rotate(){
		while (transform.rotation != to.rotation) {
			transform.rotation = Quaternion.Slerp (from.rotation, to.rotation, timeCount);
			transform.position = Vector3.Slerp (from.position, to.position, timeCount);
			timeCount += Time.deltaTime;
		}
	}
}
