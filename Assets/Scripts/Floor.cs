using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Floor : MonoBehaviour
{

    private bool select = false;
    private Material material;
    private int count = 0;
    private Color red = new Color(255, 0, 0, 1);
    private Color normal = new Color(1, 1, 1, 1);

    // Use this for initialization
    void Start () {
        if (count==0)
            material = this.gameObject.GetComponent<MeshRenderer>().material;
        count++;
    }
	
	// Update is called once per frame
	void Update () {
        if (select && Input.GetKeyDown(KeyCode.Delete))
            Destroy(this.gameObject);
    }

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                MeshRenderer rend = this.gameObject.GetComponent<MeshRenderer>();
                if (!select)
                {
                    rend.material.color = red;
                    select = true;
                }
                else
                {
                    rend.material.color = normal;
                    select = false;
                }
            }
        }
    }

}
