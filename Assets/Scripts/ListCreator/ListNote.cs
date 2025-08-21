using UnityEngine;
using UnityEngine.EventSystems;

public class ListNote : MonoBehaviour, IPointerClickHandler
{
    public ListData list;

    [SerializeField] private ListPanel listPanel;
    [SerializeField] private Note note;
    [SerializeField] private int noteType;

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
        if (noteType == 0)
            listPanel.OpenList(list);
    }

    private void RMB()
    {
        
    }

    public void Load(ListData givenList, ListPanel givenPanel, int givenType)
    {
        listPanel = givenPanel;
        list = givenList;
        noteType = givenType;

        note.ChangeText(list.name);
    }
}
