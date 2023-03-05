using System.IO;
using UnityEngine;

public class DistrictNamesGenerator : MonoBehaviour
{
    public string filePath;

    // Start is called before the first frame update
    void Start()
    {
        GenerateNames();
    }

    public void GenerateNames()
    {
        string[] names = null;
        bool isOkay = true;
        try
        {
            names = File.ReadAllLines(filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("Ошибка при создании имён Районов!\n(" + e + ")");
            isOkay = false;
        }

        if (isOkay)
        {
            District[] childs = GameObject.Find("World_map").GetComponentsInChildren<District>();

            int nameCounter = 0;
            for (int i = 0; i < childs.Length; i++)
            {
                if (nameCounter > names.Length)
                    break;
                while (names[nameCounter].Equals(""))
                {
                    ++nameCounter;
                }
                childs[i].name = names[nameCounter++];
            }
        }
    }
}
