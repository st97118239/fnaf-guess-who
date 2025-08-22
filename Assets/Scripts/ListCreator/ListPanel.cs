using System.Collections.Generic;
using UnityEngine;

public class ListPanel : MonoBehaviour
{
    public Game game;
    public SaveManager saveManager;
    public ListInfoPanel infoPanel;
    public ListData selectedList;
    public ListData openedList;

    public int menu; // -1 = loading, 0 = listsMenu, 1 = listCharactersMenu
    public bool isInfoPanelShown;
    public bool fromCharacterPanel;

    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private GameObject listGrid;
    [SerializeField] private ListSettings listSettings;
    [SerializeField] private GameObject listNotePrefab;
    [SerializeField] private GameObject emptySlotPrefab;
    [SerializeField] private GameObject polaroidPrefab;

    [SerializeField] private Note backNote;
    [SerializeField] private Note leaveNote;
    [SerializeField] private Note saveNote;
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
        //if (hasLoaded)
        //    EmptySlotsReset();

        if (fromCharacterPanel)
            game.animator.SetTrigger("ListCharClose");
        else
            game.animator.SetTrigger("ListCreatorOpen");

        fromCharacterPanel = false;

        if (!hasLoaded)
            Invoke(nameof(SpawnFirst), 1);
        //else
        //    Invoke(nameof(PlayFadeAnim), 1);
    }

    public void ClosePanel()
    {
        game.animator.SetTrigger("ListCreatorClose");
    }

    private void EmptySlotsReset()
    {
        for (int i = 0; i < emptySlots.Count; i++)
        {
            emptySlots[i].Reset();
        }
    }

    private void PlayFadeAnim()
    {
        for (int i = 0; i < emptySlots.Count; i++)
        {
            emptySlots[i].Play();
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
                listNotes[i].LoadList(saveManager.saveData.lists[i], this, ListNoteType.Lists);
            else if (i == listAmount)
                listNotes[i].NewListButton(this, ListNoteType.AddList);
        }

        backNote.Disable();
        saveNote.Disable();
        selectNote.Disable();
        menu = 0;

        PlayFadeAnim();
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
        if (!openedList.builtIn)
            saveNote.Enable();
        if (!openedList.selected && !mainPanel.isReady)
            selectNote.Enable();
        menu = 1;

        PlayFadeAnim();
    }

    public void ShowInfoPanel(Character character)
    {
        infoPanel.Show(character);
    }

    public void ListCharactersBack()
    {
        for (int i = 0; i < polaroids.Count; i++)
        {
            Destroy(polaroids[i].gameObject);
        }

        polaroids.Clear();

        SpawnListNotes();
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

        for (int i = 0; i < polaroids.Count; i++)
        {
            Destroy(polaroids[i].gameObject);
        }

        OpenListCharactersMenu();
    }

    public void NewList()
    {
        openedList = new ListData();

        saveManager.saveData.lists.Add(openedList);

        listSettings.Open();
    }
}
