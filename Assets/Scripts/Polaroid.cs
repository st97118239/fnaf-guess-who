using System.Collections;
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
    [SerializeField] private bool hasCircle;

    [SerializeField] private float fillTime = 1; 

    private void Awake()
    {
        if (polXImage)
        {
            if (startCrossedOff && polXImage.fillAmount == 0)
            {
                isCrossedOff = false;
                CrossOff();
            }
            else if (!startCrossedOff && polXImage.fillAmount == 1)
            {
                isCrossedOff = true;
                CrossOff();
            }
        }
    }

    private IEnumerator FadeImage(bool fill, string image, bool imageDisabling)
    {
        yield return null;

        polXImage.sprite = Resources.Load<Sprite>("UI/" + image);

        Color startColor = fill ? Color.white : Color.grey;
        Color endColor = fill ? Color.grey : Color.white;

        for (float i = 0; i <= fillTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > fillTime) i = fillTime;

            float fillAmount = i / fillTime;
            polXImage.fillAmount = fill ? fillAmount : 1 - fillAmount;

            if (imageDisabling)
            {
                polImage.color = Color.Lerp(startColor, endColor, fillAmount);
                characterImage.color = Color.Lerp(startColor, endColor, fillAmount);
            }

            yield return null;
        }
    }

    public void Enable()
    {
        isCrossedOff = true;
        CrossOff();
        polButton.interactable = true;
    }

    public void Disable()
    {
        isCrossedOff = false;
        CrossOff();
        polButton.interactable = false;
    }

    public void CrossOff()
    {
        if (hasCircle)
            return;

        if (isCrossedOff)
        {
            StartCoroutine(FadeImage(false, "X", true));
            isCrossedOff = false;
        }
        else
        {
            StartCoroutine(FadeImage(true, "X", true));
            isCrossedOff = true;
        }
    }

    public void Circle()
    {
        if (isCrossedOff)
            return;

        if (hasCircle)
        {
            StartCoroutine(FadeImage(false, "O", false));
            isCrossedOff = false;
        }
        else
        {
            StartCoroutine(FadeImage(true, "O", false));
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

        if (character.polaroidSprite[0])
        {
            characterImage.sprite = character.polaroidSprite[0];
            characterImage.color = Color.white;
        }
        else
            characterImage.color = Color.clear;

        if (character.characterName != string.Empty)
            characterText.text = character.characterName;
        else
            characterText.text = string.Empty;
    }
}
