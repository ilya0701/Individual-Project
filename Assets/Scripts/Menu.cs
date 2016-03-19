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
    private DirectoryInfo[] dirInfo;
    private int resourcesPathLength;

    // Use this for initialization
    void Start () {

        string path = "";
        if (main)
        {
            path = "C:/Users/Ilya/Desktop/project/Individual-Project/Assets/Resources";
            resourcesPathLength = path.Length;
        }

        CreateMenu(path);
    }
	
	// Update is called once per frame
	void Update () {

        
	
	}

    void CreateMenu(string path)
    {

        foreach (var button in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            if (button.name == "Button(Clone)")
                button.SetActive(false);

        info = new DirectoryInfo(path);
        fileInfo = info.GetFiles();
        dirInfo = info.GetDirectories();

        BackButton(path);

        foreach (var dir in dirInfo)
        {
            UnityEngine.UI.Button myButton = Instantiate(Button);
            myButton.transform.SetParent(Panel.transform);
            float z = 0 - myButton.transform.position.z;
            myButton.transform.Translate(0, 0, z);
            Text text = myButton.transform.FindChild("Text").GetComponent<Text>();
            text.text = dir.Name;

            Button temp = null;
            Button[] buttons = FindObjectsOfType(typeof(Button)) as Button[];

            foreach (var button in buttons)
                if (button.transform.FindChild("Text").GetComponent<Text>().text == dir.Name)
                    temp = button;

            string t = path + "/" + dir.Name;
            temp.onClick.AddListener(delegate () { Scripts.GetComponent<Menu>().CreateMenu(t); });
        }

        foreach (var file in fileInfo)
        {
            if (file.Extension != ".meta")
            {
                UnityEngine.UI.Button myButton = Instantiate(Button);
                myButton.transform.SetParent(Panel.transform);
                float z = 0 - myButton.transform.position.z;
                myButton.transform.Translate(0, 0, z);
                Text text = myButton.transform.FindChild("Text").GetComponent<Text>();
                text.text = Substring(file.Name, '.');

                Button temp = null;
                Button[] buttons = FindObjectsOfType(typeof(Button)) as Button[];

                foreach (var button in buttons)
                    if (button.transform.FindChild("Text").GetComponent<Text>().text == Substring(file.Name, '.'))
                        temp = button;

                string t = path.Substring(resourcesPathLength + 1, path.Length - resourcesPathLength - 1) + "/" + Substring(file.Name, '.');
                temp.onClick.AddListener(delegate () { Scripts.GetComponent<ObjectGenerate>().Generate(t); });
            }
        }
    }

    void BackButton(string path)
    {
        UnityEngine.UI.Button myButton = Instantiate(Button);
        myButton.transform.SetParent(Panel.transform);
        float z = 0 - myButton.transform.position.z;
        myButton.transform.Translate(0, 0, z);
        Text text = myButton.transform.FindChild("Text").GetComponent<Text>();
        text.text = "Back";

        Button temp = null;
        Button[] buttons = FindObjectsOfType(typeof(Button)) as Button[];

        foreach (var button in buttons)
            if (button.transform.FindChild("Text").GetComponent<Text>().text == "Back")
                temp = button;

        string t = temp.transform.FindChild("Text").GetComponent<Text>().text;
        temp.onClick.AddListener(delegate () { Scripts.GetComponent<Menu>().CreateMenu(Substring(path, '/')); });
    }


    string Substring(string fileName, char symbol)
    {
        int lastIndex = fileName.Length;
        for (int i = 0; i < fileName.Length; i++)
        {
            if (fileName[i] == symbol)
                lastIndex = i;
        }
        return fileName.Substring(0, lastIndex);
    }
}
