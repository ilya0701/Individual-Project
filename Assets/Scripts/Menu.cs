using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    public UnityEngine.UI.Button Button;
    private bool main = true;
    public GameObject Panel;
    public GameObject Scripts;

	// Use this for initialization
	void Start () {

        Panel = GameObject.Find("ObjectList");

        UnityEngine.UI.Button myButton = Instantiate(Button);
        myButton.transform.SetParent(Panel.transform);
        float z = 0 - myButton.transform.position.z;
        myButton.transform.Translate(0,0,z);
        myButton.onClick.AddListener(delegate () { Scripts.GetComponent<ObjectGenerate> ().Generate("Props/Table_A"); });

        UnityEngine.UI.Button myButton1 = Instantiate(Button);
        myButton1.transform.SetParent(Panel.transform);
        z = 0 - myButton1.transform.position.z;
        myButton1.transform.Translate(0, 0, z);
        myButton1.onClick.AddListener(delegate () { Scripts.GetComponent<ObjectGenerate>().Generate("Props/Table_A"); });

    }
	
	// Update is called once per frame
	void Update () {

        
	
	}
}
