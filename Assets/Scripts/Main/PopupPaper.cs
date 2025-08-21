using TMPro;
using UnityEngine;

public class PopupPaper : MonoBehaviour
{
    [SerializeField] private TMP_Text paperText;
    [SerializeField] private Note note;
    [SerializeField] private GameObject backgroundBlocker;

    [SerializeField] private Animator animator;

    private string error;

    public void Show(string givenError)
    {
        error = givenError;

        if (error == "MultipleHosts")
        {
            paperText.text = "Can not start host due to there already being a host on this ip address. Please turn off that host or try again later.";
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

        error = string.Empty;
        backgroundBlocker.SetActive(false);
    }
}
