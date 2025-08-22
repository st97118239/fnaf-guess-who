using UnityEngine;
using UnityEngine.EventSystems;

public class ListNote : MonoBehaviour, IPointerClickHandler
{
    public ListData list;

    [SerializeField] private ListPanel listPanel;
    [SerializeField] private Note note;
    [SerializeField] private ListNoteType listNoteType;

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
        if (listNoteType == ListNoteType.Lists)
            listPanel.OpenList(list);
        else if (listNoteType == ListNoteType.AddList)
            listPanel.NewList();
    }

    private void RMB()
    {
        
    }

    public void LoadList(ListData givenList, ListPanel givenPanel, ListNoteType givenType)
    {
        listPanel = givenPanel;
        list = givenList;
        listNoteType = givenType;

        note.ChangeText(list.name);
    }

    public void NewListButton(ListPanel givenPanel, ListNoteType givenType)
    {
        listPanel = givenPanel;
        listNoteType = givenType;

        note.ChangeText("+");
    }
}
