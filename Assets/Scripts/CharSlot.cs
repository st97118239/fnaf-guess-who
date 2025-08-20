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
    private bool isCrossedOff;

    private Color transparent = new(255, 255, 255, 0);
    private Color opaque = new(255, 255, 255, 255);

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

        if (character.characterName != string.Empty)
            charName.text = character.characterName;
    }

    public void Press()
    {
        Toggle();
    }

    private void Toggle()
    {
        if (isCrossedOff)
        {
            isCrossedOff = false;
            xImage.color = transparent;
            gameScript.crossedOff.Remove(this);
            gameScript.UpdateSidebar();
        }
        else
        {
            isCrossedOff = true;
            xImage.color = opaque;
            gameScript.crossedOff.Add(this);
            gameScript.UpdateSidebar();
        }
    }
}
