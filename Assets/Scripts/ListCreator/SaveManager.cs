using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public SaveData saveData;

    [SerializeField] private ListDataChar listCharacters;
    [SerializeField] private ListData listPanelList;
    [SerializeField] private ListData listToSave;

    [SerializeField] private CharacterList defaultList;
    [SerializeField] private ListPanel listPanel;

    private string savePath;

    private void Start()
    {
        savePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";

        if (!File.Exists(savePath))
        {
            CreateNewSave();
            CreateSave(true);
            LoadSelectedList();
            return;
        }

        string json = File.ReadAllText(savePath);

        SaveData save = JsonUtility.FromJson<SaveData>(json);
        saveData = save;

        if (save.lists.Count == 0)
        {
            CreateNewSave();
            CreateSave(true);
            LoadSelectedList();
            return;
        }

        for (int i = 0; i < save.lists.Count; i++)
        {
            Debug.Log("Found list: " + save.lists[i].name);
        }

        LoadSelectedList();
    }

    private void CreateSave(bool builtIn)
    {
        if (listCharacters.name == string.Empty || listCharacters.characters.Count == 0)
        {
            Debug.LogWarning("List is empty.");
            return;
        }
        else if (listCharacters.characters.Count < 3)
        {
            Debug.LogWarning("Too little amount of characters selected in the list. The game requires 3 or more characters in the list.");
            return;
        }

        int idxToCreateNewListAt = saveData.lists.FindIndex(l => l.name == listCharacters.name);

        if (idxToCreateNewListAt != -1)
            listToSave = saveData.lists[idxToCreateNewListAt];
        else
        {
            listToSave = new ListData();
            saveData.lists.Add(listToSave);
        }

        listToSave.characters = listCharacters.characters.Select(c => c.directory).ToList();
        listToSave.name = listCharacters.name;
        listToSave.builtIn = builtIn;

        if (saveData.lists[0] != null)
            listToSave.selected = true;

        Save();

        Debug.Log("Saved list: " + listToSave.name);
    }

    public void CreateNewSave()
    {
        listCharacters.name = defaultList.listName;

        listCharacters.characters = new List<Character>();

        for (int i = 0; i < defaultList.characters.Count; i++)
        {
            listCharacters.characters.Add(defaultList.characters[i]);
        }
    }

    public void SaveButton()
    {
        listPanelList = listPanel.openedList;
        SaveListToJSON();
    }

    public void SaveListToJSON()
    {
        if (listPanelList.name == string.Empty)
        {
            Debug.LogWarning("List has no name.");
            return;
        }
        else if (listPanelList.characters.Count == 0)
        {
            Debug.LogWarning("List is empty.");
            return;
        }
        else if (listPanelList.characters.Count < 3)
        {
            Debug.LogWarning("Too little amount of characters selected in the list. The game requires 3 or more characters in the list.");
            return;
        }

        int idxToCreateNewListAt = saveData.lists.FindIndex(l => l.name == listPanelList.name);

        if (idxToCreateNewListAt != -1)
            listToSave = saveData.lists[idxToCreateNewListAt];
        else
        {
            listToSave = new ListData();
            saveData.lists.Add(listToSave);
        }

        listToSave.characters = listPanelList.characters.ToList();
        listToSave.name = listPanelList.name;

        Save();
    }

    public void SaveCharToJSON()
    {
        if (listCharacters.name == string.Empty)
        {
            Debug.LogWarning("List has no name.");
            return;
        }
        else if (listCharacters.characters.Count == 0)
        {
            Debug.LogWarning("List is empty.");
            return;
        }
        else if (listCharacters.characters.Count < 3)
        {
            Debug.LogWarning("Too little amount of characters selected in the list. The game requires 3 or more characters in the list.");
            return;
        }

        int idxToCreateNewListAt = saveData.lists.FindIndex(l => l.name == listCharacters.name);

        if (idxToCreateNewListAt != -1)
            listToSave = saveData.lists[idxToCreateNewListAt];
        else
        {
            listToSave = new ListData();
            saveData.lists.Add(listToSave);
        }

        listToSave.characters = listCharacters.characters.Select(c => c.directory).ToList();
        listToSave.name = listCharacters.name;

        Save();
    }

    public void Save()
    {
        string save = JsonUtility.ToJson(saveData);

        using StreamWriter saveWriter = new(savePath);
        saveWriter.Write(save);
    }

    private void LoadSelectedList()
    {
        for (int i = 0; i < saveData.lists.Count; i++)
        {
            if (saveData.lists[i].selected)
            {
                listPanel.selectedList = saveData.lists[i];
                Debug.Log("Last selected list: " + saveData.lists[i].name);
            }
        }
    }
}
