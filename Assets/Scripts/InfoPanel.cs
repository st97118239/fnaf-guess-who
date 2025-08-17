using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Game gameScript;
    public Character character;
    public GameObject polaroid;
    public GameObject infoPaper;
    public GameObject fullBodyPaper;
    public GameObject audioButton;
    public Image slotImage;
    public Image fullBodyImage;
    public Image audioImage;
    public List<string> variables;
    public List<RectTransform> lines;
    public List<TMP_Text> texts;

    private System.Random rnd = new();
    private AudioSource audioPlayer;
    private List<AudioClip> audioToPlay;
    private bool isPlayingAudio;

    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Hide();
    }

    public void AudioButtonPressed()
    {
        if (isPlayingAudio)
            StopAudio();
        else
            StartAudio();
    }

    public void StartAudio()
    {
        isPlayingAudio = true;

        audioPlayer.Stop();

        if (audioToPlay.Count == 0)
        {
            audioToPlay = character.voicelines;
            audioToPlay = audioToPlay.OrderBy(i => rnd.Next()).ToList();
        }

        audioPlayer.PlayOneShot(audioToPlay[0]);
        
        Invoke(nameof(ResetAudioButton), audioToPlay[0].length);

        audioImage.sprite = Resources.Load<Sprite>("UI/Stop");
    }

    private void ResetAudioButton()
    {
        if (character.voicelines.Count == 0)
            return;

        isPlayingAudio = false;
        audioToPlay.Remove(audioToPlay[0]);
        audioImage.sprite = Resources.Load<Sprite>("UI/Play");
    }

    public void StopAudio()
    {
        audioPlayer.Stop();
        ResetAudioButton();
    }

    public void Show(Character givenCharacter)
    {
        character = givenCharacter;

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].gameObject.SetActive(true);
        }

        if (character.polaroidSprite.Count > 0 && character.polaroidSprite[0])
        {
            slotImage.sprite = character.polaroidSprite[0];
            polaroid.SetActive(true);
        }
        else
            polaroid.SetActive(false);

        if (character.fullBodySprite.Count > 0 && character.fullBodySprite[0])
        {
            fullBodyImage.sprite = character.fullBodySprite[0];
            fullBodyPaper.SetActive(true);
        }
        else
            fullBodyPaper.SetActive(false);

        if (character.voicelines.Count > 0)
        {;
            audioButton.SetActive(true);
            audioImage.sprite = Resources.Load<Sprite>("UI/Play");
            audioToPlay = character.voicelines;
            audioToPlay = audioToPlay.OrderBy(i => rnd.Next()).ToList();
        }
        else
            audioButton.SetActive(false);

        bool hasAtLeastOneText = false;
        for (int i = 0; i < variables.Count; i++)
        {
            var fi = typeof(Character).GetField(variables[i]);
            texts[i].text = fi?.GetValue(character)?.ToString();
            hasAtLeastOneText |= !string.IsNullOrEmpty(texts[i].text);
        }

        infoPaper.SetActive(hasAtLeastOneText);

        Invoke(nameof(RedrawText), 0.1f);
    }

    public void Hide()
    {
        StopAudio();

        gameScript.animator.SetTrigger("FolderClose");
        character = null;
        gameScript.isInfoPanelShown = false;
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

        gameScript.animator.SetTrigger("FolderOpen");

        gameScript.isInfoPanelShown = true;
    }
}
