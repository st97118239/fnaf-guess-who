using TMPro;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

public class ListSettings : MonoBehaviour
{
    [SerializeField] private ListPanel listPanel;
    [SerializeField] private GameObject backgroundBlocker;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private Note scriptableObjectNote;
    [SerializeField] private Note deleteNote;
    [SerializeField] private Note duplicateNote;

    [SerializeField] private GameObject doubleNameErrorText;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_Text titleText;

    private string characterListPath;

    private CharacterList characterList;
    private ListData listToSave;

    private int index;
    private bool isNewList;
    private bool isShown;
    private bool cancel;

    private static readonly int PaperOpen = Animator.StringToHash("PaperOpen");
    private static readonly int PaperClose = Animator.StringToHash("PaperClose");

    private void Start()
    {
        characterListPath = "Assets/ScriptableObjects/CharacterLists/";
    }

    private void Update()
    {
        if (!isShown || !Input.GetKeyDown(KeyCode.Escape)) 
            return;

        cancel = true;
        Close();
    }

    public void Open(bool newList, int listIdx)
    {
        cancel = false;
        isShown = true;

        isNewList = newList;
        index = listIdx;

        if (isNewList)
        {
            titleText.text = "New List";
            deleteNote.ChangeText("Cancel");
            duplicateNote.Disable();

            if (listPanel.devManager.isUnlocked)
            {
                scriptableObjectNote.gameObject.SetActive(true);
                scriptableObjectNote.Disable();
            }
        }
        else
        {
            titleText.text = "Edit List";
            nameField.text = listPanel.saveManager.saveData.lists[listIdx].name;
            deleteNote.ChangeText("Delete");
            duplicateNote.Enable();

            if (listPanel.devManager.isUnlocked)
            {
                scriptableObjectNote.gameObject.SetActive(true);
                scriptableObjectNote.Enable();
            }
        }

        animator.SetTrigger(PaperOpen);
        backgroundBlocker.SetActive(true);
        audioManager.soundEffects.PlayOneShot(audioManager.clipboardSFX);
    }

    public void Close()
    {
        isShown = false;

        if (cancel)
        {
            animator.SetTrigger(PaperClose);
            audioManager.soundEffects.PlayOneShot(audioManager.clipboardSFX);
            Invoke(nameof(DisableBackground), 0.6f);
            cancel = false;
            nameField.text = string.Empty;
            doubleNameErrorText.SetActive(false);

            if (!isNewList) 
                return;

            listPanel.openedList = null;
            listPanel.saveManager.saveData.lists.RemoveAt(index);

            return;
        }

        int doesListExist = listPanel.saveManager.saveData.lists.FindIndex(l => l.name == nameField.text);

        if (doesListExist != -1 && doesListExist != index)
        {
            doubleNameErrorText.SetActive(true);
            return;
        }

        if (isNewList)
        {
            listPanel.openedList.name = nameField.text;
            listPanel.openedList.version = PlayerPrefs.GetInt("Version");
        }
        else
            listPanel.saveManager.saveData.lists[index].name = nameField.text;

        animator.SetTrigger(PaperClose);
        Invoke(nameof(DisableBackground), 0.6f);

        listPanel.saveManager.Save();

        if (isNewList)
            listPanel.OpenList(listPanel.openedList);
        else
            listPanel.RefreshLists();

        nameField.text = string.Empty;
        doubleNameErrorText.SetActive(false);
    }

    public void DuplicateNote()
    {
        cancel = true;
        Close();

        Invoke(nameof(DuplicateList), 0.9f);
    }

    private void DuplicateList()
    {
        listPanel.saveManager.DuplicateList(index);
    }

    public void Delete()
    {
        animator.SetTrigger(PaperClose);
        Invoke(nameof(DisableBackground), 0.6f);

        if (isNewList)
        {
            listPanel.openedList = null;
            listPanel.saveManager.saveData.lists.RemoveAt(index);
        }
        else
        {
            listPanel.saveManager.RemoveList(index);
            listPanel.saveManager.Save();
        }

        index = 0;
        nameField.text = string.Empty;
        doubleNameErrorText.SetActive(false);
    }

    public void SaveToSO()
    {
#if UNITY_EDITOR
        listToSave = listPanel.saveManager.saveData.lists[index];

        characterList = ScriptableObject.CreateInstance<CharacterList>();

        characterList.characters = new(listToSave.characters.Count);

        foreach (string t in listToSave.characters)
        {
            characterList.characters.Add(Resources.Load<Character>(t));
        }

        characterList.listName = listToSave.name;
        characterList.version = PlayerPrefs.GetInt("Version");

        AssetDatabase.CreateAsset(characterList, characterListPath + characterList.listName + ".asset");
#endif

        characterList = null;

        cancel = true;
        Close();
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }
}
