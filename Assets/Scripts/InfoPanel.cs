using System;
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
    public List<string> variables;
    public List<RectTransform> lines;
    public List<TMP_Text> texts;

    public void Show(Character givenCharacter)
    {
        character = givenCharacter;

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].gameObject.SetActive(true);
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

        for (int i = 0; i < variables.Count; i++)
        {
            var fi = typeof(Character).GetField(variables[i]);
            texts[i].text = fi?.GetValue(character)?.ToString();
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
            if (!string.IsNullOrEmpty(texts[i].text))
                lines[i].gameObject.SetActive(true);
            else
                lines[i].gameObject.SetActive(false);
        }

        lines[0].gameObject.SetActive(false);

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
