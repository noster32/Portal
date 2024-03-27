using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CLamp : CComponent
{
    [Header("Particle")]
    [SerializeField] private CSparkParticle particle;
    [SerializeField] private Transform sparkTransform;

    public void PlayLampSpark()
    {
        particle.PlayParticle(2, sparkTransform);
        CAudioManager.Instance.PlayOneShot(CFMODEventsEnergy.Instance.spark6, this.transform.position);
    }
}
