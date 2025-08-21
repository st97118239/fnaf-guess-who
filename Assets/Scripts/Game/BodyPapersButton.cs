using UnityEngine;
using UnityEngine.EventSystems;

public class BodyPapersButton : MonoBehaviour, IPointerClickHandler
{
    public InfoPanel infoPanel;

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
