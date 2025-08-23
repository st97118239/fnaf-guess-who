using UnityEngine;
using UnityEngine.EventSystems;

public class CategoryButton : MonoBehaviour, IPointerClickHandler
{
    public CharactersPanel charactersPanel;

    [SerializeField] private Note note;

    private bool canClick = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (canClick)
                LMB();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (canClick)
                RMB();
        }
    }

    private void LMB()
    {
        charactersPanel.CategoryButton(1);
        note.Disable();
        Invoke(nameof(Enable), charactersPanel.fadeAnimDelay);
        canClick = false;
    }

    private void RMB()
    {
        charactersPanel.CategoryButton(-1);
        note.Disable();
        Invoke(nameof(Enable), charactersPanel.fadeAnimDelay);
        canClick = false;
    }

    private void Enable()
    {
        note.Enable();
        canClick = true;
    }
}
