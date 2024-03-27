using System.Collections;
using UnityEngine;

public class CCameraFade : CComponent
{
    [SerializeField] CanvasGroup canvasGroup;
    private bool isFadeIn = false;
    private Coroutine fadeCoroutine;

    public void StartFadeIn(float duration, bool destroy = false)
    {
        if (!isFadeIn)
        {
            isFadeIn = true;
            StartCoroutine(FadeInCoroutine(duration, destroy));
        }
    }

    public void StartFlicking(float duration, bool destroy = false)
    {
        StartCoroutine(Flicking(duration, destroy));
    }

    public void SetAlpha(float alphaValue)
    {
        canvasGroup.alpha = alphaValue;
    }

    public void FadeIn(float duration, float alphaValue = 1f, bool destroy = false)
    {
        canvasGroup.alpha = alphaValue;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInCoroutine(duration, destroy));
    }

    public void FadeOut(float duration, float alphaValue = 1f, bool destroy = false)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration, alphaValue, destroy));
    }

    private IEnumerator FadeInCoroutine(float duration, bool destroy)
    {
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / duration;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        if (fadeCoroutine != null)
            fadeCoroutine = null;

        if (destroy)
            Destroy(canvasGroup.gameObject);
    }

    private IEnumerator FadeOutCoroutine(float duration, float alphaValue, bool destroy)
    {
        while (canvasGroup.alpha < alphaValue)
        {
            canvasGroup.alpha += Time.deltaTime / duration;
            yield return null;
        }

        canvasGroup.alpha = alphaValue;

        if (fadeCoroutine != null)
            fadeCoroutine = null;

        if (destroy)
            Destroy(canvasGroup.gameObject);
    }


    public void FadeInUnscaledTime(float duration, float alphaValue = 1f, bool destroy = false)
    {
        canvasGroup.alpha = alphaValue;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInUnscaledTimeCoroutine(duration, destroy));
    }

    public void FadeOutUnscaledTime(float duration, float alphaValue = 1f, bool destroy = false)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutUnscaledTimeCoroutine(duration, alphaValue, destroy));
    }

    private IEnumerator FadeInUnscaledTimeCoroutine(float duration, bool destroy)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / duration;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        if (fadeCoroutine != null)
            fadeCoroutine = null;

        if (destroy)
            Destroy(canvasGroup.gameObject);
    }

    private IEnumerator FadeOutUnscaledTimeCoroutine(float duration, float alphaValue, bool destroy)
    {
        while (canvasGroup.alpha < alphaValue)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / duration;
            yield return null;
        }

        canvasGroup.alpha = alphaValue;

        if (fadeCoroutine != null)
            fadeCoroutine = null;

        if (destroy)
            Destroy(canvasGroup.gameObject);
    }

    private IEnumerator Flicking(float duration, bool destroy)
    {
        float flickTImeScale = 0.5f;                //alpha가 0.5가 변하는데 0.5초가 걸리는데 이 0.5초에 대한 배율
                                                    //만약 이 값이 0.5라면 alpha가 변하는데 걸리는 시간은 0.25초
        float waitTime = duration - flickTImeScale;

        if (duration <= 1f)
        {
            flickTImeScale = duration / 2f;
            waitTime = 0f;
        }

        while (canvasGroup.alpha < 0.5f)
        {
            canvasGroup.alpha += Time.deltaTime / flickTImeScale;

            yield return null;
        }

        canvasGroup.alpha = 0.5f;

        yield return new WaitForSeconds(waitTime);

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / flickTImeScale;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        if (destroy)
            Destroy(canvasGroup.gameObject);
    }
}
