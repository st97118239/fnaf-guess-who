using UnityEngine;
using UnityEngine.EventSystems;

public class BodyPapersButton : MonoBehaviour, IPointerClickHandler
{
    public InfoPanel infoPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("a");
            infoPanel.BodyPapersNext();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("b");
            infoPanel.BodyPapersBack();
        }
    }
}
