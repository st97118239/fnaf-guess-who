using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour, IPointerClickHandler
{
    public Image characterImage;
    public Image slotImage;
    public Image xImage;
    public TMP_Text charName;
    public Character character;
    public bool isChosen;
    public bool isCrossedOff;

    private Game gameScript;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Press();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            gameScript.ShowInfoPanel(character);
        }
    }

    public void Load(Character givenCharacter, Game givenGameScript)
    {
        character = givenCharacter;
        gameScript = givenGameScript;

        if (character.polaroidSprite[0])
            characterImage.sprite = character.polaroidSprite[0];
        else
            characterImage.gameObject.SetActive(false);

            charName.text = character.characterName;
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
