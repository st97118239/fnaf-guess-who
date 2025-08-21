using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public Button noteButton;
    public TMP_Text noteText;
    public Image noteXImage;

    [SerializeField] private bool isButton;
    [SerializeField] private bool startCrossedOff;

    private Color transparent = new(255, 255, 255, 0);
    private Color opaque = new(255, 255, 255, 255);

    private void Awake()
    {
        if (isButton && !noteButton)
            noteButton = GetComponent<Button>();
        if (!noteText)
            noteText = GetComponentInChildren<TMP_Text>();
        if (!noteXImage)
            noteXImage = transform.Find("X").GetComponent<Image>();

        if (noteXImage)
        {
            if (startCrossedOff)
            {
                noteXImage.color = opaque;
                if (isButton)
                    noteButton.interactable = false;
            }
            else if (!startCrossedOff)
            {
                noteXImage.color = transparent;
                if (isButton)
                    noteButton.interactable = true;
            }
        }
    }

    public void Enable()
    {
        noteXImage.color = transparent;
        noteButton.interactable = true;
    }

    public void Disable()
    {
        noteXImage.color = opaque;
        noteButton.interactable = false;
    }

    public void ChangeText(string txt)
    {
        noteText.text = txt;
    }
}
