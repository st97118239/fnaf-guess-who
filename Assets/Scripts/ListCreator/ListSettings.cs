using TMPro;
using UnityEngine;

public class ListSettings : MonoBehaviour
{
    [SerializeField] private ListPanel listPanel;
    [SerializeField] private GameObject backgroundBlocker;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject doubleNameErrorText;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_Text titleText;

    private int index;
    private bool isNewList;

    public void Open(bool newList, int listIdx)
    {
        isNewList = newList;

        if (isNewList)
            titleText.text = "New List";
        else
        {
            titleText.text = "Edit List";
            nameField.text = listPanel.saveManager.saveData.lists[listIdx].name;
            index = listIdx;
        }

        animator.SetTrigger("PaperOpen");
        backgroundBlocker.SetActive(true);
    }

    public void Close()
    {
        int doesListExist = listPanel.saveManager.saveData.lists.FindIndex(l => l.name == nameField.text);

        if (doesListExist != -1)
        {
            doubleNameErrorText.SetActive(true);
            return;
        }

        if (isNewList)
            listPanel.openedList.name = nameField.text;
        else
            listPanel.saveManager.saveData.lists[index].name = nameField.text;

            animator.SetTrigger("PaperClose");
        Invoke(nameof(DisableBackground), 0.6f);

        listPanel.saveManager.Save();

        if (isNewList)
            listPanel.OpenList(listPanel.openedList);
        else
            listPanel.RefreshLists();

        nameField.text = string.Empty;
        doubleNameErrorText.SetActive(false);
    }

    public void Delete()
    {
        listPanel.saveManager.RemoveList(index);

        animator.SetTrigger("PaperClose");
        Invoke(nameof(DisableBackground), 0.6f);

        listPanel.saveManager.Save();

        nameField.text = string.Empty;
        doubleNameErrorText.SetActive(false);
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }
}
