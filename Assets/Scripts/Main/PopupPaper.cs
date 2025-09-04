using TMPro;
using UnityEngine;

public class PopupPaper : MonoBehaviour
{
    public bool canShow = true;

    [SerializeField] private TMP_Text paperText;
    [SerializeField] private Note note;
    [SerializeField] private GameObject backgroundBlocker;
    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private Animator animator;

    private Error error;
    private PopupTextType textType;
    private bool isShown;

    private static readonly int PaperOpen = Animator.StringToHash("PaperOpen");
    private static readonly int PaperClose = Animator.StringToHash("PaperClose");

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
            NoteButton();
    }

    public void ShowError(Error givenError)
    {
        if (!canShow || isShown)
            return;

        isShown = true;

        textType = PopupTextType.Error;
        error = givenError;

        switch (error)
        {
            case Error.Unknown:
            case Error.None:
                paperText.text = "Unknown error.";
                error = Error.Unknown;
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.NoConnection:
                paperText.text = "No connection to internet. Check the IP Address, port and your internet. Otherwise try again later.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.MultipleHosts:
                paperText.text = "Can't start host due to there already being a host on this ip address. Please turn off that host or try again later.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.ServerFull:
                paperText.text = "Server is full or not active. If this is your server, then please check if your server is running and if the IP address and port are correct.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.Disconnected:
                paperText.text = "You got disconnected from the server.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.LostConnection:
                paperText.text = "Lost connection to the server. Check your internet or try again later.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.ServerDisconnected:
                paperText.text = "Server disconnected. Check Server IP Address and/or port or try again later.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.OpponentLeft:
                paperText.text = "Your opponent has been disconnected from the server.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.HostLeft:
                paperText.text = "The server host has disconnected.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.CantConnect:
                paperText.text = "Couldn't connect to the server. Check the IP Address and port or try again later.";
                note.ChangeText("Return");
                note.Enable();
                break;
            case Error.WrongVersion:
                paperText.text = "You or the host has an outdated version of the game. Please update.";
                note.ChangeText("Return");
                note.Enable();
                break;
        }

        backgroundBlocker.SetActive(true);
        gameObject.SetActive(true);
        animator.SetTrigger(PaperOpen);

        audioManager.soundEffects.PlayOneShot(audioManager.clipboardSFX);
    }

    public void ShowPopup(PopupTextType givenType)
    {
        if (!canShow || isShown)
            return;

        isShown = true;

        textType = givenType;

        switch (givenType)
        {
            case PopupTextType.None:
                paperText.text = string.Empty;
                error = Error.Unknown;
                note.ChangeText("Return");
                note.Enable();
                break;
            case PopupTextType.Error:
                ShowError(error == Error.None ? Error.None : error);
                break;
            case PopupTextType.LevelUp:
                switch (mainPanel.listPanel.charactersPanel.newCharacters.characters.Count)
                {
                    case 1:
                        paperText.text = "You levelled up and are now level" + PlayerPrefs.GetInt("Level") + ". And you have unlocked " + mainPanel.listPanel.charactersPanel.newCharacters.characters[0].characterName + "! You can add them to your list to play with them.";
                        break;
                    case > 1:
                        paperText.text = "You levelled up and are now level" + PlayerPrefs.GetInt("Level") + ". And you have unlocked " + mainPanel.listPanel.charactersPanel.newCharacters.characters.Count + " new characters! You can add these to your list to play with them.";
                        break;
                    default:
                        paperText.text = "You levelled up and are now level" + PlayerPrefs.GetInt("Level") + ". Congrats!";
                        break;
                }

                note.ChangeText("Return");
                note.Enable();
                break;
        }

        backgroundBlocker.SetActive(true);
        gameObject.SetActive(true);
        animator.SetTrigger(PaperOpen);

        audioManager.soundEffects.PlayOneShot(audioManager.clipboardSFX);
    }

    public void NoteButton()
    {
        animator.SetTrigger(PaperClose);
        audioManager.soundEffects.PlayOneShot(audioManager.clipboardSFX);

        isShown = false;
        error = Error.None;
        textType = PopupTextType.None;
        Invoke(nameof(DisableBackground), 0.6f);
    }

    private void DisableBackground()
    {
        backgroundBlocker.SetActive(false);
    }
}
