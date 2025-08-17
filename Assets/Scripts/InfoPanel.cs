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
    public Transform bodyPaperParent;
    public GameObject audioButton;
    public Image slotImage;
    public Image audioImage;
    public GameObject bodyPaperPrefab;
    public int bodyPapersIdx;
    public List<BodyPaper> bodyPapers;
    public List<string> variables;
    public List<RectTransform> lines;
    public List<TMP_Text> texts;
    public Vector3 bodyImageOffset;

    private System.Random rnd = new();
    private AudioSource audioPlayer;
    private Animator animator;
    private List<AudioClip> audioToPlay;
    private bool isPlayingAudio;

    private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (gameScript.isInfoPanelShown && Input.GetKeyDown(KeyCode.Escape))
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
        for (int i = 0; i < bodyPapers.Count; i++)
        {
            Destroy(bodyPapers[i].gameObject);
        }

        bodyPapers.Clear();
        bodyPapersIdx = 0;

        character = givenCharacter;

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].gameObject.SetActive(true);
        }

        if (character.polaroidSprite.Count > 0)
        {
            slotImage.sprite = character.polaroidSprite[0];
            polaroid.SetActive(true);
        }
        else
            polaroid.SetActive(false);

        if (character.fullBodySprite.Count > 0)
        {
            for (int i = character.fullBodySprite.Count - 1; i >= 0; i--)
            {
                BodyPaper paper;
                if (i == 0)
                    paper = Instantiate(bodyPaperPrefab, bodyPaperParent.position, bodyPaperParent.rotation, bodyPaperParent).GetComponent<BodyPaper>();
                else
                    paper = Instantiate(bodyPaperPrefab, bodyPaperParent.position + bodyImageOffset, bodyPaperParent.rotation, bodyPaperParent).GetComponent<BodyPaper>();

                bodyPapers.Insert(0, paper);
                paper.Load(character.fullBodySprite[i], i);
            }
        }

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

        PutPapersBack();
    }

    private void PutPapersBack()
    {
        bool needsToWait = false;

        while (bodyPapersIdx != 0)
        {
            BodyPapersBack();
            needsToWait = true;
        }

        StartCoroutine(Close(needsToWait));
    }

    private IEnumerator Close(bool hasToWait)
    {
        if (hasToWait)
            yield return new WaitForSeconds(0.75f);

        animator.SetTrigger("FolderClose");
        character = null;
        gameScript.isInfoPanelShown = false;
    }

    public void BodyPapersNext()
    {
        if (bodyPapersIdx < bodyPapers.Count - 1)
        {
            if (bodyPapers[bodyPapersIdx].index == 0)
                bodyPapers[bodyPapersIdx].animator.SetTrigger("FirstNext");
            else
                bodyPapers[bodyPapersIdx].animator.SetTrigger("OthersNext");

            bodyPapersIdx++;
        }
    }

    public void BodyPapersBack()
    {
        if (bodyPapersIdx > 0)
        {
            bodyPapersIdx--;

            if (bodyPapers[bodyPapersIdx].index == 0)
                bodyPapers[bodyPapersIdx].animator.SetTrigger("FirstBack");
            else
                bodyPapers[bodyPapersIdx].animator.SetTrigger("OthersBack");
        }
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

        animator.SetTrigger("FolderOpen");

        gameScript.isInfoPanelShown = true;
    }
}
