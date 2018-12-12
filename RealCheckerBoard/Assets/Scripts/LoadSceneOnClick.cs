using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadRandomLevel()
	{
		int sceneIndex = Random.Range (1,3);
		SceneManager.LoadScene(sceneIndex);
	}
}
