using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    public Button polButton;
    public Image polImage;
    public Image characterImage;
    public TMP_Text characterText;
    public Image polXImage;
    public Character character;

    [SerializeField] private bool startCrossedOff;
    [SerializeField] private bool isCrossedOff;

    private Color transparent = new(255, 255, 255, 0);
    private Color opaque = new(255, 255, 255, 255);
    private Color disabled = new(0.549f, 0.549f, 0.549f, 255);

    private void Awake()
    {
        if (polXImage)
        {
            if (startCrossedOff)
            {
                isCrossedOff = false;
                CrossOff();
            }
            else if (!startCrossedOff)
            {
                isCrossedOff = true;
                CrossOff();
            }
        }
    }

    public void Enable()
    {
        polXImage.color = transparent;
        polButton.interactable = true;
    }

    public void Disable()
    {
        polXImage.color = opaque;
        polButton.interactable = false;
    }

    public void CrossOff()
    {
        if (isCrossedOff)
        {
            polXImage.color = transparent;
            polImage.color = opaque;
            characterImage.color = opaque;
            isCrossedOff = false;
        }
        else
        {
            polXImage.color = opaque;
            polImage.color = disabled;
            characterImage.color = disabled;
            isCrossedOff = true;
        }
    }

    public void ChangeText(string txt)
    {
        characterText.text = txt;
    }

    public void Load(Character givenCharacter)
    {
        character = givenCharacter;

        characterImage.sprite = character.polaroidSprite[0];
        characterText.text = character.characterName;
    }
}
