using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheNextFlow.UnityPlugins;
using System;

public class WandScript : MonoBehaviour {

	public Material meshTexture;
	public List<GameObject> selected = new List<GameObject> ();
	public string selectionMode = "CREATE"; //CREATE, FACES, GRAB, SCALE, DELETE
	public bool triggerIsDown = false;
	public bool triggerIsVisible = false;
	public bool wholeObject = false;
	// Use this for initialization
	void Start () {
		
	}

	public void Nothing() {

	}


	public void CreateFace() {
		Action action = new Action (Nothing);

		if (selected.Count < 3) {
			Debug.Log ("-----ERROR MUST BELONG SAME OBJECT-----");
			MobileNativePopups.OpenAlertDialog ("Error", "Please select at least 3 vertices", "Cancel", action);
			return;
		}

		CADObject cadObject = selected [0].transform.parent.GetComponent<CADObject> ();

		string objName = selected [0].transform.parent.name;
		List<GameObject> pool = new List<GameObject> ();
		foreach (GameObject g in selected) {
			pool.Add (g);
			if (!g.transform.parent.name.Equals (objName)) {
				Debug.Log ("-----ERROR MUST BELONG SAME OBJECT-----");
				MobileNativePopups.OpenAlertDialog ("Error", "Please select vertices that belong to the same object", "Cancel", action);
				return;
			}
		}

		GameObject startingPoint = pool [0];
		GameObject recent = startingPoint;
		pool.Remove (startingPoint);

		List<GameObject> orderedList = new List<GameObject> ();
		List<Vector3> orderListPositions = new List<Vector3> ();

		orderedList.Add (recent);
		orderListPositions.Add (recent.transform.position);

		//action += Nothing;
		int toFind = pool.Count;

		while (true) {
			if (pool.Count == 0)
				break;

			GameObject found = null;
			foreach (GameObject checkNeighbor in pool) {
				HashSet<GameObject> key = new HashSet<GameObject> ();
				key.Add (checkNeighbor);
				key.Add (recent);
				if (cadObject.edges.ContainsKey (key)) {
					found = checkNeighbor;
					toFind--;
					break;
				}
			}

			if (found == null) {
				Debug.Log ("-error form loop");
				MobileNativePopups.OpenAlertDialog ("Error", "Please select vertices that form a single, complete loop", "Cancel", action);
				return;
			}
			else {
				recent = found;
				pool.Remove (found);
				orderedList.Add(found);
				orderListPositions.Add (found.transform.position);
			}
		}

		HashSet<GameObject> keyEnd = new HashSet<GameObject> ();
		keyEnd.Add (orderedList[0]);
		keyEnd.Add (orderedList[orderedList.Count -1]);
		if (!cadObject.edges.ContainsKey (keyEnd)) {
			Debug.Log ("-error form loop");
			MobileNativePopups.OpenAlertDialog ("Error", "Please select vertices that form a single, complete loop", "Cancel", action);
			return;
		}




		if (orderedList.Count != selected.Count) {
			Debug.Log ("-error form loop");
			MobileNativePopups.OpenAlertDialog ("Error", "You need to select vertices that form a single loop", "Cancel", action);
			return;
		}



		Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
		poly.outside = orderListPositions;
		GameObject a = Poly2Mesh.CreateGameObject(poly);
		a.transform.parent = cadObject.gameObject.transform;
		a.GetComponent<Renderer> ().material = meshTexture;
		orderListPositions.Reverse ();
		Poly2Mesh.Polygon poly2 = new Poly2Mesh.Polygon();
		poly2.outside = orderListPositions;
		GameObject a2 = Poly2Mesh.CreateGameObject(poly2);
		a2.transform.parent = cadObject.gameObject.transform;
		a2.GetComponent<Renderer> ().material = meshTexture;


		Debug.Log ("CREATE FACE NOW");
		Debug.Log (orderedList.Count);
	}
	
	// Update is called once per frame
	void Update () {
		if (selectionMode == "FACES" && (!triggerIsDown || !triggerIsVisible ) && selected.Count > 0) {

			if (triggerIsVisible) {
				CreateFace ();
			}

			foreach (GameObject g in selected) {
				Transform[] children = g.GetComponentsInChildren<Transform> ();
				foreach (Transform t in children) {
					if (t.gameObject.GetComponent<Renderer> ())
						t.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");
				}
				if (g.GetComponent<Renderer> ())
					g.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");


			}

			selected.RemoveRange (0, selected.Count);

		}
		
	}

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log (other.transform.parent.parent.name);
		if (other.transform.parent.parent.name == "Workbench") {
			//Debug.Log (selectionMode);
			//Debug.Log(triggerIsDown);
			if (selectionMode == "DELETE" ||selectionMode == "GRAB" || selectionMode == "SCALE" || selectionMode == "CREATE" || (selectionMode == "FACES" && triggerIsDown)) {
				if (other.gameObject.name.Contains ("Vertex") || (selectionMode == "DELETE" || selectionMode == "GRAB" || selectionMode == "SCALE")) {
					if (!selected.Contains (other.gameObject)) {
						selected.Add (other.gameObject);

						Transform[] children = other.gameObject.GetComponentsInChildren<Transform> ();

						if ((wholeObject && (selectionMode == "DELETE" || selectionMode == "GRAB")) || selectionMode == "SCALE"  )
							children = other.gameObject.transform.parent.GetComponentsInChildren<Transform> ();

						if (!wholeObject && selectionMode == "GRAB" && !other.gameObject.name.Contains ("Vertex"))
							return;

						foreach (Transform t in children) {
							if (t.gameObject.GetComponent<Renderer> () &&t.gameObject.GetComponent<LineRenderer> () == null  )
								t.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Mobile/Particles/Additive");
						}
						if (other.gameObject.GetComponent<Renderer> ())
							other.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Mobile/Particles/Additive");



					}
				}
				//Debug.Log ("collision started");
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.parent.parent.name == "Workbench") {
			if (selectionMode == "DELETE" ||selectionMode == "CREATE" || selectionMode == "GRAB" || selectionMode == "SCALE") {
				if (selected.Contains (other.gameObject)) {
					selected.Remove (other.gameObject);

					Transform[] children = other.gameObject.GetComponentsInChildren<Transform> ();
					if ((wholeObject && (selectionMode == "DELETE" || selectionMode == "GRAB")) || selectionMode == "SCALE"  )
						children = other.gameObject.transform.parent.GetComponentsInChildren<Transform> ();


					foreach (Transform t in children) {
						if (t.gameObject.GetComponent<Renderer> ())
							t.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");
					}
					if (other.gameObject.GetComponent<Renderer> ())
						other.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");


				}
			}
		}
	}


}
