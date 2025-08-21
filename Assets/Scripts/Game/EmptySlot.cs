using System.Collections;
using UnityEngine;

public class EmptySlot : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public int index;

    [SerializeField] private int delay = 5;
    [SerializeField] private float fadeTime = 1;
    [SerializeField] private bool fadeAtStart;

    private void Start()
    {
        if (fadeAtStart)
            Play();
    }

    private IEnumerator FadeImage()
    {
        yield return null;

        for (int i = 0; i < index * delay; i++)
        {
            yield return null;
        }

        for (float i = 0; i <= fadeTime; i += Time.deltaTime)
        {
            canvasGroup.alpha = i / fadeTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    public void Reset()
    {
        StopCoroutine("FadeImage");
        canvasGroup.alpha = 0;
    }

    public void Play()
    {
        canvasGroup.alpha = 0;
        StartCoroutine("FadeImage");
    }
}