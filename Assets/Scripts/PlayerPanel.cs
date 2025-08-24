using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public MainPanel mainPanel;
    public GameManager gameManager;
    public GameObject polaroid;
    public GameObject bodyPaper;
    public Image slotImage;
    public Image bodyImage;
    public List<TMP_Text> texts;
    public List<RectTransform> lines;
    public Vector3 bodyImageOffset;
    public bool isShown;
    public bool isOpponent;
    public Character avatar;
    public string username;
    public int wins;

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

    public void Show(bool givenIsOpponent)
    {
        isOpponent = givenIsOpponent;

        if (!isOpponent)
        {
            avatar = Resources.Load<Character>(mainPanel.avatar);
            username = mainPanel.username;
            wins = PlayerPrefs.GetInt("Wins", 0);
        }
        else if (isOpponent)
        {
            avatar = Resources.Load<Character>(gameManager.opponent.avatar);
            username = gameManager.opponent.username;
            wins = gameManager.opponent.wins;
        }

        if (avatar.polaroidSprite.Count > 0)
        {
            slotImage.sprite = avatar.polaroidSprite[0];
            polaroid.SetActive(true);
        }
        else
            polaroid.SetActive(false);

        if (avatar.fullBodySprite.Count > 0)
        {
            bodyImage.sprite = avatar.fullBodySprite[0];
            bodyPaper.SetActive(true);
        }
        else
            bodyPaper.SetActive(false);

        Invoke(nameof(RedrawText), 0.1f);
    }

    public void Close()
    {
        animator.SetTrigger("FolderClose");
        Invoke(nameof(DisableBackground), 0.6f);
        avatar = null;
        username = string.Empty;
        wins = 0;
        isShown = false;
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }

    private void RedrawText()
    {
        texts[0].text = username;
        texts[1].text = wins.ToString();

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
