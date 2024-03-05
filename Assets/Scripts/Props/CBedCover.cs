using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBedCover : CComponent
{
    [Header("Animator")]
    [SerializeField] Animator bedAnimator;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSoundClip;
    [SerializeField] private AudioClip sparkSound;

    [Header("Particle")]
    [SerializeField] private CSparkParticle particle;
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
        particle.PlayParticle(1, sparkTransform);
        audioSource.PlayOneShot(sparkSound, CSoundLoader.Instance.GetEffectVolume(0.06f));
    }

    private IEnumerator BedCoverOpen()
    {
        bedAnimator.Play("opening");
        audioSource.PlayOneShot(openSoundClip, CSoundLoader.Instance.GetEffectVolume(0.15f));
        yield return new WaitForSeconds(2f);

        animCoroutine = null;
        yield return null;
    }

    private IEnumerator BedCoverClose()
    {
        bedAnimator.Play("closing");
        audioSource.PlayOneShot(openSoundClip, CSoundLoader.Instance.GetEffectVolume(0.15f));
        yield return new WaitForSeconds(2f);

        animCoroutine = null;
        yield return null;
    }


}
