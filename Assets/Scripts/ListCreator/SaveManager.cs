using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public SaveData saveData;

    [SerializeField] private int bothWinXP;
    [SerializeField] private int winXP;
    [SerializeField] private int loseXp;
    [SerializeField] private int bothLoseXP;

    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private ListPanel listPanel;
    [SerializeField] private CharactersPanel characterPanel;
    [SerializeField] private Slider xpBar;

    [SerializeField] private ListDataChar listCharacters;
    [SerializeField] private ListData listPanelList;
    [SerializeField] private ListData listToSave;

    [SerializeField] private CharacterList defaultList;
    [SerializeField] private CharacterList allCharacters;

    private string savePath;
    private float xpRequiredForNextLevel;

    private void Awake()
    {
        int version = PlayerPrefs.GetInt("Version");

        if (version != listPanel.mainPanel.gameManager.version)
            PlayerPrefs.SetInt("Version", listPanel.mainPanel.gameManager.version);

        Debug.Log("Playing on version " + version);

        mainPanel.versionNote.ChangeText("Version: " + mainPanel.gameManager.version);

        if (PlayerPrefs.GetInt("Level") == 0)
        {
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("XP", 0);
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
            ReplaceDefault();

        foreach (ListData t in save.lists)
            Debug.Log("Found list: " + t.name);

        LoadSelectedList();

        UnlockCharacters();
    }

    private void ReplaceDefault()
    {
        CreateNewSave();

        if (saveData.lists[0].builtIn)
            listToSave = saveData.lists[0];
        else
            saveData.lists.Insert(0, listToSave);

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
        if (indexToRemove == -1) 
            return;

        saveData.lists.RemoveAt(indexToRemove);
        listPanel.RefreshLists();
    }

    public void CreateNewSave()
    {
        listCharacters.name = defaultList.listName;

        listCharacters.characters = new List<Character>();

        foreach (Character t in defaultList.characters)
        {
            listCharacters.characters.Add(t);
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
        foreach (ListData t in saveData.lists.Where(t => t.selected))
        {
            listPanel.selectedList = t;
            Debug.Log("Last selected list: " + t.name);
        }
    }

    public void DuplicateList(int indexOfList)
    {
        List<string> charactersToDuplicate = saveData.lists[indexOfList].characters;

        listPanel.NewList();

        listPanel.openedList.characters = new List<string>(charactersToDuplicate.Count);

        foreach (string t in charactersToDuplicate)
        {
            listPanel.openedList.characters.Add(t);
        }
    }

    public void UnlockCharacters()
    {
        int currentLevel = PlayerPrefs.GetInt("Level");

        foreach (Character t in allCharacters.characters)
        {
            t.isUnlocked = currentLevel >= t.levelNeeded;
        }

        listPanel.charactersPanel.newCharacters.characters.Clear();

        foreach (Character t in allCharacters.characters.Where(t => t.levelNeeded == currentLevel))
        {
            listPanel.charactersPanel.newCharacters.characters.Add(t);
        }

        bool isOnListPanel = listPanel.hasListOpen && listPanel.menu == 1;

        if (characterPanel.hasLoaded)
        {
            if (characterPanel.loadedCategory == null)
                characterPanel.LoadCategoriesFade();
            else if (characterPanel.loadedCategory != null)
                characterPanel.RefreshPolaroids();
        }

        if (isOnListPanel)
            Invoke(nameof(ResetListPanelMenu), 0.8f);

    }

    public void ResetListPanelMenu()
    {
        listPanel.menu = 1;
    }

    public void WinResults(bool playerWon, bool opponentWon)
    {
        switch (playerWon)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            case true when !opponentWon:
                GiveXP(winXP);
                break;
            case true when opponentWon:
                GiveXP(bothWinXP);
                break;
            case false when opponentWon:
                GiveXP(loseXp);
                break;
            case false when !opponentWon:
                GiveXP(bothLoseXP);
                break;
        }
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
    }

    private static int LevelXpNeeded(int currentLevel)
    {
        if (currentLevel < 1)
            return 0;

        int neededXp = 0;
        for (int i = 0; i < currentLevel; i++)
            neededXp += (100 + 25 * i);

        return neededXp;
    }

    public void GiveXP(int xpAmount)
    {
        int newXp = PlayerPrefs.GetInt("XP") + xpAmount;
        PlayerPrefs.SetInt("XP", newXp);
        Debug.Log("Gave the player " + xpAmount + " of XP. Player now has " + newXp + " amount of XP.");

        if (newXp >= xpRequiredForNextLevel)
            LevelUp();

        SetXPBar();
    }

    private void SetXPBar()
    {
        int currentLevel = PlayerPrefs.GetInt("Level");
        xpRequiredForNextLevel = LevelXpNeeded(currentLevel);
        xpBar.minValue = LevelXpNeeded(currentLevel - 1);
        xpBar.maxValue = xpRequiredForNextLevel;

        xpBar.value = PlayerPrefs.GetInt("XP");
    }

    private void LevelUp()
    {
        int newLevel = PlayerPrefs.GetInt("Level") + 1;
        PlayerPrefs.SetInt("Level", newLevel);
        Debug.Log("Player leveled up and is now level " + newLevel);
        mainPanel.SetPlayerPolaroid(false, false, true);

        xpRequiredForNextLevel = LevelXpNeeded(newLevel);

        UnlockCharacters();

        mainPanel.hasLevelledUp = true;
    }

    public void SetLevel(int givenLevel)
    {
        PlayerPrefs.SetInt("Level", givenLevel);
        PlayerPrefs.SetInt("XP", LevelXpNeeded(givenLevel - 1));
        Debug.Log("Player's level set to " + givenLevel);
        mainPanel.SetPlayerPolaroid(false, false, true);

        xpRequiredForNextLevel = LevelXpNeeded(givenLevel);

        SetXPBar();

        UnlockCharacters();
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("XP", 0);
        SetXPBar();
        Debug.Log("Reset player's level and XP.");
        mainPanel.SetPlayerPolaroid(false, false, true);
        UnlockCharacters();
    }
}
