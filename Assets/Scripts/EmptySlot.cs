using System.Collections;
using UnityEngine;

public class EmptySlot : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public int index;

    private void Start()
    {
        StartCoroutine(FadeImage());
    }

    private IEnumerator FadeImage()
    {
        yield return null;

        for (int i = 0; i < index * 5; i++)
        {
            yield return null;
        }

        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            canvasGroup.alpha = i / 1f;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}