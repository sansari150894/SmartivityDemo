using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoad : MonoBehaviour {
	public MeshRenderer mr;
	void Start () {
		mr.materials [0].DOColor (new Color32 (0, 0, 0, 0), 1f);
	}
	
	// Update is called once per frame

	public void LoadGamePlay(){
		Debug.Log ("SceneLoaded");
		StartCoroutine(LoadGamePlayWithDelay());
	}
	IEnumerator LoadGamePlayWithDelay(){
		mr.materials [0].DOColor (new Color32 (0, 0, 0, 255), 1f);
		yield return new WaitForSeconds (1f);
		SceneManager.LoadScene ("GamePlay");



	}
}
