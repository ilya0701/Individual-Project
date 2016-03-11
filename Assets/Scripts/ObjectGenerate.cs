using UnityEngine;
using System.Collections;

public class ObjectGenerate : MonoBehaviour {

    private int count = 0;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Generate(string objectName)
    {
        Instantiate(Resources.Load(objectName));
    }
}
