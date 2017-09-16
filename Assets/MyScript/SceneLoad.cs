using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour {

	// Use this for initialization
	public Animator anim;
	public Image black;
	void Start () {
		
	}
	
	// Update is called once per frame

	public void LoadGamePlay(){
		Debug.Log ("SceneLoaded");
		StartCoroutine(LoadGamePlayWithDelay());
	}
	IEnumerator LoadGamePlayWithDelay(){
		anim.SetBool ("FadeMain",true);
		yield return new WaitForSeconds (1f);
		SceneManager.LoadScene ("GamePlay");
		//Manager.instance.anim.SetBool ("Fade",false);


	}
}
