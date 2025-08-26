using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Subtitles : MonoBehaviour
{
    public TMP_Text subtitleText;
    public GameObject background;

    public void Play(string givenText, float delay)
    {
        background.SetActive(true);
        subtitleText.text = givenText;
        Invoke(nameof(Reset), delay);
    }

    public void Reset()
    {
        CancelInvoke(nameof(Reset));
        background.SetActive(false);
        subtitleText.text = string.Empty;
    }
}
