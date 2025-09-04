using UnityEngine;
using UnityEngine.EventSystems;

public class ListNote : MonoBehaviour, IPointerClickHandler
{
    public ListData list;

    [SerializeField] private ListPanel listPanel;
    [SerializeField] private Note note;
    [SerializeField] private ListNoteType listNoteType;
    [SerializeField] private int index;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LMB();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RMB();
        }
    }

    private void LMB()
    {
        switch (listNoteType)
        {
            case ListNoteType.Lists:
                listPanel.OpenList(list);
                break;
            case ListNoteType.AddList:
                listPanel.NewList();
                break;
        }

        listPanel.mainPanel.audioManager.soundEffects.PlayOneShot(listPanel.mainPanel.audioManager.noteSFX);
    }

    private void RMB()
    {
        if (!list.builtIn || listPanel.devManager.isUnlocked)
            listPanel.OpenSettings(index);
    }

    public void LoadList(ListData givenList, ListPanel givenPanel, ListNoteType givenType, int givenIndex)
    {
        listPanel = givenPanel;
        list = givenList;
        listNoteType = givenType;
        index = givenIndex;

        note.ChangeText(list.name);
    }

    public void NewListButton(ListPanel givenPanel, ListNoteType givenType)
    {
        listPanel = givenPanel;
        listNoteType = givenType;

        note.ChangeText("+");
    }
}
