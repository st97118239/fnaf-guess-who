using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterPolaroid : MonoBehaviour, IPointerClickHandler
{
    public Character character;
    public int index;

    [SerializeField] private Polaroid polaroid;
    [SerializeField] private ListPolaroid listPolaroid;
    [SerializeField] private bool isInList;

    private CharactersPanel charactersPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
            {
                if (character.isUnlocked || charactersPanel.listPanel.devManager.unlockAllCharacters)
                    LMB();
                break;
            }
            case PointerEventData.InputButton.Right:
            {
                if (character.isUnlocked || charactersPanel.listPanel.devManager.unlockAllCharacters)
                    RMB();
                break;
            }
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
        int intIsInList = charactersPanel.listPanel.openedList.characters.FindIndex(d => d == character.directory);

        if (intIsInList != -1)
        {
            polaroid.Disable();
            listPolaroid.characterCanAdd = false;
            isInList = true;
        }
        else if (!character.isUnlocked && !charactersPanel.listPanel.devManager.unlockAllCharacters)
        {
            polaroid.TearOff();
            listPolaroid.characterCanAdd = false;
            isInList = false;
        }
        else
        {
            polaroid.Enable();
            listPolaroid.characterCanAdd = true;
            isInList = false;
        }
    }

    private void LMB()
    {
        if ((charactersPanel.listPanel.openedList.builtIn && !charactersPanel.listPanel.devManager.isUnlocked) || !charactersPanel.listPanel.hasListOpen || (!listPolaroid.characterCanAdd && !isInList) || charactersPanel.listPanel.openedList.characters.Count >= charactersPanel.listPanel.maxCharacters)
            return;

        if (!isInList)
            charactersPanel.listPanel.AddCharacterToList(character);
        else
            charactersPanel.listPanel.RemoveCharacterFromList(character, listPolaroid.index);
    }

    private void RMB()
    {
        if (charactersPanel.listPanel.isInfoPanelShown) 
            return;

        charactersPanel.listPanel.menu = 2;
        charactersPanel.listPanel.infoPanel.polaroidSlot = listPolaroid;
        charactersPanel.listPanel.ShowInfoPanel(character);
    }
}
