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

    [SerializeField] private bool canLMB = true;

    private Game gameScript;
    private bool isCrossedOff;
    private bool isAccused;

    private Color transparent = new(255, 255, 255, 0);
    private Color opaque = new(255, 255, 255, 255);
    private Color disabled = new(0.549f, 0.549f, 0.549f, 255);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (canLMB)
                Press();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!gameScript.isInfoPanelShown)
            {
                gameScript.infoPanel.charSlot = this;
                gameScript.ShowInfoPanel(character);
            }
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
        if (isAccused)
            return;

        if (isCrossedOff)
        {
            isCrossedOff = false;
            xImage.color = transparent;
            slotImage.color = opaque;
            characterImage.color = opaque;
            gameScript.crossedOff.Remove(this);
            gameScript.UpdateSidebar();
        }
        else
        {
            isCrossedOff = true;
            xImage.color = opaque;
            slotImage.color = disabled;
            characterImage.color = disabled;
            gameScript.crossedOff.Add(this);
            gameScript.UpdateSidebar();
        }
    }

    public void Accuse()
    {
        if (isCrossedOff)
            Toggle();

        xImage.sprite = Resources.Load<Sprite>("UI/O");
        xImage.color = opaque;
        isAccused = true;
    }
}
