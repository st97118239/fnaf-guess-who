using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour
{
    public Image characterImage;
    public Image slotImage;
    public Image xImage;
    public TMP_Text charName;
    public Character character;
    public bool isChosen;
    public bool isCrossedOff;

    private Game gameScript;

    public void Load(Character givenCharacter, Game givenGameScript)
    {
        character = givenCharacter;
        gameScript = givenGameScript;

        characterImage.sprite = character.sprite;
        charName.text = character.name;
    }

    public void Press()
    {
        if (gameScript.hasChosen)
            Toggle();
        else
            Choose();
    }

    private void Choose()
    {
        gameScript.ChooseCharacter(character);
        isChosen = true;
    }

    private void Toggle()
    {
        if (isCrossedOff)
        {
            isCrossedOff = false;
            xImage.color = new Color(255, 255, 255, 0);
            gameScript.crossedOff.Remove(this);
            gameScript.UpdateSidebar();
        }
        else
        {
            isCrossedOff = true;
            xImage.color = new Color(255, 255, 255, 255);
            gameScript.crossedOff.Add(this);
            gameScript.UpdateSidebar();
        }
    }
}
