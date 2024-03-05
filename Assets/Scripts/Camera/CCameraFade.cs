using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class CCameraFade : CComponent
{
    [SerializeField] CanvasGroup canvasGroup;
    private bool isFadeIn = false;

    public void StartFadeIn(float duration, bool destroy = false)
    {
        if (!isFadeIn)
        {
            isFadeIn = true;
            StartCoroutine(FadeIn(duration, destroy));
        }
    }

    public void StartFlicking(float duration, bool destroy = false)
    {
        StartCoroutine(Flicking(duration, destroy));
    }

    private IEnumerator FadeIn(float duration, bool destroy)
    {
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / duration;
            yield return null;
        }

        if (destroy)
            Destroy(canvasGroup.gameObject);
    }

    private IEnumerator Flicking(float duration, bool destroy)
    {
        float flickTImeScale = 0.5f;                //alpha�� 0.5�� ���ϴµ� 0.5�ʰ� �ɸ��µ� �� 0.5�ʿ� ���� ����
                                                    //���� �� ���� 0.5��� alpha�� ���ϴµ� �ɸ��� �ð��� 0.25��
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
