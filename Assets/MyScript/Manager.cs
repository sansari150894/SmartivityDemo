using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

	// Use this for initialization
	public static Manager instance;
	public Animator anim;
	public Image black;
	public int remainingCube = 3;
	public  List <GameObject> list=new List<GameObject>();

	public MeshRenderer mr;


	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
