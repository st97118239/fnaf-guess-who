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
    public bool isHidden;

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
        if (isHidden)
        {
            isHidden = false;
            xImage.color = new Color(255, 255, 255, 0);
        }
        else
        {
            isHidden = true;
            xImage.color = new Color(255, 255, 255, 255);
        }
    }
}
