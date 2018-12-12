using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOnClick : MonoBehaviour {

	public void Quit()
	{
#if UNITY_EDITOR
	Debug.Log("User quitting game");
	UnityEditor.EditorApplication.isPlaying = false;
#else
	Application.Quit();
#endif
	}
		
}
