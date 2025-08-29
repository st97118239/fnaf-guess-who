using System.Collections;
using UnityEngine;

public class EmptySlot : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public int index;

    [SerializeField] private int delay = 5;
    [SerializeField] private float floatDelay = 0.9f;
    [SerializeField] private float fadeTime = 1;
    [SerializeField] private bool fadeAtStart;
    [SerializeField] private bool useFloatDelay;

    private bool shouldReverse;
    private bool shouldDestroySelf;
    private bool shouldDestroyChild;

    private void Start()
    {
        if (fadeAtStart)
            Play(false, false, false);
    }

    private IEnumerator FadeImage()
    {
        yield return null;

        if (transform.childCount == 0)
        {
            //canvasGroup.alpha = shouldReverse ? 0 : 1;

            if (shouldDestroyChild)
            {
                foreach (Transform child in gameObject.transform)
                    Destroy(child.gameObject);
            }

            if (shouldDestroySelf)
                Destroy(gameObject);

            yield break;
        }

        if (useFloatDelay)
        {
            for (float i = 0; i <= index * (floatDelay / 2); i += Time.deltaTime)
            {
                yield return null;
            }
        }
        else
        {
            for (int i = 0; i < index * delay; i++)
            {
                yield return null;
            }
        }

        float alphaToFade = shouldReverse ? canvasGroup.alpha : 1 - canvasGroup.alpha;
        float amountToFade = alphaToFade / (fadeTime / 0.005f);
        for (float i = 0; i <= fadeTime; i += 0.005f)
        {
            canvasGroup.alpha += amountToFade * (shouldReverse ? -1 : 1);

            yield return null;
        }

        canvasGroup.alpha = shouldReverse ? 0 : 1;
        
        if (shouldDestroyChild)
        {
            foreach (Transform child in gameObject.transform)
                Destroy(child.gameObject);
        }

        if (shouldDestroySelf)
            Destroy(gameObject);
    }

    public void Reset()
    {
        StopCoroutine(nameof(FadeImage));
        canvasGroup.alpha = 0;
    }

    public void Play(bool givenReverse, bool givenDestroyChild, bool givenDestroySelf)
    {
        canvasGroup.alpha = givenReverse ? 1 : 0;

        shouldReverse = givenReverse;
        shouldDestroyChild = givenDestroyChild;
        shouldDestroySelf = givenDestroySelf;
        StartCoroutine(nameof(FadeImage));
    }
}