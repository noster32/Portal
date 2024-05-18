using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBedCover : CComponent
{
    [Header("Animator")]
    [SerializeField] Animator bedAnimator;

    [Header("Particle")]
    [SerializeField] private ParticleSystem sparkParticle;
    [SerializeField] private Transform sparkTransform;

    private Coroutine animCoroutine;

    public override void Start()
    {
        base.Start();

        bedAnimator.Play("closing", 0, 1f);
    }

    public void PlayBedCoverOpen()
    {
        if(animCoroutine == null)
            animCoroutine = StartCoroutine(BedCoverOpen());
    }

    public void PlayBedCoverClose()
    {
        if(animCoroutine == null)
            animCoroutine = StartCoroutine(BedCoverClose());
    }

    public void PlayBedSpark()
    {
        CParticleManager.Instance.PlayParticle(sparkParticle, sparkTransform.position, sparkTransform.rotation);
        CAudioManager.Instance.PlayOneShot(CFMODEventsEnergy.Instance.spark6, this.transform.position);
    }

    private IEnumerator BedCoverOpen()
    {
        bedAnimator.Play("opening");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door3, this.transform.position);
        yield return new WaitForSeconds(2f);

        animCoroutine = null;
        yield return null;
    }

    private IEnumerator BedCoverClose()
    {
        bedAnimator.Play("closing");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door3, this.transform.position);
        yield return new WaitForSeconds(2f);

        animCoroutine = null;
        yield return null;
    }


}
