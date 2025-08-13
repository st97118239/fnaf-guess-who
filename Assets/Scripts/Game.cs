using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public List<Character> characters;
    public GameObject gamePanel;
    public GameObject charSlotPrefab;
    public List<CharSlot> charSlots;
    public Character chosenCharacter;
    public bool hasChosen;
    public Color hiddenColor;
    public Color defaultColor;
    
    private int slotAmount;

    private void Start()
    {
        slotAmount = characters.Count;

        charSlots = new List<CharSlot>(slotAmount);

        for (int i = 0; i < slotAmount; i++)
        {
            GameObject slotObj = Instantiate(charSlotPrefab, gamePanel.transform);
            charSlots.Add(slotObj.GetComponent<CharSlot>());
            charSlots[i].Load(characters[i], this);
        }
    }

    public void ChooseCharacter(Character givenChar)
    {
        chosenCharacter = givenChar;
        hasChosen = true;
    }
}
