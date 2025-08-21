using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public Game gameScript;
    public Character opponentChar;
    public string playerAccused;
    public string playerCharName;
    public string opponentAccused;
    public GameObject polaroid;
    public GameObject bodyPaper;
    public Image slotImage;
    public Image bodyImage;
    public List<TMP_Text> texts;
    public List<RectTransform> lines;
    public Vector3 bodyImageOffset;
    public bool isShown;
    public string result;

    [SerializeField] private GameObject backgroundBlocker;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isShown)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Show(Character opponentCharacter, string playerSuspect, string playerCharacter, string opponentSuspected)
    {
        opponentChar = opponentCharacter;
        playerAccused = playerSuspect;
        playerCharName = playerCharacter;
        opponentAccused = opponentSuspected;

        if (opponentChar.polaroidSprite.Count > 0)
        {
            slotImage.sprite = opponentChar.polaroidSprite[0];
            polaroid.SetActive(true);
        }
        else
            polaroid.SetActive(false);

        if (opponentChar.fullBodySprite.Count > 0)
        {
            bodyImage.sprite = opponentChar.fullBodySprite[0];
            bodyPaper.SetActive(true);
        }
        else
            bodyPaper.SetActive(false);

        Invoke(nameof(RedrawText), 0.1f);
    }

    public void Close()
    {
        animator.SetTrigger("FolderClose");
        backgroundBlocker.SetActive(false);
        opponentChar = null;
        isShown = false;
    }

    private void RedrawText()
    {
        texts[0].text = result;
        texts[1].text = opponentChar.characterName;
        texts[2].text = playerAccused;
        texts[3].text = playerCharName;
        texts[4].text = opponentAccused;

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

        isShown = true;
    }
}
