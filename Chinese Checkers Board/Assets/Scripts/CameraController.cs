using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public BoardManager bm;

	public Transform from;
	public Transform to;

	private float timeCount = 0.0f;
	public bool rotate=false;

	public bool waitBefore;
	public bool waitAfter;
    // Update is called once per frame
    void Update()
    {
		/*
		GetCenter(Vector3.up);
		float fracComplete = (Time.time - startTime) / duration * speed;
		transform.position = Vector3.Slerp (startRelCenter, endRelCenter, fracComplete*speed);
		transform.position += centerPoint;
		*/
		if (rotate) {
			//Rotate ();
			transform.rotation = Quaternion.Slerp (from.rotation, to.rotation, timeCount);
			transform.position = Vector3.Slerp (from.position, to.position, timeCount);
			timeCount += Time.deltaTime;
			//check if finished rotating
			if (Quaternion.Angle(transform.rotation,to.rotation)<Mathf.Epsilon) {
				rotate = false;
				timeCount = 0.0f;
				Debug.Log ("done rotating");
				if (!bm.gameEnded&&!bm.curPlayer.isManual)
					StartCoroutine (WaitAI());
			}
		}
	}

	IEnumerator WaitAI(){
		yield return new WaitForSeconds (0.6f);
		bm.curPlayer.DoMove();
	}

}
