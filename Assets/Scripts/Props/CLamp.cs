using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CLamp : CComponent
{
    [Header("Audio")]
    [SerializeField] private AudioClip sparkSound;

    [Header("Particle")]
    [SerializeField] private CSparkParticle particle;
    [SerializeField] private Transform sparkTransform;

    private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayLampSpark()
    {
        particle.PlayParticle(2, sparkTransform);
        audioSource.PlayOneShot(sparkSound, CSoundLoader.Instance.GetEffectVolume(0.06f));
    }
}
