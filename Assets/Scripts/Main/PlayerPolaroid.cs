using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPolaroid : MonoBehaviour, IPointerClickHandler
{
    public bool isLoaded;
    public bool isReady;

    [SerializeField] private bool isOpponent;
    [SerializeField] private Polaroid pol;
    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField] private float checkmarkTime = 0.3f;
    [SerializeField] private MainPanel mainPanel;

    private string username;
    private Character character;

    private bool fadeImage;
    private bool fadeText;

    private Color nameColor;
    private Color normalNameColor = Color.black;
    private Color devNameColor = new Color(172, 0, 0, 1);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!mainPanel.playerPanel.isShown)
            {
                mainPanel.playerPanel.Show(isOpponent);
            }
        }
    }

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
                    pol.characterText.color = Color.Lerp(Color.clear, nameColor, fillAmount);
            }
            else if(!fadeIn)
            {
                if (fadeImage)
                    pol.characterImage.color = Color.Lerp(Color.white, Color.clear, fillAmount);
                if (fadeText)
                    pol.characterText.color = Color.Lerp(nameColor, Color.clear, fillAmount);
            }

            yield return null;
        }
    }

    private IEnumerator Checkmark(bool fill)
    {
        yield return null;

        for (float i = 0; i <= checkmarkTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > checkmarkTime) i = checkmarkTime;

            float fillAmount = i / checkmarkTime;
            pol.polXImage.fillAmount = fill ? fillAmount : 1 - fillAmount;

            yield return null;
        }
    }

    public void Load(string givenName, string givenCharDir, bool isDev, bool shouldFadeImage, bool shouldFadeText)
    {
        if (shouldFadeImage)
            pol.characterImage.color = Color.clear;
        if (shouldFadeText)
            pol.characterText.color = Color.clear;

        username = givenName;
        character = Resources.Load<Character>(givenCharDir);

        if (!character)
        {
            mainPanel.avatar = "Characters/FNAF1/Freddy";
            character = Resources.Load<Character>(mainPanel.avatar);
        }

        if (character.polaroidSprite[0])
            pol.characterImage.sprite = character.polaroidSprite[0];

        pol.characterText.text = username;

        fadeImage = shouldFadeImage;
        fadeText = shouldFadeText;

        if (isDev)
            nameColor = devNameColor;
        else
            nameColor = normalNameColor;

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

    public void Ready(bool toggle)
    {
        if (toggle == true && !isReady)
            StartCoroutine(Checkmark(toggle));
        else if (toggle == false && isReady)
            StartCoroutine(Checkmark(toggle));

        isReady = toggle;
    }
}
