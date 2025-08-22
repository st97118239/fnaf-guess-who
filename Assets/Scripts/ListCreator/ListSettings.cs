using TMPro;
using UnityEditor.Overlays;
using UnityEngine;

public class ListSettings : MonoBehaviour
{
    [SerializeField] private ListPanel listPanel;
    [SerializeField] private GameObject backgroundBlocker;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject doubleNameErrorText;

    [SerializeField] private TMP_InputField nameField;

    public void Open()
    {
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

        listPanel.openedList.name = nameField.text;

        animator.SetTrigger("PaperClose");
        Invoke(nameof(DisableBackground), 0.6f);

        listPanel.saveManager.Save();

        listPanel.OpenList(listPanel.openedList);

        nameField.text = string.Empty;
        doubleNameErrorText.SetActive(false);
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }
}
