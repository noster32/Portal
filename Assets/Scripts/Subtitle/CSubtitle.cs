using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CSubtitle : CComponent
{
    [SerializeField] private RectTransform testBoxRect;
    private RectTransform rectTransform;

    private Coroutine boxLerpCoroutine;
    private TextMeshProUGUI textMeshPro;
    private CCameraFade fade;

    private List<string> subtitleList = new List<string>();

    public override void Awake()
    {
        base.Awake();

        rectTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        fade = GetComponent<CCameraFade>();
    }

    public void SetText(string text, float lifetime = 6f)
    {
        StartCoroutine(SubtitleLiftTimeCoroutine(text, lifetime));
    }

    public override void Update()
    {
        base.Update();
      
        if(rectTransform.sizeDelta.y != testBoxRect.sizeDelta.y)
        {
            if(boxLerpCoroutine == null)
            {
                boxLerpCoroutine = StartCoroutine(BoxSizeLerpCoroutine());
            }
        }
    }

    private void SortingText()
    {
        string temp = string.Empty;

        for (int i = 0; i < subtitleList.Count; ++i)
        {
            if(i == 0)
                temp = subtitleList[i];
            else
                temp += "\n" + subtitleList[i];
        }

        textMeshPro.text = temp;
    }

    private IEnumerator SubtitleLiftTimeCoroutine(string subtitle, float lifetime)
    {
        if (lifetime < 6f)
            lifetime = 6f;

        subtitleList.Add(subtitle);

        if(subtitleList.Count == 1)
        {
            fade.FadeOut(0.5f);
            textMeshPro.text = subtitle;

            yield return new WaitForSeconds(lifetime);
        }
        else
        {
            SortingText();

            yield return new WaitForSeconds(lifetime + 0.5f);
        }


        subtitleList.Remove(subtitle);

        if(subtitleList.Count == 0)
        {
            fade.FadeIn(0.5f);
            textMeshPro.text = string.Empty;
        }
        else
        {
            SortingText();
        }

        yield return null;
    }

    private IEnumerator BoxSizeLerpCoroutine()
    {
        Vector2 start = testBoxRect.sizeDelta;
        Vector2 end = rectTransform.sizeDelta;
        float elapsedTime = 0f;
        float duration = 0.2f;

        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            testBoxRect.sizeDelta = Vector2.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        testBoxRect.sizeDelta = end;

        boxLerpCoroutine = null;
        yield return null;
    }
}
