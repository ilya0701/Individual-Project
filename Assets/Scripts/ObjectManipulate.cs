using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ObjectManipulate : MonoBehaviour
{

    private bool select = false;
    private Material material;
    private int count = 0;
    private Color red = new Color(15, 0, 0, 1);
    private Color normal = new Color(1, 1, 1, 1);
    private Camera camera;

    // Use this for initialization
    void Start()
    {
        camera = Camera.main;
        if (count == 0 && this.gameObject.GetComponent<MeshRenderer>()!=null)
            material = this.gameObject.GetComponent<MeshRenderer>().material;
        count++;
    }

    // Update is called once per frame
    void Update()
    {
        if (select && Input.GetKeyDown(KeyCode.Delete))
            Destroy(this.gameObject);

        if (!this.gameObject.name.Contains("Floor") && !this.gameObject.name.Contains("Ceiling") && !this.gameObject.name.Contains("Walls"))
        {
            if (select)
            {
                float temp = Input.GetAxis("Mouse ScrollWheel");
                if (this.gameObject.transform.localScale.x < 0.1 && temp < 0)
                    temp = 0;
                this.gameObject.transform.localScale += new Vector3(temp, temp, temp);
            }
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.Space))
            Deselect();

        if (!select)
            Deselect();

    }

    void OnMouseDown()
    {
        bool toSelect = false;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0) && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (!select)
                {
                    DeselectAll();
                    toSelect = Select();
                }
                else
                    toSelect = Deselect();

                select = toSelect;
            }
            else if (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (!select)
                    toSelect = Select();
                else
                    toSelect = Deselect();

                select = toSelect;
            }
        }
    }

    bool Select()
    {
        MeshRenderer[] rend = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rend.Length; i++)
        {
            Material[] mat = rend[i].materials;
            foreach (var m in mat)
                m.color = red;
        }
        return true;
    }

    bool Deselect()
    {
        MeshRenderer[] rend = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rend.Length; i++)
        {
            Material[] mat = rend[i].materials;
            foreach (var m in mat)
                m.color = normal;
        }
        return false;
    }

    void DeselectAll()
    {
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in objects)
            if (obj.GetComponent<ObjectManipulate>() != null)
                obj.GetComponent<ObjectManipulate>().select = false;
    }

}