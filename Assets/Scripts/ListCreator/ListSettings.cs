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
    private bool isShown;
    private bool cancel;

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
        {
            cancel = true;
            Close();
        }
    }

    public void Open(bool newList, int listIdx)
    {
        cancel = false;
        isShown = true;

        isNewList = newList;
        index = listIdx;

        if (isNewList)
            titleText.text = "New List";
        else
        {
            titleText.text = "Edit List";
            nameField.text = listPanel.saveManager.saveData.lists[listIdx].name;
        }

        animator.SetTrigger("PaperOpen");
        backgroundBlocker.SetActive(true);
    }

    public void Close()
    {
        isShown = false;

        if (cancel)
        {
            animator.SetTrigger("PaperClose");
            Invoke(nameof(DisableBackground), 0.6f);
            cancel = false;
            nameField.text = string.Empty;
            doubleNameErrorText.SetActive(false);

            if (isNewList)
            {
                listPanel.openedList = null;
                listPanel.saveManager.saveData.lists.RemoveAt(index);
            }
            index = 0;

            return;
        }

        int doesListExist = listPanel.saveManager.saveData.lists.FindIndex(l => l.name == nameField.text);

        if (doesListExist != -1 && doesListExist != index)
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

        index = 0;
        nameField.text = string.Empty;
        doubleNameErrorText.SetActive(false);
    }

    public void Delete()
    {
        animator.SetTrigger("PaperClose");
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

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }
}
