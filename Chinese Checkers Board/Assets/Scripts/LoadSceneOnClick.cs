using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {
	public int sceneIndex;

	public void LoadLevel()
	{
		Debug.Log ("Loading game (Scene Index 1");
		SceneManager.LoadScene(sceneIndex);
	}
}
