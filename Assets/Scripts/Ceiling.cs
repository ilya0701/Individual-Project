using UnityEngine;
using System.Collections;

public class Ceiling : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject gameObject = GameObject.Find("GameObject");
		PlaneGen planeGen = gameObject.GetComponent<PlaneGen>();
		//Vector3 v = new Vector3 (0,0,0); 
		Vector3 v = new Vector3 (0,0,planeGen.height*2 + planeGen.depth); 
		transform.Translate(v);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
