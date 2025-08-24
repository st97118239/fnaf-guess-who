using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevClipboard : MonoBehaviour
{
    public bool isShown;

    [SerializeField] private DevManager devManager;
    [SerializeField] private ListPanel listPanel;
    [SerializeField] private CharactersPanel characterPanel;

    [SerializeField] private GameObject devClipboard;
    [SerializeField] private Animator clipboardAnimator;
    [SerializeField] private GameObject backgroundBlocker;
    [SerializeField] private GameObject paper2;

    [SerializeField] private Toggle unlockAllCharsToggle;

    private bool isUnlocked;

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
            CloseClipboard();
    }

    public void OpenClipboard()
    {
        if (!isUnlocked)
            paper2.SetActive(false);

        clipboardAnimator.SetTrigger("PaperOpen");
        backgroundBlocker.SetActive(true);

        isShown = true;
    }

    public void CloseClipboard()
    {
        clipboardAnimator.SetTrigger("PaperClose");
        Invoke(nameof(DisableBackground), 0.6f);
        Save();

        bool isOnListPanel = false;

        if (listPanel.hasListOpen && listPanel.menu == 1)
            isOnListPanel = true;

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
    }

    public void Save()
    {
        devManager.unlockAllCharacters = unlockAllCharsToggle.isOn;
    }
}
