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
    [SerializeField] private Polaroid polaroid;

    private Game gameScript;
    public bool isCrossedOff;
    private bool isAccused;

    private void Start()
    {
        if (!polaroid)
            polaroid = GetComponent<Polaroid>();
    }

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
            polaroid.CrossOff();
            gameScript.crossedOff.Remove(this);
            gameScript.UpdateSidebar();
        }
        else
        {
            isCrossedOff = true;
            polaroid.CrossOff();
            gameScript.crossedOff.Add(this);
            gameScript.UpdateSidebar();
        }
    }

    public void Accuse()
    {
        if (isCrossedOff)
            Toggle();

        polaroid.Circle();
        isAccused = true;
    }
}
