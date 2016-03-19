using UnityEngine;
using System.Collections;

public class ObjectGenerate : MonoBehaviour {

    private GameObject camera;

	// Use this for initialization
	void Start () {
        camera = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Generate(string objectName)
    {
        var obj = Instantiate(Resources.Load(objectName), camera.transform.position + (camera.transform.forward * 2) + new Vector3(0,0,-1), camera.transform.rotation) as GameObject;
        obj.AddComponent<ObjectManipulate>();
        obj.AddComponent<Dragable>();
        if (obj.GetComponent<MeshRenderer>() == null)
            obj.AddComponent<MeshRenderer>();
        if (obj.GetComponent<MeshCollider>() == null)
            obj.AddComponent<MeshCollider>();
        if (obj.GetComponent<Rigidbody>() == null)
        {
            obj.AddComponent<Rigidbody>();
            Rigidbody r = obj.GetComponent<Rigidbody>();
            r.isKinematic = false;
        }
        else
            obj.GetComponent<Rigidbody>().isKinematic = false;

    }
}
