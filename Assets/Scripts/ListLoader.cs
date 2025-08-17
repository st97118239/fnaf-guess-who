using System.IO;
using UnityEngine;

public class ListLoader : MonoBehaviour
{
    public ListData list;
    [SerializeField] private SaveData saveData;
    [SerializeField] private ListData listData;

    private void Start()
    {
        using StreamReader reader = new StreamReader(Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json");
        string json = reader.ReadToEnd();

        SaveData save = JsonUtility.FromJson<SaveData>(json);
        Debug.Log(save.ToString());
        saveData = save;

        for (int i = 0; i < save.lists.Count; i++)
        {
            foreach (string character in save.lists[i].characters)
            {
                Debug.Log("Found character: " + character);
            }
        }
    }

    public void SaveIntoJson()
    {
        saveData.lists.Add(listData);

        string save = JsonUtility.ToJson(saveData);

        using StreamWriter saveWriter = new(Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json");
        saveWriter.Write(save);
    }
}
