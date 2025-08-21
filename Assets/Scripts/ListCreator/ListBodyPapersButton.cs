using UnityEngine;
using UnityEngine.EventSystems;

public class ListBodyPapersButton : MonoBehaviour, IPointerClickHandler
{
    public ListInfoPanel infoPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            infoPanel.BodyPapersNext(true);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            infoPanel.BodyPapersBack(true);
        }
    }
}
