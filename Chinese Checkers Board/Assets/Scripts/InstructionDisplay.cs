using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionDisplay : MonoBehaviour
{
	GameObject canvas;
	GameObject helpTitle;
	GameObject helpText;
	public bool help = false;

	public void Start(){
		canvas = GameObject.Find ("Canvas");
		helpTitle = canvas.transform.Find ("HelpTitle").gameObject;
		helpText = canvas.transform.Find ("HelpText").gameObject;
	}
	public void display(){
		if (!help) {
			help = true;
			helpText.SetActive (true);
			helpTitle.SetActive (true);
			//StartCoroutine (waitToHide ());
		} else {
			help = false;
			helpText.SetActive (false);
			helpTitle.SetActive (false);
		}
	}

	IEnumerator waitToHide(){
		yield return new WaitForSeconds (10.0f);
		helpText.SetActive (false);
		helpTitle.SetActive (false);
	}
}
