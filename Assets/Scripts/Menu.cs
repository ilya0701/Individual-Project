using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public UnityEngine.UI.Button Button;
    private bool main = true;
    public GameObject Panel;
    public GameObject Scripts;

    private DirectoryInfo info;
    private FileInfo[] fileInfo;

    // Use this for initialization
    void Start () {

        string path = "";
        if (main)
            path = "C:/Users/ilyar/Desktop/ip/Individual-Project/Assets/Resources";

        CreateMenu(path);

       /* UnityEngine.UI.Button myButton = Instantiate(Button);
        myButton.transform.SetParent(Panel.transform);
        float z = 0 - myButton.transform.position.z;
        myButton.transform.Translate(0,0,z);
        myButton.onClick.AddListener(delegate () { Scripts.GetComponent<ObjectGenerate> ().Generate("Props/Table_A"); });

        UnityEngine.UI.Button myButton1 = Instantiate(Button);
        myButton1.transform.SetParent(Panel.transform);
        z = 0 - myButton1.transform.position.z;
        myButton1.transform.Translate(0, 0, z);
        myButton1.onClick.AddListener(delegate () { Scripts.GetComponent<ObjectGenerate>().Generate("Props/Table_A"); });*/

    }
	
	// Update is called once per frame
	void Update () {

        
	
	}

    void CreateMenu(string path)
    {
        Button[] buttons = FindObjectsOfType(typeof(Button)) as Button[];
        foreach (var button in buttons)
            Destroy(button);

        info = new DirectoryInfo(path);
        fileInfo = info.GetFiles();

        foreach (var file in fileInfo)
        {

            FileAttributes attr = File.GetAttributes(path + "/" + file.Name);
            //  if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            //  {
            UnityEngine.UI.Button myButton = Instantiate(Button);
            myButton.transform.SetParent(Panel.transform);
            float z = 0 - myButton.transform.position.z;
            myButton.transform.Translate(0, 0, z);
            Text text = myButton.transform.FindChild("Text").GetComponent<Text>();
            text.text = removeExtension(file.Name);

            Button temp = null;

            buttons = FindObjectsOfType(typeof(Button)) as Button[];

            foreach (var button in buttons)
                if (button.transform.FindChild("Text").GetComponent<Text>().text == removeExtension(file.Name))
                {
                    temp = button;
                }

            temp.onClick.AddListener(delegate () { Scripts.GetComponent<Menu>().CreateMenu(path + "/" + removeExtension(file.Name)); });


            //   }

        }
    }

    string removeExtension(string fileName)
    {
        int lastIndex = 0;
        for (int i = 0; i < fileName.Length; i++)
        {
            if (fileName[i] == '.')
                lastIndex = i;
        }
        return fileName.Substring(0, lastIndex);
    }
}
