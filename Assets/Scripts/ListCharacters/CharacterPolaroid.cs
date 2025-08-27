using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterPolaroid : MonoBehaviour, IPointerClickHandler
{
    public Character character;
    public int index;

    [SerializeField] private Polaroid polaroid;
    [SerializeField] private ListPolaroid listPolaroid;

    private CharactersPanel charactersPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LMB();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (character.winsNeeded <= PlayerPrefs.GetInt("Wins") || charactersPanel.listPanel.devManager.unlockAllCharacters)
                RMB();
        }
    }

    public void Load(Character givenCharacter, CharactersPanel givenPanel, int givenIndex)
    {
        character = givenCharacter;
        charactersPanel = givenPanel;
        index = givenIndex;

        listPolaroid.index = givenIndex;
        listPolaroid.character = givenCharacter;
        listPolaroid.listPanel = givenPanel.listPanel;
        polaroid.Load(character);

        CheckIfCanAdd();
    }

    public void CheckIfCanAdd()
    {
        int isInList = charactersPanel.listPanel.openedList.characters.FindIndex(d => d == character.directory);

        if (isInList != -1 || (character.winsNeeded > PlayerPrefs.GetInt("Wins") && !charactersPanel.listPanel.devManager.unlockAllCharacters))
        {
            polaroid.Disable();
            listPolaroid.characterCanAdd = false;
        }
        else
        {
            polaroid.Enable();
            listPolaroid.characterCanAdd = true;
        }
    }

    private void LMB()
    {

    }

    private void RMB()
    {
        if (!charactersPanel.listPanel.isInfoPanelShown)
        {
            charactersPanel.listPanel.menu = 2;
            charactersPanel.listPanel.infoPanel.polaroidSlot = listPolaroid;
            charactersPanel.listPanel.ShowInfoPanel(character);
        }
    }
}
