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

        for (int i = 0; i < index * 15; i++)
        {
            yield return null;
        }

        // loop over 1 second
        for (float i = 0; i <= 1.5; i += Time.deltaTime)
        {
            // set color with i as alpha
            canvasGroup.alpha = i / 1.5f;
            yield return null;
        }
    }
}