using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletParticle : CComponent
{
    [SerializeField] private ParticleSystem concretParticle;
    [SerializeField] private ParticleSystem metalParticle;

    private static CBulletParticle instance;

    public override void Awake()
    {
        base.Awake();

        if (!instance)
            instance = this;
        else
            Debug.LogError("Already have ParticleSystem");

    }
    public static CBulletParticle GetInstance()
    {
        return instance;
    }

    public ParticleSystem GetConcretParticle()
    {
        return concretParticle;
    }

    public ParticleSystem GetMetalParticle()
    {
        return metalParticle;
    }
}
