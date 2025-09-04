using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListPanel : MonoBehaviour
{
    public MainPanel mainPanel;
    public Game game;
    public SaveManager saveManager;
    public ListSettings listSettings;
    public ListInfoPanel infoPanel;
    public ListData selectedList;
    public ListData openedList;
    public CharactersPanel charactersPanel;
    public CharacterClipboard characterClipboard;
    public DevManager devManager;

    public int menu; // -1 = loading, 0 = listsMenu, 1 = listCharactersMenu, 2 = charactersPanel
    public bool isInfoPanelShown;
    public bool fromCharacterPanel;
    public bool hasListOpen;
    public int maxCharacters = 96;

    [SerializeField] private GameObject listGrid;
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

    private static readonly int ListCreatorClose = Animator.StringToHash("ListCreatorClose");
    private static readonly int ListCreatorOpen = Animator.StringToHash("ListCreatorOpen");
    private static readonly int ListCharClose = Animator.StringToHash("ListCharClose");

    private void Awake()
    {
        if (emptySlotAmount != 0) 
            return;

        emptySlotAmount = 96; // Set emptySlotAmount to max otherwise we get an error
        Debug.LogWarning("emptySlotAmount is set to 0, please set an amount.");
    }

    private void Update()
    {
        if (mainPanel.currentPanel != Panels.ListPanel)
            return;

        if (menu == 1 && !isInfoPanelShown && Input.GetKeyDown(KeyCode.Escape))
            BackNote();
    }

    public void LoadPanel()
    {
        mainPanel.currentPanel = Panels.ListPanel;

        game.animator.SetTrigger(fromCharacterPanel ? ListCharClose : ListCreatorOpen);

        fromCharacterPanel = false;

        if (!hasLoaded)
            Invoke(nameof(SpawnFirst), 1);
    }

    public void ClosePanel()
    {
        mainPanel.currentPanel = Panels.MainPanel;
        game.animator.SetTrigger(ListCreatorClose);
    }

    private void PlayFadeAnim(bool shouldReverse, bool shouldDestroyChildren, bool shouldDestroySelf)
    {
        foreach (EmptySlot t in emptySlots)
        {
            t.Play(shouldReverse, shouldDestroyChildren, shouldDestroySelf);
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
        if (listNotes != null)
        {
            foreach (ListNote note in listNotes.Where(note => note))
            {
                Destroy(note.gameObject);
            }

            listNotes.Clear();
        }

        if (polaroids != null)
        {
            foreach (ListPolaroid pol in polaroids.Where(pol => pol))
            {
                Destroy(pol.gameObject);
            }

            polaroids.Clear();
        }

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

        if (listNotes != null)
        {
            foreach (ListNote note in listNotes.Where(note => note))
            {
                Destroy(note.gameObject);
            }

            listNotes.Clear();
        }

        if (polaroids != null)
        {
            foreach (ListPolaroid pol in polaroids.Where(pol => pol))
            {
                Destroy(pol.gameObject);
            }

            polaroids.Clear();
        }

        foreach (EmptySlot t in emptySlots)
        {
            t.Reset();
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
        if (openedList == null)
        {
            Debug.Log("No opened list found.");
            BackNote();
            return;
        }

        foreach (ListPolaroid t in polaroids.Where(t => t))
        {
            Destroy(t.gameObject);
        }

        polaroids.Clear();

        foreach (ListNote t in listNotes.Where(t => t))
        {
            Destroy(t.gameObject);
        }

        listNotes.Clear();

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
        foreach (ListPolaroid t in polaroids.Where(t => t))
        {
            Destroy(t.gameObject);
        }

        polaroids.Clear();

        foreach (ListNote t in listNotes.Where(t => t))
        {
            Destroy(t.gameObject);
        }

        listNotes.Clear();

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

        Invoke(nameof(SpawnListNotes), 0.7f);
    }

    public void BackNote()
    {
        if (menu == 1) 
            ListCharactersBack();
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

        listSettings.Open(true, saveManager.saveData.lists.IndexOf(openedList));
    }

    public void OpenSettings(int index)
    {
        listSettings.Open(false, index);
    }
}
