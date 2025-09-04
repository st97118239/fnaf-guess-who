using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Button;

public class PlayerPolaroid : MonoBehaviour, IPointerClickHandler
{
    public bool isLoaded;
    public bool isReady;

    [SerializeField] private bool isOpponent;
    [SerializeField] private Polaroid pol;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField] private float checkmarkTime = 0.3f;
    [SerializeField] private MainPanel mainPanel;

    private string username;
    private Character character;
    private int level;

    private bool fadeImage;
    private bool fadeName;
    private bool fadeLevel;

    private Color nameColor;
    private readonly Color normalNameColor = Color.black;
    private readonly Color devNameColor = new Color(172, 0, 0, 1);

    [SerializeField]
    private ButtonClickedEvent m_OnRightClick = new ButtonClickedEvent();

    public ButtonClickedEvent onRightClick
    {
        get => m_OnRightClick;
        set => m_OnRightClick = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            m_OnRightClick?.Invoke();
        }
    }

    private IEnumerator FadeImage(bool fadeIn)
    {
        yield return null;

        for (float i = 0; i <= fadeTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;
            
            switch (fadeIn)
            {
                case true:
                {
                    if (fadeImage)
                        pol.characterImage.color = Color.Lerp(Color.clear, Color.white, fillAmount);
                    if (fadeName)
                        pol.characterText.color = Color.Lerp(Color.clear, nameColor, fillAmount);
                    if (fadeLevel)
                        levelText.color = Color.Lerp(Color.clear, Color.white, fillAmount);
                    break;
                }
                case false:
                {
                    if (fadeImage)
                        pol.characterImage.color = Color.Lerp(Color.white, Color.clear, fillAmount);
                    if (fadeName)
                        pol.characterText.color = Color.Lerp(nameColor, Color.clear, fillAmount);
                    if (fadeLevel)
                        levelText.color = Color.Lerp(Color.white, Color.clear, fillAmount);
                    break;
                }
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

    public void OnRightClick()
    {
        if (!mainPanel.playerPanel.isShown)
        {
            mainPanel.playerPanel.Show(isOpponent);
        }
    }

    public void Load(Player player, bool usePlayer, bool shouldFadeImage, bool shouldFadeName, bool shouldFadeLevel)
    {
        if (shouldFadeImage)
            pol.characterImage.color = Color.clear;
        if (shouldFadeName)
            pol.characterText.color = Color.clear;

        if (usePlayer)
        {
            username = player.username;
            character = Resources.Load<Character>(player.avatar);
            level = player.level;

            nameColor = player.isDev ? devNameColor : normalNameColor;
        }
        else
        {
            username = mainPanel.username;
            character = Resources.Load<Character>(mainPanel.avatar);
            level = PlayerPrefs.GetInt("Level");

            nameColor = mainPanel.devManager.isUnlocked ? devNameColor : normalNameColor;
        }

        if (!character)
        {
            mainPanel.avatar = "Characters/FNAF1/Freddy";
            character = Resources.Load<Character>(mainPanel.avatar);
        }

        if (character.polaroidSprite[0])
            pol.characterImage.sprite = character.polaroidSprite[0];

        pol.characterText.text = username;
        levelText.text = level.ToString();

        fadeImage = shouldFadeImage;
        fadeName = shouldFadeName;
        fadeLevel = shouldFadeLevel;

        isLoaded = true;

        StartCoroutine(FadeImage(true));
    }

    public void Clear()
    {
        if (!isLoaded)
            return;

        character = null;
        username = string.Empty;
        level = 0;

        isLoaded = false;

        fadeImage = true;
        fadeName = true;
        fadeLevel = true;

        StartCoroutine(FadeImage(false));
    }

    public void Ready(bool toggle)
    {
        if ((toggle && !isReady) || (!toggle && isReady))
            StartCoroutine(Checkmark(toggle));

        isReady = toggle;
    }
}
