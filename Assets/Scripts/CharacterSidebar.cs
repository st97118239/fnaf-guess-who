using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSidebar : MonoBehaviour
{
    public Image slotImage;
    public Image characterImage;
    public TMP_Text characterNameText;

    private Character character;

    public void SetCharacter(Character givenCharacter)
    {
        character = givenCharacter;

        slotImage.color = new Color(255, 255, 255, 255);
        characterImage.sprite = character.sprite;
        characterImage.color = new Color(255, 255, 255, 255);
        characterNameText.text = character.name;
    }
}
