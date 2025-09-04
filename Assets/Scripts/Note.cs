using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public Button noteButton;
    public TMP_Text noteText;
    public Image noteOverlayImage;
    public Image noteXImage;

    public bool isCrossedOff;
    public bool isImageOn;

    [SerializeField] private bool isButton = true;
    [SerializeField] private bool startCrossedOff;
    [SerializeField] private bool startWithImage;

    [SerializeField] private float crossOffTime = 1;
    [SerializeField] private Vector2 spawnRotation = new(-4f,4f);

    [SerializeField] private RectTransform rectTransform;

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
            switch (startCrossedOff)
            {
                case true:
                    noteXImage.fillAmount = 1;
                    isCrossedOff = true;
                    noteButton.interactable = false;
                    break;
                case false:
                {
                    noteXImage.fillAmount = 0;
                    isCrossedOff = false;
                    noteButton.interactable = isButton;
                    break;
                }
            }
        }

        noteOverlayImage.fillAmount = startWithImage ? 1 : 0;

        rectTransform.localEulerAngles = new Vector3(0f, 0f, Random.Range(spawnRotation[0], spawnRotation[1]));
    }

    private IEnumerator FadeImage(bool fill, string imageDirectory, Image imageToChange)
    {
        yield return null;

        imageToChange.sprite = Resources.Load<Sprite>(imageDirectory);

        for (float i = 0; i <= crossOffTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > crossOffTime) i = crossOffTime;

            float fillAmount = i / crossOffTime;
            imageToChange.fillAmount = fill ? fillAmount : 1 - fillAmount;

            yield return null;
        }
    }

    public void Enable()
    {
        if (!isCrossedOff)
            return;

        noteXImage.fillOrigin = 0;
        noteButton.interactable = true;
        isCrossedOff = false;
        if (gameObject.activeSelf)
            StartCoroutine(FadeImage(false, "UI/X", noteXImage));
    }

    public void Disable()
    {
        if (isCrossedOff)
            return;

        noteXImage.fillOrigin = 0;
        noteButton.interactable = false;
        isCrossedOff = true;
        if (gameObject.activeSelf)
            StartCoroutine(FadeImage(true, "UI/X", noteXImage));
    }

    public void Checkmark()
    {
        noteXImage.fillOrigin = 1;
        noteButton.interactable = false;
        isCrossedOff = false;
        if (gameObject.activeSelf)
            StartCoroutine(FadeImage(true, "UI/Checkmark", noteXImage));
    }

    public void ChangeText(string txt)
    {
        noteText.text = txt;
    }

    public void ChangeImage(string imageDirectory)
    {
        noteOverlayImage.sprite = Resources.Load<Sprite>(imageDirectory);
    }

    public void EnableImage(string imageDirectory)
    {
        if (isImageOn)
            return;

        StartCoroutine(FadeImage(true, imageDirectory, noteOverlayImage));
        isImageOn = true;
    }

    public void DisableImage(string imageDirectory)
    {
        if (!isImageOn)
            return;

        StartCoroutine(FadeImage(false, imageDirectory, noteOverlayImage));
        isImageOn = false;
    }
}
