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

    private Game gameScript;

    private void Start()
    {
        character.isChosen = false;
        character.isCrossedOff = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Press();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!gameScript.isInfoPanelShown)
                gameScript.ShowInfoPanel(character);
        }
    }

    public void Load(Character givenCharacter, Game givenGameScript)
    {
        character = givenCharacter;
        gameScript = givenGameScript;

        if (character.polaroidSprite.Count > 0 && character.polaroidSprite[0])
            characterImage.sprite = character.polaroidSprite[0];
        else
            characterImage.gameObject.SetActive(false);

            charName.text = character.characterName;
    }

    public void Press()
    {
        if (gameScript.chosenCharacter)
            Toggle();
        else
            Choose();
    }

    private void Choose()
    {
        gameScript.ChooseCharacter(character);
        character.isChosen = true;
    }

    private void Toggle()
    {
        if (character.isCrossedOff)
        {
            character.isCrossedOff = false;
            xImage.color = new Color(255, 255, 255, 0);
            gameScript.crossedOff.Remove(this);
            gameScript.UpdateSidebar();
        }
        else
        {
            character.isCrossedOff = true;
            xImage.color = new Color(255, 255, 255, 255);
            gameScript.crossedOff.Add(this);
            gameScript.UpdateSidebar();
        }
    }
}
