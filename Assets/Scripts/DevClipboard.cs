using UnityEngine;
using UnityEngine.UI;

public class DevClipboard : MonoBehaviour
{
    public bool isShown;

    [SerializeField] private DevManager devManager;

    [SerializeField] private GameObject devClipboard;
    [SerializeField] private Animator clipboardAnimator;
    [SerializeField] private GameObject backgroundBlocker;

    [SerializeField] private Toggle unlockAllCharsToggle;

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
            CloseClipboard();
    }

    public void OpenClipboard()
    {
        clipboardAnimator.SetTrigger("PaperOpen");
        backgroundBlocker.SetActive(true);

        isShown = true;
    }

    public void CloseClipboard()
    {
        clipboardAnimator.SetTrigger("PaperClose");
        Invoke(nameof(DisableBackground), 0.6f);
        Save();

        isShown = false;
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }

    public void UnlockAllCharsBox()
    {

    }

    public void Save()
    {
        devManager.unlockAllCharacters = unlockAllCharsToggle.isOn;
    }
}
