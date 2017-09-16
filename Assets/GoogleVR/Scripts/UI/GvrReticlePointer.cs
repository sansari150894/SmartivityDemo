// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
/// Draws a circular reticle in front of any object that the user points at.
/// The circle dilates if the object is clickable.
public class GvrReticlePointer : GvrBasePointer {

	public static GvrReticlePointer instance;

	//
	public GameObject red_gaze,white_gaze;
	Mesh mesh;
	Coroutine AnimatedReticle_CR;
	public static GameObject CurrentGazeObject;
	GameObject LastGazedObject;

	EventTrigger GazeObjectEventTrigger;
	PointerEventData GazeObjectPointerData;

	internal float ReticleLoadingTime=2f;
	float initialValue = 0f;

	public bool objectClick=false;
  // The constants below are expsed for testing.
  // Minimum inner angle of the reticle (in degrees).
  public const float RETICLE_MIN_INNER_ANGLE = 0.0f;
  // Minimum outer angle of the reticle (in degrees).
  public const float RETICLE_MIN_OUTER_ANGLE = 0.5f;
  // Angle at which to expand the reticle when intersecting with an object
  // (in degrees).
  public const float RETICLE_GROWTH_ANGLE = 1.5f;

  // Minimum distance of the reticle (in meters).
  public const float RETICLE_DISTANCE_MIN = 0.45f;
  // Maximum distance of the reticle (in meters).
  public const float RETICLE_DISTANCE_MAX = 10.0f;

  /// Number of segments making the reticle circle.
  public int reticleSegments = 20;

  /// Growth speed multiplier for the reticle/
  public float reticleGrowthSpeed = 8.0f;

  /// Sorting order to use for the reticle's renderer.
  /// Range values come from https://docs.unity3d.com/ScriptReference/Renderer-sortingOrder.html.
  /// Default value 32767 ensures gaze reticle is always rendered on top.
  [Range(-32767, 32767)]
  public int reticleSortingOrder = 32767;

  public Material MaterialComp { private get; set; }

  // Current inner angle of the reticle (in degrees).
  // Exposed for testing.
  public float ReticleInnerAngle { get; private set; }

  // Current outer angle of the reticle (in degrees).
  // Exposed for testing.
  public float ReticleOuterAngle { get; private set; }

  // Current distance of the reticle (in meters).
  // Getter exposed for testing.
  public float ReticleDistanceInMeters { get; private set; }

  // Current inner and outer diameters of the reticle, before distance multiplication.
  // Getters exposed for testing.
  public float ReticleInnerDiameter { get; private set; }

  public float ReticleOuterDiameter { get; private set; }

  public override float MaxPointerDistance { get { return RETICLE_DISTANCE_MAX; } }

  public override void OnPointerEnter(RaycastResult raycastResultResult, bool isInteractive) {
		print ("PointerEnterIsCalled");	
    SetPointerTarget(raycastResultResult.worldPosition, isInteractive);
	CurrentGazeObject = raycastResultResult.gameObject;
	AnimatedReticle_CR = StartCoroutine(AnimatedReticleVertices());
	}

  public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive) {
    SetPointerTarget(raycastResultResult.worldPosition, isInteractive);
  }

  public override void OnPointerExit(GameObject previousObject) {
    ReticleDistanceInMeters = RETICLE_DISTANCE_MAX;
    ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
    ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;
		StopCoroutine(AnimatedReticle_CR);
		CreateReticleVertices();
		red_gaze.GetComponent<Image> ().fillAmount = 0;
		red_gaze.SetActive (false);
		white_gaze.transform.DOScale(new Vector3(0.0025f,0.0025f,0f),0f);
		CancelInvoke ();

		//MaterialComp.color = Color.white;
  }

  public override void OnPointerClickDown() {}

  public override void OnPointerClickUp() {}

  public override void GetPointerRadius(out float enterRadius, out float exitRadius) {
    float min_inner_angle_radians = Mathf.Deg2Rad * RETICLE_MIN_INNER_ANGLE;
    float max_inner_angle_radians = Mathf.Deg2Rad * (RETICLE_MIN_INNER_ANGLE + RETICLE_GROWTH_ANGLE);

    enterRadius = 2.0f * Mathf.Tan(min_inner_angle_radians);
    exitRadius = 2.0f * Mathf.Tan(max_inner_angle_radians);
  }



  void Awake() {
    ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
    ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;
  }

  protected override void Start() {
		instance = this;
    base.Start();
		mesh = new Mesh();
		gameObject.AddComponent<MeshFilter>();
		GetComponent<MeshFilter>().mesh = mesh;

    Renderer rendererComponent = GetComponent<Renderer>();
    rendererComponent.sortingOrder = reticleSortingOrder;
	MaterialComp = rendererComponent.material;
		//materialComp = gameObject.GetComponent<Renderer>().material;
	CreateReticleVertices();
  }

  void Update() {
    //UpdateDiameters();
  }

  private bool SetPointerTarget(Vector3 target, bool interactive) {
    if (base.PointerTransform == null) {
      Debug.LogWarning("Cannot operate on a null pointer transform");
      return false;
    }

    Vector3 targetLocalPosition = base.PointerTransform.InverseTransformPoint(target);

    ReticleDistanceInMeters =
      Mathf.Clamp(targetLocalPosition.z, RETICLE_DISTANCE_MIN, RETICLE_DISTANCE_MAX);
    if (interactive) {
      ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE + RETICLE_GROWTH_ANGLE;
      ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE + RETICLE_GROWTH_ANGLE;
    } else {
      ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
      ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;
    }
    return true;
  }

  private void CreateReticleVertices() {
		red_gaze.SetActive (false);
		white_gaze.transform.DOScale(new Vector3(0.0025f,0.0025f,0f),0f);
		CancelInvoke ();
		CircularFiller.instance.fill = 0f;
}

	IEnumerator AnimatedReticleVertices()
	{
		Debug.Log ("AnimatedCalled");
		white_gaze.transform.DOScale(new Vector3(0.005f,0.005f,0f),0f);
		red_gaze.SetActive (true);
		InvokeRepeating ("Loader",0,0.2f);
		yield return new WaitForSeconds(ReticleLoadingTime);

		if ((GazeObjectEventTrigger = CurrentGazeObject.GetComponent<EventTrigger>()) != null){
			GazeObjectEventTrigger.OnPointerDown(GazeObjectPointerData);
		}

		print (CurrentGazeObject.name);

		if (CurrentGazeObject.GetComponent<Button> () != null && CurrentGazeObject.GetComponent<Button> ().enabled) {
			Debug.Log ("ThisIsCalled");
			CurrentGazeObject.GetComponent<Button>().onClick.Invoke();
		}
	}

	void Loader(){
		CircularFiller.instance.fill = CircularFiller.instance.fill + .1f;
		if(red_gaze.GetComponent<Image> ().fillAmount>=1){
			CancelInvoke ();
			white_gaze.transform.DOScale(new Vector3(0.0025f,0.0025f,0f),0f);
			CircularFiller.instance.fill = 0f;
		}
	}



}
