using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public bool mouseDown = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPointerDown(PointerEventData eventData){
	//	Debug.Log ("OnPointerDown");
		mouseDown = true;
	}

	public void OnPointerUp(PointerEventData eventData){
	//	Debug.Log ("OnPointerUp");
		mouseDown = false;
	}

}
