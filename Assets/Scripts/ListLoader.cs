using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ListLoader : MonoBehaviour
{
    public ListDataChar list;
    [SerializeField] private SaveData saveData;
    [SerializeField] private ListData listData;

    private string savePath;

    private void Start()
    {
        savePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";

        if (!File.Exists(savePath))
            return;

        using StreamReader reader = new StreamReader(savePath);
        string json = reader.ReadToEnd();

        SaveData save = JsonUtility.FromJson<SaveData>(json);
        Debug.Log(save.ToString());
        saveData = save;

        for (int i = 0; i < save.lists.Count; i++)
        {
            foreach (string characterDirectory in save.lists[i].characters)
            {
                Character character = Resources.Load<Character>(characterDirectory);

                Debug.Log("Found character: " + character.characterName);

                if (i == 0)
                {
                    list.characters.Add(character);
                    list.name = save.lists[i].name;
                }
            }
        }
    }

    public void SaveIntoJson()
    {
        int idxToCreateNewListAt = saveData.lists.FindIndex(l => l.name == list.name);

        if (idxToCreateNewListAt != -1)
            listData = saveData.lists[idxToCreateNewListAt];
        else
        {
            listData = new ListData();
            saveData.lists.Add(listData);
        }

        listData.characters = list.characters.Select(c => c.directory).ToList();
        listData.name = list.name;

        string save = JsonUtility.ToJson(saveData);

        using StreamWriter saveWriter = new(savePath);
        saveWriter.Write(save);
    }
}
