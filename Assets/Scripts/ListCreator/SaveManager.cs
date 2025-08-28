using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public SaveData saveData;

    public List<float> levelXPNeeded;

    [SerializeField] private float bothWinXP;
    [SerializeField] private float winXP;
    [SerializeField] private float loseXp;
    [SerializeField] private float bothLoseXP;

    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private Slider xpBar;

    [SerializeField] private ListDataChar listCharacters;
    [SerializeField] private ListData listPanelList;
    [SerializeField] private ListData listToSave;

    [SerializeField] private CharacterList defaultList;
    [SerializeField] private ListPanel listPanel;

    private string savePath;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Version") != listPanel.mainPanel.gameManager.version)
            PlayerPrefs.SetInt("Version", listPanel.mainPanel.gameManager.version);

        Debug.Log("Playing on version " + PlayerPrefs.GetInt("Version"));

        mainPanel.versionNote.ChangeText("Version: " + mainPanel.gameManager.version);

        if (PlayerPrefs.GetInt("Level") == 0)
        {
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetFloat("XP", 0f);
        }

        SetXPBar();

        mainPanel.SpawnPosters();
    }

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

        if (save.lists[0].version != defaultList.version)
        {
            ReplaceDefault();
        }

        for (int i = 0; i < save.lists.Count; i++)
        {
            Debug.Log("Found list: " + save.lists[i].name);
        }

        LoadSelectedList();
    }

    private void ReplaceDefault()
    {
        CreateNewSave();

        listToSave = saveData.lists[0];

        listToSave.characters = defaultList.characters.Select(c => c.directory).ToList();
        listToSave.name = defaultList.name;
        listToSave.builtIn = true;
        listToSave.version = defaultList.version;

        Save();

        Debug.Log("Replaced default list with new list.");
    }

    private void CreateSave(bool builtIn)
    {
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
        listToSave.version = PlayerPrefs.GetInt("Version");

        if (saveData.lists[0] != null)
            listToSave.selected = true;

        Save();

        Debug.Log("Saved list: " + listToSave.name);
    }

    public void RemoveList(int indexToRemove)
    {
        if (indexToRemove != -1)
        {
            saveData.lists.RemoveAt(indexToRemove);
            listPanel.RefreshLists();
        }
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

    public void Save()
    {
        if (listPanel.openedList != null && !listPanel.openedList.builtIn)
            listPanel.openedList.version = PlayerPrefs.GetInt("Version");

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

    public void WinResults(bool playerWon, bool opponentWon)
    {
        if (playerWon && !opponentWon)
            GiveXP(winXP);
        else if (playerWon && opponentWon)
            GiveXP(bothWinXP);
        else if (!playerWon && opponentWon)
            GiveXP(loseXp);
        else if (!playerWon && !opponentWon)
            GiveXP(bothLoseXP);
    }

    public void GiveXP(float xpAmount)
    {
        if (levelXPNeeded.Count <= PlayerPrefs.GetInt("Level"))
        {
            Debug.Log("Player is already max level.");
            return;
        }

        PlayerPrefs.SetFloat("XP", PlayerPrefs.GetFloat("XP") + xpAmount);
        Debug.Log("Gave the player " + xpAmount + " of XP. Player now has " + PlayerPrefs.GetFloat("XP") + " amount of XP.");

        if (PlayerPrefs.GetFloat("XP") >= levelXPNeeded[PlayerPrefs.GetInt("Level")])
            LevelUp();

        SetXPBar();
    }

    private void SetXPBar()
    {
        if (PlayerPrefs.GetInt("Level") == 1)
            xpBar.minValue = 0;
        else
            xpBar.minValue = levelXPNeeded[PlayerPrefs.GetInt("Level") - 1];

        if (levelXPNeeded.Count <= PlayerPrefs.GetInt("Level"))
            xpBar.maxValue = PlayerPrefs.GetFloat("XP");
        else
            xpBar.maxValue = levelXPNeeded[PlayerPrefs.GetInt("Level")];

        xpBar.value = PlayerPrefs.GetFloat("XP");
    }

    private void LevelUp()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        Debug.Log("Player leveled up and is now level " + PlayerPrefs.GetInt("Level"));
        mainPanel.SetPlayerPolaroid(false, false, true);
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetFloat("XP", 0);
        SetXPBar();
        Debug.Log("Reset player's level and XP.");
        mainPanel.SetPlayerPolaroid(false, false, true);
    }
}
