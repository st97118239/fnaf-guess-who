using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public CharacterList characterList;
    public GameObject gamePanel;
    public CharacterSidebar characterSidebar;
    public InfoPanel infoPanel;
    public GameObject charSlotPrefab;
    public List<CharSlot> charSlots;
    public List<CharSlot> crossedOff;
    public Character chosenCharacter;
    public bool hasChosen;

    private int slotAmount;

    private void Start()
    {
        slotAmount = characterList.characters.Count;

        charSlots = new List<CharSlot>(slotAmount);

        for (int i = 0; i < slotAmount; i++)
        {
            GameObject slotObj = Instantiate(charSlotPrefab, gamePanel.transform);
            charSlots.Add(slotObj.GetComponent<CharSlot>());
            charSlots[i].Load(characterList.characters[i], this);
        }

        UpdateSidebar();
    }

    public void ChooseCharacter(Character givenChar)
    {
        chosenCharacter = givenChar;
        hasChosen = true;
        characterSidebar.SetCharacter(chosenCharacter);
    }

    public void UpdateSidebar()
    {
        characterSidebar.ReloadSidebarStats(crossedOff.Count, charSlots.Count);
    }

    public void ShowInfoPanel(Character character)
    {
        infoPanel.Show(character);
    }
}
