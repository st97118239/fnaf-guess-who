using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ListInfoPanel : MonoBehaviour
{
    public ListPanel listPanel;
    public AudioManager audioManager;
    public Character character;
    public ListPolaroid polaroidSlot;
    public Polaroid polaroid;
    public GameObject infoPaper;
    public Transform bodyPaperParent;
    public Note audioNote;
    public Note avatarNote;
    public Note chooseNote;
    public GameObject bodyPaperPrefab;
    public int bodyPapersIdx;
    public List<BodyPaper> bodyPapers;
    public List<string> variables;
    public List<RectTransform> lines;
    public List<TMP_Text> texts;
    public Vector3 bodyImageOffset;

    [SerializeField] private Subtitles subtitles;
    [SerializeField] private GameObject backgroundBlocker;
    [SerializeField] private float paperTimerBase;

    private Animator animator;
    private bool isPlayingAudio;
    private bool playPaperTimer;
    private float paperTimer;
    private int bodyPaperClicksQueue;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!listPanel.isInfoPanelShown)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Hide();
        else if (Input.GetKeyDown(KeyCode.Space))
            AudioButtonPressed();

        if (playPaperTimer)
        {
            if (paperTimer > 0)
                paperTimer -= Time.deltaTime;
            else
                playPaperTimer = false;
        }
        else
        {
            if (bodyPaperClicksQueue > 0)
                BodyPapersNext(false);
            else if (bodyPaperClicksQueue < 0)
                BodyPapersBack(false);
        }
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

        audioManager.voicelines.Stop();

        character.voicelines.Play(audioManager.voicelines, subtitles);

        Invoke(nameof(ResetAudioButton), character.voicelines.voicelineLength);

        audioNote.ChangeImage("UI/Stop");
    }

    private void ResetAudioButton()
    {
        if (!character.voicelines)
            return;

        isPlayingAudio = false;
        audioNote.ChangeImage("UI/Play");
    }

    public void StopAudio()
    {
        audioManager.voicelines.Stop();
        subtitles.Reset();
        CancelInvoke(nameof(ResetAudioButton));
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
            polaroid.characterImage.sprite = character.polaroidSprite[0];
            polaroid.gameObject.SetActive(true);
        }
        else
            polaroid.gameObject.SetActive(false);

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

        if (character.voicelines)
        {
            audioNote.gameObject.SetActive(true);
            audioNote.ChangeImage("UI/Play");
        }
        else
            audioNote.gameObject.SetActive(false);

        if (listPanel.mainPanel.avatar == character.directory || listPanel.mainPanel.settingsMenu.isConnected || (character.winsNeeded > PlayerPrefs.GetInt("Wins") && !listPanel.devManager.unlockAllCharacters))
        {
            avatarNote.Disable();
        }
        else
            avatarNote.Enable();

        if (listPanel.menu == 1)
        {
            chooseNote.ChangeText("Remove");
            if (listPanel.openedList.builtIn && !listPanel.devManager.isUnlocked)
                chooseNote.Disable();
            else
                chooseNote.Enable();
        }
        else if (listPanel.menu == 2)
        {
            chooseNote.ChangeText("Add");
            if ((listPanel.openedList.builtIn && !listPanel.devManager.isUnlocked) || !listPanel.hasListOpen || !polaroidSlot.characterCanAdd || listPanel.openedList.characters.Count >= listPanel.maxCharacters)
                chooseNote.Disable();
            else
                chooseNote.Enable();
        }
        else
            chooseNote.gameObject.SetActive(false);

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
            bodyPapersIdx--;

            if (bodyPapers[bodyPapersIdx].index == 0)
                bodyPapers[bodyPapersIdx].animator.SetTrigger("FirstBack");
            else
                bodyPapers[bodyPapersIdx].animator.SetTrigger("OthersBack");

            needsToWait = true;
        }

        StartCoroutine(Close(needsToWait));
    }

    private IEnumerator Close(bool hasToWait)
    {
        if (hasToWait)
            yield return new WaitForSeconds(0.75f);

        animator.SetTrigger("FolderClose");
        audioManager.soundEffects.PlayOneShot(audioManager.folderCloseSFX);
        Invoke(nameof(DisableBackground), 0.6f);
        character = null;
        polaroidSlot = null;
        listPanel.isInfoPanelShown = false;
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }

    public void BodyPapersNext(bool manualClick)
    {
        if (manualClick && bodyPapersIdx < bodyPapers.Count - 1)
            bodyPaperClicksQueue++;

        if (!playPaperTimer && bodyPapersIdx < bodyPapers.Count - 1)
        {
            if (bodyPapers[bodyPapersIdx].index == 0)
                bodyPapers[bodyPapersIdx].animator.SetTrigger("FirstNext");
            else
                bodyPapers[bodyPapersIdx].animator.SetTrigger("OthersNext");

            bodyPapersIdx++;
            bodyPaperClicksQueue--;

            paperTimer = paperTimerBase;
            playPaperTimer = true;
        }

        if (bodyPapersIdx >= bodyPapers.Count - 1)
            bodyPaperClicksQueue = 0;
    }

    public void BodyPapersBack(bool manualClick)
    {
        if (manualClick && bodyPapersIdx > 0)
            bodyPaperClicksQueue--;

        if (!playPaperTimer && bodyPapersIdx > 0)
        {
            bodyPapersIdx--;
            bodyPaperClicksQueue++;

            if (bodyPapers[bodyPapersIdx].index == 0)
                bodyPapers[bodyPapersIdx].animator.SetTrigger("FirstBack");
            else
                bodyPapers[bodyPapersIdx].animator.SetTrigger("OthersBack");

            paperTimer = paperTimerBase;
            playPaperTimer = true;
        }

        if (bodyPapersIdx <= 0)
            bodyPaperClicksQueue = 0;
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

        backgroundBlocker.SetActive(true);
        animator.SetTrigger("FolderOpen");
        audioManager.soundEffects.PlayOneShot(audioManager.folderOpenSFX);

        listPanel.isInfoPanelShown = true;
    }

    public void ChooseCharacter()
    {
        if (listPanel.menu == 1 && (!listPanel.openedList.builtIn || listPanel.devManager.isUnlocked))
        {
            listPanel.RemoveCharacterFromList(character, polaroidSlot.index);

            Hide();
        }
        else if (listPanel.menu == 2 && (!listPanel.openedList.builtIn || listPanel.devManager.isUnlocked))
        {
            listPanel.AddCharacterToList(character);

            Hide();
        }
    }

    public void AvatarNote()
    {
        if (listPanel.mainPanel.avatar != character.directory && !listPanel.mainPanel.settingsMenu.isConnected)
        {
            listPanel.mainPanel.settingsMenu.settings.avatar = character.directory;
            listPanel.mainPanel.avatar = character.directory;
            listPanel.mainPanel.settingsMenu.Save();
            listPanel.mainPanel.SetPlayerPolaroid(true, false);

            Hide();
        }
    }
}
