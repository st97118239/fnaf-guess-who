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
    [SerializeField] private AudioClip markerSFX;

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

    public void Load(Character givenCharacter, Game givenGameScript, bool givenCanLMB)
    {
        character = givenCharacter;
        gameScript = givenGameScript;

        polaroid.Load(givenCharacter);

        canLMB = givenCanLMB;
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
            gameScript.possibleSlots.Add(this);
            gameScript.crossedOff.Remove(this);
            gameScript.UpdateSidebar();
        }
        else
        {
            isCrossedOff = true;
            polaroid.CrossOff();
            gameScript.possibleSlots.Remove(this);
            gameScript.crossedOff.Add(this);
            gameScript.UpdateSidebar();
            gameScript.audioManager.soundEffects.PlayOneShot(markerSFX);
        }
    }

    public void Accuse()
    {
        if (isCrossedOff)
            Toggle();

        polaroid.Circle();
        isAccused = true;
        gameScript.HasAccused(character);
    }

    public void CanLMB(bool can)
    {
        if (can)
            canLMB = true;
        else if (!can)
            canLMB = false;
    }
}
