using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ListPanel : MonoBehaviour
{
    public Game game;
    public SaveManager saveManager;
    public ListInfoPanel infoPanel;
    public ListData selectedList;
    public ListData openedList;
    public CharactersPanel charactersPanel;
    public CharacterClipboard characterClipboard;

    public int menu; // -1 = loading, 0 = listsMenu, 1 = listCharactersMenu
    public bool isInfoPanelShown;
    public bool fromCharacterPanel;
    public bool hasListOpen;
    public int maxCharacters = 96;

    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private GameObject listGrid;
    [SerializeField] private ListSettings listSettings;
    [SerializeField] private GameObject listNotePrefab;
    [SerializeField] private GameObject emptySlotPrefab;
    [SerializeField] private GameObject polaroidPrefab;

    [SerializeField] private Note backNote;
    [SerializeField] private Note leaveNote;
    [SerializeField] private Note selectNote;

    [SerializeField] private int emptySlotAmount;
    
    private List<ListNote> listNotes;
    private List<EmptySlot> emptySlots;
    private List<ListPolaroid> polaroids;
    private int listAmount;
    private bool hasLoaded;

    private void Awake()
    {
        if (emptySlotAmount == 0)
        {
            emptySlotAmount = 96; // Set emptySlotAmount to max otherwise we get an error
            Debug.LogWarning("emptySlotAmount is set to 0, please set an amount.");
        }
    }

    public void LoadPanel()
    {
        if (fromCharacterPanel)
            game.animator.SetTrigger("ListCharClose");
        else
            game.animator.SetTrigger("ListCreatorOpen");

        fromCharacterPanel = false;

        if (!hasLoaded)
            Invoke(nameof(SpawnFirst), 1);
    }

    public void ClosePanel()
    {
        game.animator.SetTrigger("ListCreatorClose");
    }

    private void PlayFadeAnim(bool shouldReverse, bool shouldDestroyChildren, bool shouldDestroySelf)
    {
        for (int i = 0; i < emptySlots.Count; i++)
        {
            emptySlots[i].Play(shouldReverse, shouldDestroyChildren, shouldDestroySelf);
        }
    }

    public void SpawnFirst()
    {
        hasLoaded = true;

        menu = -1;

        emptySlots = new List<EmptySlot>(emptySlotAmount);

        for (int i = 0; i < emptySlotAmount; i++)
        {
            GameObject slotObj = Instantiate(emptySlotPrefab, listGrid.transform);
            emptySlots.Add(slotObj.GetComponent<EmptySlot>());
            emptySlots[i].index = i;
        }

        backNote.Disable();
        selectNote.Disable();
        menu = 0;

        SpawnListNotes();
    }

    private void SpawnListNotes()
    {
        listAmount = saveManager.saveData.lists.Count;

        listNotes = new List<ListNote>(listAmount);

        for (int i = 0; i < listAmount + 1; i++)
        {
            GameObject slotObj = Instantiate(listNotePrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform);
            listNotes.Add(slotObj.GetComponent<ListNote>());
            if (i < listAmount)
                listNotes[i].LoadList(saveManager.saveData.lists[i], this, ListNoteType.Lists, i);
            else if (i == listAmount)
                listNotes[i].NewListButton(this, ListNoteType.AddList);
        }

        PlayFadeAnim(false, false, false);
    }

    public void OpenList(ListData list)
    {
        Debug.Log("Opened list: " + list.name);

        openedList = list;

        for (int i = 0; i < listNotes.Count; i++)
        {
            Destroy(listNotes[i].gameObject);
        }

        listNotes.Clear();

        for (int i = 0; i < emptySlots.Count; i++)
        {
            emptySlots[i].Reset();
        }

        OpenListCharactersMenu();
    }

    public void OpenListCharactersMenu()
    {
        polaroids = new List<ListPolaroid>(openedList.characters.Count);

        for (int i = 0; i < openedList.characters.Count; i++)
        {
            GameObject slotObj = Instantiate(polaroidPrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform);
            polaroids.Add(slotObj.GetComponent<ListPolaroid>());
            polaroids[i].Load(openedList.characters[i], this, i);
        }

        backNote.Enable();
        if (!openedList.selected && !mainPanel.isReady)
            selectNote.Enable();
        if (menu != 2)
            menu = 1;

        hasListOpen = true;

        PlayFadeAnim(false, false, false);
    }

    public void RefreshCharactersMenu()
    {
        PlayFadeAnim(true, true, false);

        Invoke(nameof(RefreshCharactersMenuSpawn), 0.7f);
    }

    private void RefreshCharactersMenuSpawn()
    {
        polaroids = new List<ListPolaroid>(openedList.characters.Count);

        for (int i = 0; i < openedList.characters.Count; i++)
        {
            GameObject slotObj = Instantiate(polaroidPrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform);
            polaroids.Add(slotObj.GetComponent<ListPolaroid>());
            polaroids[i].Load(openedList.characters[i], this, i);
        }

        PlayFadeAnim(false, false, false);
    }

    public void RefreshLists()
    {
        PlayFadeAnim(true, true, false);

        Invoke(nameof(RefreshListsSpawn), 0.7f);
    }

    private void RefreshListsSpawn()
    {
        listAmount = saveManager.saveData.lists.Count;

        listNotes = new List<ListNote>(listAmount);

        for (int i = 0; i < listAmount + 1; i++)
        {
            GameObject slotObj = Instantiate(listNotePrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform);
            listNotes.Add(slotObj.GetComponent<ListNote>());
            if (i < listAmount)
                listNotes[i].LoadList(saveManager.saveData.lists[i], this, ListNoteType.Lists, i);
            else if (i == listAmount)
                listNotes[i].NewListButton(this, ListNoteType.AddList);
        }

        PlayFadeAnim(false, false, false);
    }

    public void ShowInfoPanel(Character character)
    {
        infoPanel.Show(character);
    }

    public void ListCharactersBack()
    {
        PlayFadeAnim(true, true, false);

        hasListOpen = false;
        openedList = new();

        backNote.Disable();
        selectNote.Disable();
        menu = 0;

        polaroids.Clear();

        Invoke(nameof(SpawnListNotes), 0.7f);
    }

    public void BackNote()
    {
        if (menu == 1)
        {
            ListCharactersBack();
        }
    }

    public void SelectNote()
    {
        selectedList.selected = false;
        selectedList = openedList;
        selectedList.selected = true;
        Debug.Log("Selected list: " + selectedList.name);
        selectNote.Disable();
        saveManager.Save();
    }

    public void RemoveCharacterFromList(Character givenCharacter, int givenIndex)
    {
        Debug.Log("Removed " + givenCharacter.name + " from list " + openedList.name);
        openedList.characters.RemoveAt(givenIndex);

        saveManager.Save();

        RefreshCharactersMenu();

        if (menu == 2)
            charactersPanel.RecheckPolaroids();
    }

    public void AddCharacterToList(Character givenCharacter)
    {
        int isInList = openedList.characters.FindIndex(d => d == givenCharacter.directory);

        if (isInList != -1)
        {
            charactersPanel.RecheckPolaroids();
            Debug.Log("Character already added.");
            return;
        }

        Debug.Log("Added " + givenCharacter.name + " to list " + openedList.name);
        openedList.characters.Add(givenCharacter.directory);

        saveManager.Save();

        RefreshCharactersMenu();

        if (menu == 2)
            charactersPanel.RecheckPolaroids();
    }

    public void NewList()
    {
        openedList = new ListData();

        saveManager.saveData.lists.Add(openedList);

        listSettings.Open(true, -1);
    }

    public void OpenSettings(int index)
    {
        listSettings.Open(false, index);
    }
}
