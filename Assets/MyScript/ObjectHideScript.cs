using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ObjectHideScript : MonoBehaviour {
	
	void Start () {
		Invoke ("Reset",0f);
	}
	void Update () {
	}

	public void HideObject(){	
		
		Manager.instance.mr.materials [0].DOColor (new Color32(0,0,0,255),1f);
		GameObject g = gameObject;
		gameObject.SetActive (false);
		Manager.instance.list.Add (g);
		Manager.instance.remainingCube--;
		Invoke ("Reset",1f);
		if(Manager.instance.remainingCube<2){
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
		Manager.instance.mr.materials [0].DOColor (new Color32(0,0,0,0),1f);
	}


}
