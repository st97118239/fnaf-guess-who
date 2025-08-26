using TMPro;
using UnityEngine;

public class Subtitles : MonoBehaviour
{
    public TMP_Text subtitleText;

    public void Play(string givenText, float delay)
    {
        subtitleText.text = givenText;
        Invoke(nameof(Reset), delay);
    }

    public void Reset()
    {
        CancelInvoke(nameof(Reset));
        subtitleText.text = string.Empty;
    }
}
