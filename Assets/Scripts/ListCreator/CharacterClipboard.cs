using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClipboard : MonoBehaviour
{
    [SerializeField] private ListPanel listPanel;
    [SerializeField] private GameObject characterClipboard;
    [SerializeField] private Animator clipboardAnimator;
    [SerializeField] private GameObject backgroundBlocker;

    [SerializeField] private TMP_InputField indexField;
    [SerializeField] private GameObject errorText;

    private int originalIndex;
    private int indexToMoveTo;
    private string charDir;
    private bool isShown;

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
            CloseClipboard();
    }

    public void OpenClipboard(string givenCharDir, int givenIndex)
    {
        isShown = true;

        errorText.SetActive(false);
        originalIndex = givenIndex;
        charDir = givenCharDir;
        Load();
        clipboardAnimator.SetTrigger("PaperOpen");
        backgroundBlocker.SetActive(true);
    }

    public void CloseClipboard()
    {
        isShown = false;

        int index = int.Parse(indexField.text) - 1;

        if (index < 0 || index > listPanel.openedList.characters.Count - 1)
        {
            errorText.SetActive(true);
            return;
        }

        clipboardAnimator.SetTrigger("PaperClose");
        Invoke(nameof(DisableBackground), 0.6f);
        Save();
        listPanel.RefreshCharactersMenu();
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }

    public void Save()
    {
        indexToMoveTo = int.Parse(indexField.text) - 1;
        string characterToMove = listPanel.openedList.characters[originalIndex];
        listPanel.openedList.characters.RemoveAt(originalIndex);
        listPanel.openedList.characters.Insert(indexToMoveTo, characterToMove);

        Debug.Log("Moved " + characterToMove + " from " + originalIndex +  " to " + indexToMoveTo);

        listPanel.saveManager.Save();
    }

    private void Load()
    {
        indexField.text = (originalIndex + 1).ToString();
    }
}
