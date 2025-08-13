using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Character character;
    public GameObject polaroid;
    public Image slotImage;
    public Image fullBodyImage;
    public List<RectTransform> lines;
    public List<TMP_Text> texts;
    public RectTransform nameLine;
    public TMP_Text nameText;
    public RectTransform yearLine;
    public TMP_Text yearText;
    public RectTransform affiliationLine;
    public TMP_Text affiliationText;
    public RectTransform firstAppearanceLine;
    public TMP_Text firstAppearanceText;
    public RectTransform classLine;
    public TMP_Text classText;
    public RectTransform modelLine;
    public TMP_Text modelText;
    public RectTransform colorLine;
    public TMP_Text colorText;
    public RectTransform eyeColorLine;
    public TMP_Text eyeColorText;
    public RectTransform pronounsLine;
    public TMP_Text pronounsText;
    public RectTransform aliasesLine;
    public TMP_Text aliasesText;

    public void Show(Character givenCharacter)
    {
        character = givenCharacter;

        for (int i = 0; i < lines.Count; i++)
        {
            if (texts[i].text != null)
            {
                lines[i].gameObject.SetActive(true);
            }
        }

        if (character.sprite)
        {
            slotImage.sprite = character.sprite;
            polaroid.SetActive(true);
        }
        else
            polaroid.SetActive(false);

        if (character.fullBodySprite)
        {
            fullBodyImage.sprite = character.fullBodySprite;
            fullBodyImage.gameObject.SetActive(true);
        }
        else
            fullBodyImage.gameObject.SetActive(false);

        if (character.characterName != string.Empty)
        {
            nameText.text = character.characterName;
        }
        else
        {
            nameText.text = null;
        }

        if (character.year != string.Empty)
        {
            yearText.text = character.year;
        }
        else
        {
            yearText.text = null;
        }

        if (character.affiliation != string.Empty)
        {
            affiliationText.text = character.affiliation;
        }
        else
        {
            affiliationText.text = null;
        }

        if (character.firstAppearance != string.Empty)
        {
            firstAppearanceText.text = character.firstAppearance;
        }
        else
        {
            firstAppearanceText.text = null;
        }

        if (character.type != string.Empty)
        {
            classText.text = character.type;
        }
        else
        {
            classText.text = null;
        }

        if (character.model != string.Empty)
        {
            modelText.text = character.model;
        }
        else
        {
            modelText.text = null;
        }

        if (character.color != string.Empty)
        {
            colorText.text = character.color;
        }
        else
        {
            colorText.text = null;
        }

        if (character.eyeColor != string.Empty)
        {
            eyeColorText.text = character.eyeColor;
        }
        else
        {
            eyeColorText.text = null;
        }

        if (character.pronouns != string.Empty)
        {
            pronounsText.text = character.pronouns;
        }
        else
        {
            pronounsText.text = null;
        }

        if (character.aliases != string.Empty)
        {
            aliasesText.text = character.aliases;
        }
        else
        {
            aliasesText.text = null;
        }

        Invoke("RedrawText", 0.1f);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        character = null;
    }

    private void RedrawText()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (texts[i].text != null && i != 0)
            {
                lines[i].gameObject.SetActive(true);
            }
            else
            {
                lines[i].gameObject.SetActive(false);
            }

            nameLine.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);

        StartCoroutine(EnableFolder());
    }

    private IEnumerator EnableFolder()
    {
        yield return 0;

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].sizeDelta = new Vector2(lines[i].sizeDelta.x, texts[i].preferredHeight);
        }

        lines[0].gameObject.SetActive(true);
    }
}
