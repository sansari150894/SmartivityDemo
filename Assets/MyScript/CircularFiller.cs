using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularFiller : MonoBehaviour {

	public float fill = 0f;
	public static CircularFiller instance;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Image> ().fillAmount = fill;
	}
}
