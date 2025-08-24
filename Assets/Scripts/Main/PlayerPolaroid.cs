using System.Collections;
using UnityEngine;

public class PlayerPolaroid : MonoBehaviour
{
    public bool isLoaded;

    [SerializeField] private Polaroid pol;
    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField] private MainPanel mainPanel;

    private string username;
    private Character character;

    private bool fadeImage;
    private bool fadeText;

    private IEnumerator FadeImage(bool fadeIn)
    {
        yield return null;

        for (float i = 0; i <= fadeTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;
            
            if (fadeIn)
            {
                if (fadeImage)
                    pol.characterImage.color = Color.Lerp(Color.clear, Color.white, fillAmount);
                if (fadeText)
                    pol.characterText.color = Color.Lerp(Color.clear, Color.black, fillAmount);
            }
            else if(!fadeIn)
            {
                if (fadeImage)
                    pol.characterImage.color = Color.Lerp(Color.white, Color.clear, fillAmount);
                if (fadeText)
                    pol.characterText.color = Color.Lerp(Color.black, Color.clear, fillAmount);
            }

            yield return null;
        }
    }

    public void Load(string givenName, string givenCharDir, bool shouldFadeImage, bool shouldFadeText)
    {
        if (shouldFadeImage)
            pol.characterImage.color = Color.clear;
        if (shouldFadeText)
            pol.characterText.color = Color.clear;

        username = givenName;
        character = Resources.Load<Character>(givenCharDir);

        if (!character)
        {
            mainPanel.character = "Characters/FNAF1/Freddy";
            character = Resources.Load<Character>(mainPanel.character);
        }

        if (character.polaroidSprite[0])
            pol.characterImage.sprite = character.polaroidSprite[0];

        pol.characterText.text = username;

        fadeImage = shouldFadeImage;
        fadeText = shouldFadeText;

        isLoaded = true;

        StartCoroutine(FadeImage(true));
    }

    public void Clear()
    {
        if (!isLoaded)
            return;

        username = string.Empty;
        character = null;

        isLoaded = false;

        fadeImage = true;
        fadeText = true;

        StartCoroutine(FadeImage(false));
    }
}
