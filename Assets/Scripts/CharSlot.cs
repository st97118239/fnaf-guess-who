using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour
{
    public Image slotImage;
    public Character character;
    public bool isChosen;
    public bool isHidden;

    public void Load(Character givenCharacter)
    {
        character = givenCharacter;

        slotImage.sprite = character.sprite;
    }
}
