using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CLamp : CComponent
{
    [Header("Particle")]
    [SerializeField] private ParticleSystem sparkParticle;
    [SerializeField] private Transform sparkTransform;

    public void PlayLampSpark()
    {
        CParticleManager.Instance.PlayParticle(sparkParticle, sparkTransform.position, sparkTransform.rotation);
        CAudioManager.Instance.PlayOneShot(CFMODEventsEnergy.Instance.spark6, this.transform.position);
    }
}
