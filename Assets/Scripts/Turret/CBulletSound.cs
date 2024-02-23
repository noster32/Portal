using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletSound : CComponent
{
    #region Sound
    [Header("Hit Sound")]
    [SerializeField] private AudioClip[] concretHitSoundClip;
    [SerializeField] private AudioClip[] metalHitSoundClip;
    [SerializeField] private AudioClip[] glassHitSoundClip;
    #endregion

    [SerializeField] private AudioSource audioSource;

    [HideInInspector] public Action<CBulletSound> bulletSoundDestroy;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator PlayHitConcretSound()
    {
        SoundUtility.PlayRandomSound(audioSource, concretHitSoundClip);
        yield return new WaitForSeconds(1f);

        bulletSoundDestroy(this);
        yield return null;
    }

    public IEnumerator PlayHitMetalSound()
    {
        SoundUtility.PlayRandomSound(audioSource, metalHitSoundClip);
        yield return new WaitForSeconds(1f);

        bulletSoundDestroy(this);
        yield return null;
    }

    public IEnumerator PlayHitGlassSound()
    {
        SoundUtility.PlayRandomSound(audioSource, glassHitSoundClip);
        yield return new WaitForSeconds(1f);

        bulletSoundDestroy(this);
        yield return null;
    }
        

    public void DestroyBulletSound(Action<CBulletSound> destroyAction)
    {
        bulletSoundDestroy = destroyAction;
    }
}
