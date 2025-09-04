using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CategoryNote : MonoBehaviour, IPointerClickHandler
{
    public CharacterCategory category;

    [SerializeField] private CharactersPanel charactersPanel;
    [SerializeField] private Note note;

    private bool canLMB = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) 
            return;

        if (note.isCrossedOff || !canLMB)
            return;

        LMB();
    }

    private void LMB()
    {
        canLMB = false;
        charactersPanel.OpenCategoryFade(category);
        charactersPanel.listPanel.mainPanel.audioManager.soundEffects.PlayOneShot(charactersPanel.listPanel.mainPanel.audioManager.noteSFX);
    }

    public void LoadCategory(CharacterCategory givenCategory, CharactersPanel givenPanel)
    {
        charactersPanel = givenPanel;
        category = givenCategory;

        if (category.characters.Count == 0)
            note.Disable();

        int amountLocked = category.characters.Count(t => !t.isUnlocked);

        if (category.characters.Count == amountLocked && !charactersPanel.listPanel.devManager.unlockAllCharacters)
            note.Disable();

        note.ChangeText(category.categoryName);
    }
}
