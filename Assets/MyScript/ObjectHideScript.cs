using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ObjectHideScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Manager.instance.anim.SetBool ("Fade",true);
		Invoke ("Reset",0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void HideObject(){
		GameObject g = gameObject;
		gameObject.SetActive (false);
		Manager.instance.list.Add (g);
		Manager.instance.remainingCube--;
		Manager.instance.anim.SetBool ("Fade",true);
		Invoke ("Reset",0f);
		if(Manager.instance.remainingCube<2){
			//GenerateCube ();
			Invoke ("GenerateCube",1f);
		}
	}

	void GenerateCube(){
		Manager.instance.list[0].SetActive(true);
		Manager.instance.list.RemoveAt(0);
		print (Manager.instance.list [0].name);

		Manager.instance.remainingCube++;

	}

	void Reset(){
		print ("FadeOutisCalled");
		Manager.instance.anim.SetBool ("Fade",false);
	}
}
