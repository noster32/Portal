using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestParticle : CComponent
{
    [SerializeField] private ParticleSystem _particleSystem;
    private static CTestParticle instance;
    public override void Awake()
    {
        base.Awake();
        
        if(!instance)
            instance = this;
        else
            Debug.LogError("Already have ParticleSystem");

    }
    public static CTestParticle GetInstance()
    {
        return instance;
    }

    public ParticleSystem GetTestParticle()
    {
        return _particleSystem;
    }
}
