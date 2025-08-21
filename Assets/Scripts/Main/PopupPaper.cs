using TMPro;
using UnityEngine;

public class PopupPaper : MonoBehaviour
{
    [SerializeField] private TMP_Text paperText;
    [SerializeField] private Note note;
    [SerializeField] private GameObject backgroundBlocker;

    [SerializeField] private Animator animator;

    private Error error;

    public void Show(Error givenError)
    {
        error = givenError;

        if (error == Error.Unkown || error == Error.None)
        {
            paperText.text = "Unkown error.";
            error = Error.Unkown;
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.NoConnection)
        {
            paperText.text = "No connection to internet. Check the IP Address, port and your internet. Otherwise try again later.";
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.MultipleHosts)
        {
            paperText.text = "Can't start host due to there already being a host on this ip address. Please turn off that host or try again later.";
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.ServerFull)
        {
            paperText.text = "Server is full or not active. If this is your server, then please check if your server is running and if the IP address and port are correct.";
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.Disconnected)
        {
            paperText.text = "You got disconnected from the server.";
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.LostConnection)
        {
            paperText.text = "Lost cconnection to the server. Check your internet or try again later.";
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.ServerDisconnected)
        {
            paperText.text = "Server disconnected. Change Server IP Address and/or port or try again later.";
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.OpponentLeft)
        {
            paperText.text = "Your opponent has been disconnected from the server.";
            note.ChangeText("Return");
            note.Enable();
        }
        else if (error == Error.HostLeft)
        {
            paperText.text = "The server host has disconnected.";
            note.ChangeText("Return");
            note.Enable();
        }

        backgroundBlocker.SetActive(true);
        gameObject.SetActive(true);
        animator.SetTrigger("PaperOpen");
    }

    public void NoteButton()
    {
        animator.SetTrigger("PaperClose");

        error = Error.None;
        backgroundBlocker.SetActive(false);
    }
}
