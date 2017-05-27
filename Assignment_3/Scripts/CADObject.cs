using System;
using UnityEngine;
using System.Collections.Generic;


class HashSetEqualityComparer<GameObject>:IEqualityComparer<HashSet<GameObject>>
{
	public int GetHashCode(HashSet<GameObject> hashSet)
	{
		if(hashSet==null)
			return 0;
		int h = 0x14345843;//some arbitrary number
		foreach(GameObject elem in hashSet)
		{
			h=h+hashSet.Comparer.GetHashCode(elem);
		}
		return h;
	}

	public bool Equals(HashSet<GameObject> set1,HashSet<GameObject> set2)
	{
		if(set1==set2)
			return true;
		if(set1==null||set2==null)
			return false;
		return set1.SetEquals(set2);
	}
}


public class CADObject:MonoBehaviour
{
	public IEqualityComparer<HashSet<GameObject>> comp = new HashSetEqualityComparer<GameObject>();
	public Dictionary<HashSet<GameObject>, GameObject> edges;
	public List<GameObject> vertices = new List<GameObject> ();

	void Start () {
		
		edges = new Dictionary<HashSet<GameObject>, GameObject> (comp);
	}

	public void ResetEdges() {
		edges = new Dictionary<HashSet<GameObject>, GameObject> (comp);
	}

	// Update is called once per frame
	void Update () {

	}

	public int RemoveVertex(GameObject v) {
		Debug.Log ("Check vertex" + v.name);

		vertices.Remove (v);
		List<HashSet<GameObject>> keyList = new List<HashSet<GameObject>>(edges.Keys);
		foreach (HashSet<GameObject> key in keyList) {
			GameObject[] g = new GameObject [2];
			key.CopyTo (g);

			Debug.Log (g[0].name + "  " + g[1].name);
			if (g [0] == v || g [1] == v) {
				Debug.Log ("FOUDN EDGE TO DELTE");
				Destroy (edges [key]);
				edges.Remove (key);
			}

		}

		return 1;

	}

	public int Absorb (CADObject toAbsorb) {
		//move all vertices over
		this.vertices.AddRange(toAbsorb.vertices);
		//move all the edges over
		List<HashSet<GameObject>> keyList = new List<HashSet<GameObject>>(toAbsorb.edges.Keys);

		foreach (HashSet<GameObject> key in keyList) {
			this.edges.Add(key,toAbsorb.edges[key]);
		}
		//loop through all children, switch gameobjects over
		Transform[] children = toAbsorb.gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform t in children)
		{
			t.SetParent (gameObject.transform);
		}
		//delete old gameobject
		toAbsorb.gameObject.SetActive(false);

		return 1;
	}

	public void UpdateEdges() {
		List<HashSet<GameObject>> keyList = new List<HashSet<GameObject>>(edges.Keys);
		foreach (HashSet<GameObject> key in keyList) {
			GameObject edgeContainer = edges [key];
			//this.edges.Add(key,toAbsorb.edges[key]);
			LineRenderer li = edgeContainer.GetComponent<LineRenderer>();
			/*List<GameObject> lis =
			li.SetPosition (0, currentV.transform.position);
			li.SetPosition(1, target.transform.position);*/
			GameObject[] g = new GameObject [2];
			key.CopyTo (g);
			li.SetPosition (0, g[0].transform.position);
			li.SetPosition(1, g[1].transform.position);

		}
	}
}


