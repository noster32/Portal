using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLightRail2 : CComponent
{
    [SerializeField] float speed = 1.37f;
    [SerializeField] float slowSpeed = 0.3f;

    [SerializeField] Transform platformTransform;
    [SerializeField] Transform startEmitterTransform;
    [SerializeField] Transform standByTransform;
    [SerializeField] private Transform[] destinationTransform;

    private bool isActive = false;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform[] lineTransform;
    private StudioEventEmitter platformEmitter;
    private StudioEventEmitter emitterEmitter;

    public override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void Start()
    {
        base.Start();

        platformEmitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.beamPlatformLoop1, platformTransform.gameObject);
        emitterEmitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.apcFirstgearLoop1, startEmitterTransform.gameObject);

        lineRenderer.positionCount = lineTransform.Length;
        for (int i = 0; i < lineTransform.Length; ++i)
        {
            lineRenderer.SetPosition(i, lineTransform[i].position);
        }

        StandByLightRail();
    }

    public void SetSpeedSlow()
    {
        this.speed = slowSpeed;
    }

    private void StandByLightRail()
    {
        StartCoroutine(StandByRightRailCoroutine(platformTransform, standByTransform));
    }

    private IEnumerator StandByRightRailCoroutine(Transform platform, Transform standby)
    {
        platformEmitter.Play();
        while (true)
        {
            if (Vector3.Distance(platform.position, standby.position) < 0.02f)
            {
                platform.position = standby.position;
                break;
            }

            platform.position = Vector3.MoveTowards(platform.position, standby.position, speed * Time.deltaTime);

            yield return null;
        }

        platformEmitter.Stop();
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.elevbell, platform.position);
        yield return null;
    }

    public void StartLightRail()
    {
        if (!isActive)
        {
            isActive = true;

            StartCoroutine(StartLightRailCoroutine(platformTransform, destinationTransform));
        }
    }

    private IEnumerator StartLightRailCoroutine(Transform platform, Transform[] destination)
    {
        emitterEmitter.Play();
        yield return new WaitForSeconds(2f);
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.elevbell, platform.position);
        platformEmitter.Play();
        for (int i = 0; i < destination.Length; ++i)
        {
            while(true)
            {
                if (Vector3.Distance(platform.position, destination[i].position) < 0.02f)
                {
                    platform.position = destination[i].position;
                    break;
                }

                platform.position = Vector3.MoveTowards(platform.position, destination[i].position, speed * Time.deltaTime);

                yield return null;
            }
        }

        platformEmitter.Stop();
        emitterEmitter.Stop();
        yield return null;
    }
}
