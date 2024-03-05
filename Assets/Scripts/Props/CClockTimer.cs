using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CClockTimer : CComponent
{
    [Header("Setting")]
    [SerializeField] private float timerTime = 60f;
    [SerializeField] private bool sharedTime = false;

    [Header("Material")]
    [SerializeField] private Renderer minutesRenderer;
    [SerializeField] private Renderer secondsRenderer;
    [SerializeField] private Renderer milliSecondsTenRenderer;
    [SerializeField] private Renderer milliSecondsOneRenderer;
    [SerializeField] private Renderer centiSecondsRenderer;

    private Material minutesMaterial;
    private Material secondsMaterial;
    private Material milliSecondsTenMaterial;
    private Material milliSecondsOneMaterial;
    private Material centiSecondsMaterial;

    [Header("Texture")]
    [SerializeField] private Texture[] secondsTexture = new Texture[60];
    [SerializeField] private Texture[] centiSecondsTexture = new Texture[10];

    private Coroutine timerCoroutine;

    public override void Awake()
    {
        base.Awake();

        if(sharedTime)
        {
            minutesMaterial = minutesRenderer.sharedMaterial;
            secondsMaterial = secondsRenderer.sharedMaterial;
            milliSecondsTenMaterial = milliSecondsTenRenderer.sharedMaterial;
            milliSecondsOneMaterial = milliSecondsOneRenderer.sharedMaterial;
            centiSecondsMaterial = centiSecondsRenderer.sharedMaterial;
        }
        else
        {
            minutesMaterial = minutesRenderer.material;
            secondsMaterial = secondsRenderer.material;
            milliSecondsTenMaterial = milliSecondsTenRenderer.material;
            milliSecondsOneMaterial = milliSecondsOneRenderer.material;
            centiSecondsMaterial = centiSecondsRenderer.material;
        }
    }

    public override void Start()
    {
        base.Start();

        minutesMaterial.mainTexture = secondsTexture[1];
        secondsMaterial.mainTexture = secondsTexture[0];
        milliSecondsTenMaterial.mainTexture = secondsTexture[0];
        milliSecondsOneMaterial.mainTexture = centiSecondsTexture[0];
        centiSecondsMaterial.mainTexture = centiSecondsTexture[0];
    }

    public void StartTimer()
    {
        if(timerCoroutine == null)
            timerCoroutine = StartCoroutine(TimerCoroutine(timerTime));
    }

    private IEnumerator TimerCoroutine(float duration)
    {
        float elapsedTime = duration;

        while (elapsedTime >= 0f)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);

            minutesMaterial.mainTexture = secondsTexture[timeSpan.Minutes];
            secondsMaterial.mainTexture = secondsTexture[timeSpan.Seconds];
            milliSecondsTenMaterial.mainTexture = secondsTexture[timeSpan.Milliseconds / 100];
            milliSecondsOneMaterial.mainTexture = centiSecondsTexture[UnityEngine.Random.Range(0, centiSecondsTexture.Length)];
            centiSecondsMaterial.mainTexture = centiSecondsTexture[UnityEngine.Random.Range(0, centiSecondsTexture.Length)];

            elapsedTime -= Time.deltaTime;

            yield return null;
        }

        minutesMaterial.mainTexture = secondsTexture[0];
        secondsMaterial.mainTexture = secondsTexture[0];
        milliSecondsTenMaterial.mainTexture = secondsTexture[0];
        milliSecondsOneMaterial.mainTexture = centiSecondsTexture[0];
        centiSecondsMaterial.mainTexture = centiSecondsTexture[0];

        timerCoroutine = null;
        yield return null;
    }
}
