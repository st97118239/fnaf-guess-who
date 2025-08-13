using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour
{
    public Image characterImage;
    public Image slotImage;
    public Character character;
    public bool isChosen;
    public bool isHidden;

    private Game gameScript;
    private Button button;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        characterImage = button.GetComponent<Image>();
        slotImage = GetComponent<Image>();
    }

    public void Load(Character givenCharacter, Game givenGameScript)
    {
        character = givenCharacter;
        gameScript = givenGameScript;

        characterImage.sprite = character.sprite;
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
        slotImage.sprite = Resources.Load<Sprite>("UI/Selected");
    }

    private void Toggle()
    {
        if (isHidden)
        {
            isHidden = false;
            characterImage.sprite = character.sprite;
        }
        else
        {
            isHidden = true;
            characterImage.sprite = Resources.Load<Sprite>("UI/Hidden");
        }
    }
}
