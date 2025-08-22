using UnityEngine;
using UnityEngine.EventSystems;

public class CategoryNote : MonoBehaviour, IPointerClickHandler
{
    public CharacterCategory category;

    [SerializeField] private CharactersPanel charactersPanel;
    [SerializeField] private Note note;

    private bool canLMB = true;
    private bool canRMB = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (note.isCrossedOff || !canLMB)
                return;

            LMB();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (note.isCrossedOff || !canRMB)
                return;

            RMB();
        }
    }

    private void LMB()
    {
        canLMB = false;
       charactersPanel.OpenCategoryFade(category);
    }

    private void RMB()
    {
        
    }

    public void LoadCategory(CharacterCategory givenCategory, CharactersPanel givenPanel)
    {
        charactersPanel = givenPanel;
        category = givenCategory;

        if (category.characters.Count == 0)
            note.Disable();

        note.ChangeText(category.categoryName);
    }
}
