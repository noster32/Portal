using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CLightRail : CComponent
{
    [SerializeField] float speed = 1.37f;
    [SerializeField] float waitTIme = 3f;
    [SerializeField] bool isPlayerWait = false;
    
    [SerializeField] Transform platformTransform;
    [SerializeField] Transform startTransform;
    [SerializeField] Transform endTransform;
    [SerializeField] Animator[] endcapAnimator;

    private bool isActive = false;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform[] lineTransform;
    private StudioEventEmitter emitter;

    public override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void Start()
    {
        base.Start();

        emitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.beamPlatformLoop1, platformTransform.gameObject);
        lineRenderer.positionCount = lineTransform.Length;
        for(int i = 0; i < lineTransform.Length; ++i) 
        {
            lineRenderer.SetPosition(i, lineTransform[i].position);
        }
    }

    public bool GetIsActive() => isActive;

    public bool GetIsPlayerWait() => isPlayerWait;

    public bool SetIsPlayerWaitFalse() => isPlayerWait = false;

    public void StartLightRail()
    {
        if(!isActive)
        {
            isActive = true;

            float distance = Vector3.Distance(startTransform.position, endTransform.position);
            float totalTime = distance / speed;

            StartCoroutine(StartLightRailCoroutine(startTransform.position, endTransform.position, totalTime, waitTIme));
        }
    }

    public void StartLightRailRepeat()
    {
        if(!isActive)
        {
            isActive = true;

            float distance = Vector3.Distance(startTransform.position, endTransform.position);
            float totalTime = distance / speed;

            StartCoroutine(RepeatLightRailCoroutine(startTransform.position, endTransform.position, totalTime, waitTIme));
        }
    }

    private IEnumerator StartLightRailCoroutine(Vector3 startPos, Vector3 endPos, float duration, float waitSec)
    {
        foreach(Animator anim in endcapAnimator)
        {
            anim.Play("movedown");
        }

        Vector3 originPos = platformTransform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < 0.834f)
        {
            float t = elapsedTime / 0.834f;

            platformTransform.localPosition = Vector3.Lerp(originPos, new Vector3(0f, 0.75f, 0f), t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        platformTransform.localPosition = new Vector3(0f, 0.75f, 0f);

        float animWaitSec = 2.042f - 0.834f;
        yield return new WaitForSeconds(animWaitSec);

        originPos = platformTransform.position;
        elapsedTime = 0f;

        emitter.Play();

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            platformTransform.position = Vector3.Lerp(originPos, endPos, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        platformTransform.position = endPos;
        emitter.Stop();
        if (!isPlayerWait)
        {
            yield return new WaitForSeconds(waitSec);

            StartCoroutine(RepeatLightRailCoroutine(startPos, endPos, duration, waitSec));
        }
        else
            isActive = false;

        yield return null;
    }

    private IEnumerator RepeatLightRailCoroutine(Vector3 startPos, Vector3 endPos, float duration, float waitSec)
    {
        float elapsedTime;
        while(true)
        {
            elapsedTime = 0f;
            emitter.Play();

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;

                platformTransform.position = Vector3.Lerp(endPos, startPos, t);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            platformTransform.position = startPos;
            emitter.Stop();

            yield return new WaitForSeconds(waitSec);

            elapsedTime = 0f;
            emitter.Play();

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;

                platformTransform.position = Vector3.Lerp(startPos, endPos, t);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            platformTransform.position = endPos;
            emitter.Stop();

            yield return new WaitForSeconds(waitSec);
        }
    }
}
