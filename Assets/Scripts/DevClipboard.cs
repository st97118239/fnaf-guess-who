using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevClipboard : MonoBehaviour
{
    public bool isShown;

    [SerializeField] private DevManager devManager;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private ListPanel listPanel;
    [SerializeField] private CharactersPanel characterPanel;

    [SerializeField] private GameObject devClipboard;
    [SerializeField] private Animator clipboardAnimator;
    [SerializeField] private GameObject backgroundBlocker;
    [SerializeField] private GameObject paper2;

    [SerializeField] private Toggle unlockAllCharsToggle;
    [SerializeField] private TMP_InputField winsField;
    [SerializeField] private TMP_InputField gamesField;
    [SerializeField] private TMP_InputField levelField;
    [SerializeField] private Note saveCharsNote;

    private bool isUnlocked;

    private static readonly int PaperOpen = Animator.StringToHash("PaperOpen");
    private static readonly int PaperClose = Animator.StringToHash("PaperClose");

    private void Start()
    {
        paper2.SetActive(false);
    }

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
            CloseClipboard();
    }

    public void OpenClipboard()
    {
        if (!isUnlocked)
            paper2.SetActive(false);

        winsField.text = PlayerPrefs.GetInt("Wins").ToString();
        gamesField.text = PlayerPrefs.GetInt("Games").ToString();
        levelField.text = PlayerPrefs.GetInt("Level").ToString();

        clipboardAnimator.SetTrigger(PaperOpen);
        backgroundBlocker.SetActive(true);

        isShown = true;
    }

    public void CloseClipboard()
    {
        clipboardAnimator.SetTrigger(PaperClose);
        Invoke(nameof(DisableBackground), 0.6f);
        Save();

        bool isOnListPanel = listPanel.hasListOpen && listPanel.menu == 1;

        if (characterPanel.hasLoaded)
        {
            if (characterPanel.loadedCategory == null)
                characterPanel.LoadCategoriesFade();
            else if (characterPanel.loadedCategory != null)
                characterPanel.RefreshPolaroids();
        }

        if (isOnListPanel)
            Invoke(nameof(ResetListPanelMenu), 0.8f);

        isShown = false;
    }

    public void ResetListPanelMenu()
    {
        listPanel.menu = 1;
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }

    public void NextPage()
    {
        isUnlocked = true;

        paper2.SetActive(true);

#if UNITY_EDITOR
        saveCharsNote.Enable();
#else
        saveCharsNote.Disable();
#endif
    }

    public void Save()
    {
        devManager.unlockAllCharacters = unlockAllCharsToggle.isOn;

        PlayerPrefs.SetInt("Wins", int.Parse(winsField.text));
        PlayerPrefs.SetInt("Games", int.Parse(gamesField.text));

        int levelFieldText = int.Parse(levelField.text);

        if (PlayerPrefs.GetInt("Level") != levelFieldText)
            saveManager.SetLevel(levelFieldText);
    }
}
