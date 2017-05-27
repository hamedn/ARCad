/*
 * MODEL CITATIONS
 * 
 * 
 * */

using TheNextFlow.UnityPlugins;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class WorldControllerScript : MonoBehaviour {
	public Material blackMaterial;

	public GameObject workBench;
	public GameObject workBench2;
	public GameObject target;

	public GameObject createCanvas;
	public Dropdown createDropdown;
	public ToggleGroup editorToggles;
	public Button deleteButton;
	public Button triggerButton;
	public Button toolbarSlider;
	public Text toolbarText;

	List<GameObject> liveObjects = new List<GameObject> ();
	ButtonDown trigger = null;
	GameObject selected = null;

	public GameObject cyl;

	bool triggerIsDown = false;

	Quaternion cacheQuat = new Quaternion();
	Quaternion cacheQuat2 = new Quaternion();

	Vector3 cachePos = new Vector3();
	Vector3 cachePos2 = new Vector3();
	Vector3 cachePos3 = new Vector3();

	Vector3 pivotPos = new Vector3();

	Vector3 cacheScale = new Vector3();
	Vector3 cacheVertexScale = new Vector3();

	bool allWorkspace = false;

	public GameObject groundPivot;
	public GameObject toolbarPivot;
	public GameObject pivot = null;
	public Transform cacheParent;

	Vector3 wcache;
	Vector3 wcache3;
	Quaternion wcache2;

	bool initTrigger = false;

	public GameObject onToolbar = null;
	//public Object pref;
	// Use this for initialization


	public GameObject menuBomb;
	public GameObject menuWall;
	public GameObject menuHouse;
	public GameObject menuMan;

	public int createDropDownValue = 0;

	public GameObject currentObject = null;
	public GameObject currentV = null;
	public GameObject startV = null;

	GameObject previewLine;

	public GameObject minimap;
	bool minimapFound = false;
	string mapSelected = null;
	GameObject map = null;
	public bool tweening = false;

	public Canvas travelCanvas;
	public GameObject travelTip;
	public GameObject groundIndicator;
	public Button moveButton;

	public float mapScaleFactor = 1.0f;

	public bool currentlyMoving = false;
	public Canvas wholeObjectCanvas;
	public GameObject corner1;
	public GameObject corner2;

	public Text triggerText;
	public Text moveText;

	void Start () {
		
		deleteButton.gameObject.SetActive(false);
		toolbarSlider.gameObject.SetActive(false);
		trigger = triggerButton.GetComponent<ButtonDown> ();

		wcache = workBench.transform.localPosition;
		wcache3 = workBench.transform.localScale;
		wcache2 = workBench.transform.rotation;

		addShaderObject (menuMan);

		previewLine = new GameObject ();
		LineRenderer lineRenderer;
		lineRenderer = previewLine.AddComponent<LineRenderer>();
		lineRenderer.useWorldSpace = true;
		lineRenderer.startWidth = 1.0f;
		lineRenderer.material = blackMaterial;
		lineRenderer.widthMultiplier = 0.3f;
		//	lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(Color.black, Color.black);
		lineRenderer.numPositions = 2;
		previewLine.transform.parent = this.gameObject.transform;
		previewLine.SetActive (false);

		groundIndicator.SetActive (false);
		travelCanvas.gameObject.SetActive (false);

	}
	
	// Update is called once per frame
	void Update () {

		if (onToolbar != null && !allWorkspace) {
			cacheParent.GetComponent<CADObject> ().UpdateEdges ();
		}
		//Debug.Log(mapScaleFactor);
		//Debug.Log (groundIndicator.GetComponent<Renderer> ().bounds);
//if (findGroundIndicator () != null)
		//	Debug.Log (findGroundIndicator ().GetComponent<Renderer> ().bounds);
		//groundIndicator.SetActive (false);
		/*Renderer[] rendComp = groundIndicator.GetComponentsInChildren<Renderer> ();
		foreach (Renderer comp in rendComp) {
			comp.enabled = true;
		}
*/
		updateTravel ();

		updateTween ();



		bool stonesThere = false;
		bool showTrigger = false;
		string activeT = "";

		bool setSlider = false;
		{
			List<string> trackableList = new List<string> ();
			StateManager sm = TrackerManager.Instance.GetStateManager ();
			IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours ();



			if (selected != null || onToolbar != null) {
			
				foreach (TrackableBehaviour tb in activeTrackables) {
					trackableList.Add (tb.TrackableName);
				}

				if (trackableList.Contains ("Autumn") && trackableList.Contains ("tarmac")) {
					setSlider = true;
				}
				if (trackableList.Contains ("tarmac")) {
					stonesThere = true;
				}
			}

			foreach (TrackableBehaviour tb in activeTrackables) {
				trackableList.Add (tb.TrackableName);
			}

			if (trackableList.Contains ("stones") && !minimapFound) {
				//First time you found it
				resetWorkspace();
				minimapFound = true;
				minimap.SetActive (true);
			}
			if (!trackableList.Contains ("stones")) {
				minimapFound = false;
				minimap.SetActive (false);
			}

		
			if (trackableList.Contains ("acid")) {
				travelCanvas.gameObject.SetActive (true);
				createCanvas.gameObject.SetActive (false);
			} else {
				travelCanvas.gameObject.SetActive (false);
				createCanvas.gameObject.SetActive (true);
				if (target.GetComponent<WandScript> ().selectionMode.Equals("GRAB") || target.GetComponent<WandScript> ().selectionMode.Equals("DELETE")) {
					wholeObjectCanvas.gameObject.SetActive(true);
				}
				else {
					wholeObjectCanvas.gameObject.SetActive(false);
				}
			}


			var t2 = editorToggles.ActiveToggles ();
			foreach (Toggle ts in t2) {
				activeT = ts.name;
			}

			bool activeLine = false;
			if (trackableList.Contains ("tarmac") && (activeT == "Create" || activeT == "Rotate"  || activeT=="Grab"|| activeT=="Scale"|| activeT=="Delete") && trackableList.Contains("Legos")) {
				showTrigger = true;

				if (currentV != null) {
					activeLine = true;
					if (!previewLine.activeSelf) {
						previewLine.SetActive (true);
					}

					LineRenderer li = previewLine.GetComponent<LineRenderer>();
					li.SetPosition (0, currentV.transform.position);
					li.SetPosition(1, target.transform.position);
				
				}
			} else if (trackableList.Contains ("tarmac") && selected != null) {
				showTrigger = true;
			}


			if (!activeLine && previewLine.activeSelf) {
				previewLine.SetActive (false);

			}

		//	Debug.Log ("Stones:" + trackableList.Contains ("tarmac"));
		//	Debug.Log (activeT);
		}
		triggerButton.gameObject.SetActive(showTrigger);





		if (selected != null) {

		





			if (!allWorkspace) {
				if (onToolbar == null) {
					toolbarText.text = "To Toolbar";
				} else {
					toolbarText.text = "To Ground";
				}
			} else {
				if (onToolbar == null) {
					toolbarText.text = "To Toolbar";
				} else {
					toolbarText.text = "To Ground";
				}
			}
		}

		selectionLoop();





		toolbarSlider.gameObject.SetActive(setSlider);
		target.GetComponent<WandScript> ().triggerIsDown = trigger.mouseDown;
		target.GetComponent<WandScript> ().triggerIsVisible = showTrigger;
		//Debug.Log (trigger.mouseDown);

		if (onToolbar != null) {
			if (onToolbar.GetComponent<CADObject> ()) {
				onToolbar.GetComponent<CADObject> ().UpdateEdges ();
			}
		}
		if (selected != null) {
			if (selected.GetComponent<CADObject> ()) {
				selected.GetComponent<CADObject> ().UpdateEdges ();
				if (target.GetComponent<WandScript> ().selectionMode == "SCALE") {

					//Vector3 deltaPos = cyl.transform.position - cachePos;
					float dist = 1 + (System.Math.Abs (target.transform.position.y - cachePos.y)) / 2.0f;
					//Using this code: https://forum.unity3d.com/threads/scale-around-point-similar-to-rotate-around.232768/
					float RS = (1.0f / dist);
					if (cachePos.y < target.transform.position.y)
						RS = dist;

					Vector3 C = cachePos2 - pivotPos; // diff from object pivot to desired pivot/origin
					Vector3 FP = (C * RS) + pivotPos;

					selected.transform.localScale = cacheScale * RS;
					//selected.gameObject.transform.parent.transform.localScale = 
					selected.transform.position = FP;


					Transform[] children = selected.GetComponentsInChildren<Transform> ();
					foreach (Transform t in children) {
						if (t.gameObject.name.Contains ("Vertex")) {
							t.localScale = cacheVertexScale * (1 / RS);
						}
					}

					Debug.Log (selected.transform.localScale);
				}
			}
		}

		if (target.GetComponent<WandScript> ().selectionMode.Equals("CREATE")) {
			triggerText.text = "Create Vertex";

			if (target.GetComponent<WandScript> ().selected.Count > 0 && currentV != null) {
				triggerText.text = "End Creation";
			} else if (target.GetComponent<WandScript> ().selected.Count > 0 && currentV == null) {
				triggerText.text = "Extend Creation";
			}

		}


	}

	public GameObject findGroundIndicator() {
		if (map == null)
			return null;
		GameObject wbClone = map;
		foreach (Transform chi in wbClone.transform) {
			if (chi.name.Contains ("GroundIndicator"))
				return chi.gameObject;
		}

		return null;
	}

	public void toggleTravel() {
		if (!currentlyMoving) {
			beginMove ();
			moveText.text = "Stop Moving";

		} else {

			moveText.text = "Move";
			currentlyMoving = false;
			//cachePos = null;
			//cachePos2 = null;
			if (minimapFound)
				resetWorkspace ();
			return;

		}

	}

	public void updateTravel() {

		if (currentlyMoving) {
			Vector3 dif = travelTip.transform.position - cachePos;
			//Debug.Log (dif);
			//findGroundIndicator().transform.position = cachePos2 + dif;
			workBench.transform.position = cachePos3 + dif * (1.0f/mapScaleFactor);
			resetWorkspace ();
		}

	}


	public void beginMove() {
		Debug.Log("begin move pushed");
		deleteButton.gameObject.SetActive(true);
		currentlyMoving = true;
		cachePos = travelTip.transform.position;
		//cachePos2 = findGroundIndicator().transform.position;
		cachePos3 = workBench.transform.position;

	}

	public void resetWorkspace () {
		groundIndicator.SetActive (true);
		GameObject copyIndicator = Instantiate(groundIndicator, groundIndicator.transform.position, groundIndicator.transform.rotation,groundIndicator.transform.parent) as GameObject;            
		groundIndicator.SetActive (false);
		copyIndicator.transform.parent = workBench.transform;



		Vector3 wCache1 = workBench.transform.localScale;
		Vector3 wCache2 = workBench.transform.position;
		workBench.transform.localScale = new Vector3 (1, 1, 1);
		workBench.transform.position = new Vector3 ();

	
		var childrenz = new List<GameObject>();
		foreach (Transform child in minimap.transform) childrenz.Add(child.gameObject);
		childrenz.ForEach(child => Destroy(child));


		//Debug.Log("clicked");
		float minx = 0; float maxx = 0; float miny = 0; float maxy = 0; float minz = 0; float maxz = 0;
		bool first = true;
		Dictionary<string,string[]> lineToVertices = new Dictionary<string,string[]> ();

		for(int i = 0; i < workBench.transform.childCount; i++)
		{                
			GameObject child = workBench.transform.GetChild (i).gameObject;
			if (child.GetComponent<CADObject> () != null) {
				
				List<HashSet<GameObject>> keyList = new List<HashSet<GameObject>>(child.GetComponent<CADObject>().edges.Keys);
				foreach (HashSet<GameObject> key in keyList) {
					GameObject edgeContainer = child.GetComponent<CADObject>().edges [key];
					GameObject[] g = new GameObject [2];
					key.CopyTo (g);
					lineToVertices[edgeContainer.name] = new string[2];
					lineToVertices [edgeContainer.name] [0] = g [0].name;
					lineToVertices [edgeContainer.name] [1] = g [1].name;
				}

				List<GameObject> verts = new List<GameObject> ();
				verts.AddRange (child.GetComponent<CADObject> ().vertices);
				verts.Add (corner1);
				verts.Add (corner2);

				foreach (GameObject vertex in verts) {
					Vector3 size = vertex.transform.position;

					//Debug.Log (size);
					if (first) {
						first = false;
						minx = size.x; maxx = size.x; miny = size.z; maxy = size.z; maxz = size.z; minz = size.z;
					}
					if (size.x < minx)
						minx = size.x;
					if (size.x > maxx)
						maxx = size.x;
					if (size.z < miny)
						miny = size.z;
					if (size.z > maxy)
						maxy = size.z;
				}

			}

		}

		float lenx = maxx - minx; float leny = maxy - miny;

		//Debug.Log (lenx);
		//Debug.Log (leny);
		//Debug.Log(maxx); Debug.Log(minx); Debug.Log(maxy); Debug.Log(miny);

		map = Instantiate(workBench, minimap.transform.position, minimap.transform.rotation, minimap.transform) as GameObject;            

		float maxLength = 5.0f;
		float multiplier = 1.0f;
		if (lenx > maxLength || leny > maxLength) {
			float length = maxLength;
			if (lenx < leny) {
				map.transform.localScale *= (length / leny);
				multiplier = (length / leny);
			} else {
				map.transform.localScale *= (length / lenx);
				multiplier = (length / lenx);
			}
		}


		//mapScaleFactor = (1.0f / multiplier);

		//This updates all the edges
		for (int i = 0; i < map.transform.childCount; i++)
		{                
			GameObject child = map.transform.GetChild (i).gameObject;
			if (child.GetComponent<CADObject> () != null) {
				List<GameObject> toDestroy = new List<GameObject> ();
				List<GameObject> toAdd = new List<GameObject> ();
				child.GetComponent<CADObject> ().ResetEdges ();

				if (mapSelected != null && child.name.Equals(mapSelected)) {
					addShaderObject (child);
				} else {
					removeShaderObject (child);
				}

				foreach (Transform g in child.transform) {
					if (g.name.Contains("Line")) {
							string[] children = lineToVertices [g.name];
							Transform v1 = null;
							Transform v2 = null;
							foreach (Transform g1 in child.transform) {
								if (g1.name == children [0])
									v1 = g1;
								else if (g1.name == children [1])
									v2 = g1;
							}
							
							toDestroy.Add (g.gameObject);

							GameObject lineContainer = new GameObject ();
							lineContainer.name = "Line" + RandomString (10);
							LineRenderer lineRenderer = createLineRenderer (lineContainer, v1.gameObject, v2.gameObject, multiplier);
							//lineContainer.transform.parent = child.transform;
							lineRenderer.useWorldSpace = false;
							//lineContainer.transform.parent = map.transform;
							toAdd.Add(lineContainer);

							HashSet<GameObject> hash = new HashSet<GameObject> ();
							hash.Add (v1.gameObject);
							hash.Add (v2.gameObject);

							child.GetComponent<CADObject> ().edges.Add (hash, lineContainer);

					}


				}

				foreach (GameObject g in toDestroy) {
					Destroy (g);
				}
				foreach (GameObject g in toAdd) {
					g.transform.parent = child.transform;
				}

			}
			Renderer[] rendComp = child.GetComponentsInChildren<Renderer> ();
			foreach (Renderer comp in rendComp) {
				comp.enabled = true;
			}

			MeshCollider[] meshComp = child.GetComponentsInChildren<MeshCollider> ();
			foreach (MeshCollider comp in meshComp) {
				comp.enabled = true;
			}

		}



		//Vector3 wCache1 = workBench.transform.localScale;
		//Vector3 wCache2 = workBench.transform.position;
		workBench.transform.localScale = wCache1;
		workBench.transform.position = wCache2;

		Destroy (copyIndicator);
	}

	public void switchObjects() {
		//Debug.Log(selected.transform.parent.name);


		if (onToolbar == null) {
			//add something to toolbar
			if (!allWorkspace) {
				selected.transform.SetParent (workBench2.transform);
				onToolbar = selected.gameObject;
			} else {
				selected.transform.parent.SetParent (workBench2.transform.parent);
				onToolbar = selected.transform.parent.gameObject;
			}


		} else {
			//remove something from toolbar
			if (onToolbar.name == "Workbench") {
				onToolbar.transform.SetParent (groundPivot.transform.parent);
			} else {
				onToolbar.transform.SetParent (workBench.transform);
			}
			onToolbar = null;
		}


	}


	public void toggleWorkspace() {
		allWorkspace = !allWorkspace;
		target.GetComponent<WandScript> ().wholeObject = allWorkspace;

	}

	void selectionLoop()
	{


		bool uiTouched =false;

		foreach (Touch touch in Input.touches)
		{
			int id = touch.fingerId;
			if (EventSystem.current.IsPointerOverGameObject(id))
			{
				// ui touched
				uiTouched = true;
			}
		}
			
		if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !uiTouched )
		{

			var cams = Camera.allCameras;
			Camera mainCam = null;
			foreach (Camera c in cams)
			{
				if (c.enabled)
					mainCam = c;
			}      
			Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool found = false;
			if (Camera.main.enabled && Physics.Raycast(ray, out hit))
			{
				/*selection*/
				GameObject h = hit.collider.transform.gameObject;
				Debug.Log (h.GetType ());
				Debug.Log ((h.transform.parent.parent.parent.name == "Minimap"));
				if (h.transform.parent.parent.parent.name == "Minimap") {
					Debug.Log ("selecting object");
					selectObject (h.transform.parent.gameObject);
					found = true;
					UpdateMapHighlight ();
				}

				/*
				if (liveObjects.Contains (h)) {
					//Debug.Log (h.GetType ());
					//Debug.Log ((h != selected && selected != null));
					if (h != selected && selected != null) {
						deselectObject (selected);
					}
					selectObject (h);
					found = true;
				} else if (h.transform.parent.name == "menu") {

					removeShaderObject (menuMan);
					removeShaderObject (menuBomb);
					removeShaderObject (menuWall);
					removeShaderObject (menuHouse);

					addShaderObject (h);


					

					if (h == menuMan) {
						createDropDownValue = 0;
					}
					if (h == menuBomb) {
						createDropDownValue = 1;
					}
					if (h == menuWall) {
						createDropDownValue = 3;
					}
					if (h == menuHouse) {
						createDropDownValue = 2;
					}

				}
			*/
			}

			/*
			if (!found)
				deselectObject (selected);*/
		}


	}

	void selectObject(GameObject obj)
	{
		if (tweening)
			return;


		deleteButton.gameObject.SetActive(true);

		mapSelected = obj.name;

	//	foreach (Transform t2 in obj.transform) {

	//	}
		Debug.Log ("Trying to find object" + obj.name);

		GameObject target = null;

		foreach (Transform child in workBench.transform) {
			if (child.name.CompareTo(obj.name) == 0) {
				Debug.Log ("Selcted object" + obj.name);
				target = child.gameObject;
			}

			Debug.Log ("checked against" + child.name);
			//Debug.Log (transform.name.CompareTo (obj.name));
		}



		float minx = 0; float maxx = 0; float miny = 0; float maxy = 0; float minz = 0; float maxz = 0;
		bool first = true;

		List<Vector3> posList = new List<Vector3> ();

		float minyy = 0;
		foreach (GameObject vertex in target.GetComponent<CADObject>().vertices) {
			Vector3 size = vertex.transform.position;

			if (first) {
				first = false;
				minx = size.x; maxx = size.x; miny = size.z; maxy = size.z; maxz = size.z; minz = size.z;
				minyy = size.y;
			}
			if (size.x < minx)
				minx = size.x;
			if (size.x > maxx)
				maxx = size.x;
			if (size.z < miny)
				miny = size.z;
			if (size.z > maxy)
				maxy = size.z;

			minyy = Math.Min (size.y, minyy);

			posList.Add (size);

			Debug.Log ("Used this size" + size);
		}
		float lenx = Math.Abs(maxx - minx); float leny = Math.Abs(maxy - miny);

		/*
		List<Vector3> posList = new List<Vector3> ();
		Transform[] chil = selected.GetComponentsInChildren<Transform> ();
		foreach (Transform t in chil) {
			if (t.gameObject.name.Contains ("Vertex")) {
				
				cacheVertexScale = t.localScale;
			}
		}*/
		pivotPos = GetMeanVector(posList);
		pivotPos.y = minyy - 0.05f;
		groundPivot.transform.position = pivotPos;
		workBench.transform.SetParent (groundPivot.transform);


		tweening = true;
		float length = 5.0f;
		//Debug.Log (lenx);
		//Debug.Log (leny);

		float factor = 1.0f;

		if(lenx < leny)
		{
			Vector3 scale = groundPivot.transform.localScale * (length / lenx);                                                                               
			iTween.ScaleTo(groundPivot, scale, 1f);
			factor = (length / lenx);
			Debug.Log (1);
			Debug.Log (scale);
		}else
		{
			Vector3 scale = groundPivot.transform.localScale * (length / leny);
			iTween.ScaleTo(groundPivot, scale, 1f);
			factor = (length / leny);
			Debug.Log(2);
			Debug.Log (scale);
		}

		mapScaleFactor = (1.0f / factor);


		for (int i = 0; i < workBench.transform.childCount; i++) {                
			GameObject child = workBench.transform.GetChild (i).gameObject;
			if (child.GetComponent<CADObject> () != null) {
				/*foreach (GameObject vert in child.GetComponent<CADObject> ().vertices) {
				//	vert.transform.scale
					Debug.Log(factor);
					Debug.Log(new Vector3(1,1,1) * (1/factor));
					iTween.ScaleBy(vert,new Vector3(1.0f* (1/factor),1.0f* (1/factor),1.0f* (1/factor)) , 1f);
				}*/
			}

		}

		iTween.MoveTo (groundPivot, iTween.Hash("time",1f,
			"x",0f,
			"y",0f,
			"z",0f,
			"oncompletetarget",this.gameObject,
			"oncomplete","OnComplete"));

	/*	Vector3 pos = build.transform.GetChild(i).transform.position;
		Vector3 groundpos = editground.transform.position;
		//build.transform.position += (groundpos - pos);
		Vector3 t = build.transform.position + (groundpos - pos);
		iTween.MoveTo(build, t, 2);




		groundPivot.transform.position = 
*/



	}

	public void updateTween() {
		if (tweening || currentlyMoving) {
			for (int i = 0; i < workBench.transform.childCount; i++) {                
				GameObject child = workBench.transform.GetChild (i).gameObject;
				if (child.GetComponent<CADObject> () != null) {
					child.GetComponent<CADObject> ().UpdateEdges ();
					//Debug.Log ("update edges");
				}

			}
		}

	}

	public void OnComplete() {
		Debug.Log("tweeeeeeeeeeen completeted");
		tweening = false;

		workBench.transform.SetParent (groundPivot.transform.parent);

		resetWorkspace ();
	}

	void deselectObject(GameObject obj)
	{
		deleteButton.gameObject.SetActive(false);
		if (obj != null) {
			//Debug.Log ("deselecting object");
			Transform[] children = obj.GetComponentsInChildren<Transform> ();
			foreach (Transform t in children) {
				if (t.gameObject.GetComponent<Renderer> ())
					t.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");
			}
			if (obj.GetComponent<Renderer> ())
				obj.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");

			selected = null;
		}
	}

	void addShaderObject(GameObject obj) {
		if (obj != null) {
			//Debug.Log ("deselecting object");
			Transform[] children = obj.GetComponentsInChildren<Transform> ();
			foreach (Transform t in children) {
				if (t.gameObject.GetComponent<Renderer> () && t.gameObject.GetComponent<LineRenderer> () == null)
					t.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Mobile/Particles/Additive");
			}
			if (obj.GetComponent<Renderer> ())
				obj.GetComponent<Renderer> ().material.shader = Shader.Find ("Mobile/Particles/Additive");

		}

	}



	void removeShaderObject(GameObject obj) {
		if (obj != null) {
			//Debug.Log ("deselecting object");
			Transform[] children = obj.GetComponentsInChildren<Transform> ();
			foreach (Transform t in children) {
				if (t.gameObject.GetComponent<Renderer> ())
					t.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");
			}
			if (obj.GetComponent<Renderer> ())
				obj.GetComponent<Renderer> ().material.shader = Shader.Find ("Custom/NewSurfaceShader");
		}

	}



	public void UpdateMode() {
	//	Debug.Log("hide stuff");
		var t = editorToggles.ActiveToggles();

		foreach (Toggle ts in t)
		{
			if (ts.name == "Create") {
				target.GetComponent<WandScript> ().selectionMode = "CREATE";
				triggerText.text = "Create Vertex";
			} else if (ts.name == "Rotate") {
				target.GetComponent<WandScript> ().selectionMode = "FACES";
				triggerText.text = "Hold to Connect Vertices";
			} else if (ts.name == "Grab") {
				target.GetComponent<WandScript> ().selectionMode = "GRAB";
				triggerText.text = "Grab";

			}
			else if (ts.name == "Scale") {
				target.GetComponent<WandScript> ().selectionMode = "SCALE";
				triggerText.text = "Scale";
			}
			else if (ts.name == "Delete") {
				target.GetComponent<WandScript> ().selectionMode = "DELETE";
				triggerText.text = "Delete";
			}
			//Debug.Log (ts.name);
		}

	}
		
	void Create() {
			List<string> trackableList = new List<string> ();
		StateManager sm = TrackerManager.Instance.GetStateManager ();
		IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours ();
		foreach (TrackableBehaviour tb in activeTrackables) {
			trackableList.Add (tb.TrackableName);
			//Debug.Log("Trackable: " + tb.TrackableName);
		}

		if (trackableList.Contains ("Legos") && trackableList.Contains ("tarmac")) {
			UnityEngine.Object pref = null;

			pref = Resources.Load ("Vertex");

			/*
			if (createDropDownValue == 0)
				pref = Resources.Load ("BombermanPrefab");
			if (createDropDownValue == 1)
				pref = Resources.Load ("BombFinal");
			if (createDropDownValue == 2)
				pref = Resources.Load ("House");
			if (createDropDownValue == 3)
				pref = Resources.Load ("Wall");
*/

			bool startObject = false;
			bool endObject = false;
			if (currentObject == null) {
				if (target.GetComponent<WandScript> ().selected.Count > 0) {
					GameObject continueV = target.GetComponent<WandScript> ().selected [0];
					currentObject = continueV.transform.parent.gameObject;
					startV = continueV;
					currentV = continueV;
					return;
				}


				currentObject = new GameObject ();
				currentObject.name = "Creation" + RandomString (10);
				currentObject.AddComponent<CADObject> ();
				triggerButton.GetComponent<ButtonDown> ();
				startObject = true;
			} else {
				if (target.GetComponent<WandScript> ().selected.Count > 0) {
					if (target.GetComponent<WandScript> ().selected.Contains(currentV)) {

						currentV = null;
						startV = null;
						currentObject = null;
						if (trackableList.Contains ("stones")) 
							resetWorkspace();

						/*
						Debug.Log ("-error form loop");
						Action action = new Action (Nothing);

						MobileNativePopups.OpenAlertDialog ("Error", "You must create a vertex elsewhere", "Cancel", action);
						return;*/
						return;
					}
					endObject = true;
				}
			}



			GameObject newObject;
			//Debug.Log (pref.name);
			if (!endObject) {
				newObject = (Instantiate (pref, target.transform.position, new Quaternion ()) as GameObject);

				newObject.name = "Vertex" + RandomString (10);

			
				foreach (TrackableBehaviour tb in activeTrackables) {
					if (tb.TrackableName == "Legos") {
						newObject.transform.rotation = tb.gameObject.transform.rotation;
					}
				}
				
				newObject.transform.parent = currentObject.transform;
				currentObject.transform.parent = workBench.transform;
				liveObjects.Add (newObject);

				currentObject.GetComponent<CADObject> ().vertices.Add (newObject);
			} else {
				newObject = target.GetComponent<WandScript> ().selected[0];
			}

			if (startObject) {
				startV = currentObject;
			} else {
				GameObject lineContainer = new GameObject ();
				lineContainer.name = "Line" + RandomString (10);
				LineRenderer lineRenderer = createLineRenderer(lineContainer, currentV, newObject);
				HashSet<GameObject> line = new HashSet<GameObject> ();
				line.Add (currentV);
				line.Add (newObject);
				currentObject.GetComponent<CADObject> ().edges.Add (line, lineContainer);
				lineContainer.transform.parent = currentObject.transform;

				bool sameObject = (currentV.transform.parent.GetComponent<CADObject> () == newObject.transform.parent.GetComponent<CADObject> ());
				if (!sameObject) {
					newObject.transform.parent.GetComponent<CADObject> ().Absorb (currentV.transform.parent.GetComponent<CADObject> ());
				}
				Debug.Log ("Same object: " + sameObject);
			}

			currentV = newObject;

			if (endObject) {
				currentV = null;
				startV = null;
				currentObject = null;
				if (trackableList.Contains ("stones")) 
					resetWorkspace();


			}
			if (startObject) {
				startV = newObject;
			}


		}


	
	}

	private Vector3 GetMeanVector(List<Vector3> positions)
	{
		if (positions.Count == 0)
			return Vector3.zero;
		float x = 0f;
		float y = 0f;
		float z = 0f;
		foreach (Vector3 pos in positions)
		{
			x += pos.x;
			y += pos.y;
			z += pos.z;
		}
		return new Vector3(x / positions.Count, y / positions.Count, z / positions.Count);
	}
	
	public void TriggerButton() {
		//Debug.Log("hello world");
		var t = editorToggles.ActiveToggles();
		foreach (Toggle ts in t)
		{
			if (ts.name == "Create") {
				Create ();
			} else if (ts.name == "Rotate") {
				//target.GetComponent<WandScript> ().CreateFace ();
				//Debug.Log ("hiii");
			} else if (ts.name == "Grab") {
				GrabObject ();
			}else if (ts.name == "Scale") {
				ScaleObject ();

			
			}else if (ts.name == "Delete") {
				DeleteObjectNew ();


			}
			//Debug.Log (ts.name);
		}


	}

	public void ScaleObject() {
		if (selected == null) {
			if (target.GetComponent<WandScript> ().selected.Count > 0) {
				//add something to toolbar
				GameObject toSelect;
				toSelect = target.GetComponent<WandScript> ().selected [0].transform.parent.gameObject;
				selected = toSelect.gameObject;
				//toSelect.transform.SetParent (target.transform);
				cachePos = target.transform.position;
				cachePos2 = selected.gameObject.transform.position;
				cacheScale = selected.gameObject.transform.localScale;

				//pivotPos = selected.gameObject.transform.position;

				List<Vector3> posList = new List<Vector3> ();
				Transform[] chil = selected.GetComponentsInChildren<Transform> ();
				foreach (Transform t in chil) {
					if (t.gameObject.name.Contains ("Vertex")) {
						posList.Add (t.position);
						cacheVertexScale = t.localScale;
					}
				}
				pivotPos = GetMeanVector(posList);

				triggerText.text = "Stop Scaling";
			
					
			}
		} else {
			triggerText.text = "Scale";
			//remove something from toolbar
			//onToolbar.transform.SetParent (workBench.transform.parent);
			selected = null;
		}


	}

	public void UpdateMapHighlight (){
		if (map != null) {
			for (int i = 0; i < map.transform.childCount; i++) {                
				GameObject child = map.transform.GetChild (i).gameObject;
				if (child.GetComponent<CADObject> () != null) {
					if (mapSelected != null && child.name.Equals (mapSelected)) {
						addShaderObject (child);
					} else {
						removeShaderObject (child);
					}

				}
			}
		}

	}

	public void DeleteObjectNew() {
		if (target.GetComponent<WandScript> ().selected.Count > 0) {
			if (allWorkspace) {
				//add something to toolbar
				GameObject toSelect;
				toSelect = target.GetComponent<WandScript> ().selected [0].transform.parent.gameObject;
				Destroy (toSelect);
			} else {
				foreach (GameObject to in target.GetComponent<WandScript> ().selected) {
					GameObject toSelect;
					toSelect = to.transform.gameObject;
					CADObject ccc = to.transform.parent.GetComponent<CADObject> ();

					if (toSelect.name.Contains ("Vertex")) {
						ccc.RemoveVertex (toSelect);
					}
					Destroy (toSelect);


				}
			}
		}

		target.GetComponent<WandScript> ().selected = new List<GameObject> ();


	}


	public void GrabObject() {
		if (onToolbar == null) {
			if (target.GetComponent<WandScript> ().selected.Count > 0) {
				if (allWorkspace) {
					//add something to toolbar
					GameObject toSelect;
					toSelect = target.GetComponent<WandScript> ().selected [0].transform.parent.gameObject;

					toSelect.transform.SetParent (target.transform);
					onToolbar = toSelect.gameObject;

					triggerText.text = "Release";
				} else {
					GameObject toSelect;
					toSelect = target.GetComponent<WandScript> ().selected [0].transform.gameObject;
					if (!toSelect.name.Contains ("Vertex"))
						return;
					cacheParent = toSelect.transform.parent;
					toSelect.transform.SetParent (target.transform);
					onToolbar = toSelect.gameObject;

					triggerText.text = "Release";
				
				}
			}


		} else {
			//remove something from toolbar
			if (allWorkspace) {
				onToolbar.transform.SetParent (workBench.transform);
				onToolbar = null;
			} else {
				onToolbar.transform.SetParent (cacheParent);
				onToolbar = null;
			}

			triggerText.text = "Grab";
		}


	}

	public void DeleteObject() {
		iTween.ScaleTo(workBench, new Vector3(1,1,1), 1f);
		iTween.MoveTo (workBench, iTween.Hash ("time", 1f,
			"x", 0f,
			"y", 0f,
			"z", 0f,
			"oncompletetarget", this.gameObject,
			"oncomplete", "OnComplete"));


		tweening = true;

		GameObject s = selected;
		mapSelected = null;
		deselectObject(s);
		deleteButton.gameObject.SetActive(false);
		selected = null;

		UpdateMapHighlight ();
		//liveObjects.Remove (s);
	//	Destroy (s);

	}

	private LineRenderer createLineRenderer(GameObject lineContainer, GameObject currentV, GameObject newObject, float widthMulti = 1.0f) {
		LineRenderer lineRenderer;

		lineRenderer = lineContainer.AddComponent<LineRenderer>();
		lineRenderer.useWorldSpace = true;
		lineRenderer.startWidth = 1.0f;
		lineRenderer.material = blackMaterial;
		lineRenderer.widthMultiplier = 0.3f * widthMulti;

		//	lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(Color.black, Color.black);
		lineRenderer.numPositions = 2;
		lineRenderer.SetPosition (0, currentV.transform.position);
		lineRenderer.SetPosition (1, newObject.transform.position);
		newObject.AddComponent<MeshCollider> ();
		return lineRenderer;
	}


	public void Nothing() {

	}


	private static System.Random random = new System.Random();
	public static string RandomString(int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[random.Next(s.Length)]).ToArray());
	}


}
