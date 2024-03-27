using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFMODEventsEnergy : CSingleton<CFMODEventsEnergy>
{
    [field: Header("Energy SFX")]
    [field: SerializeField] public EventReference energyBounce1 { get; private set; }
    [field: SerializeField] public EventReference energyBounce2 { get; private set; }
    [field: SerializeField] public EventReference energySingExplosion2 { get; private set; }
    [field: SerializeField] public EventReference energySingLoop4 { get; private set; }
    [field: SerializeField] public EventReference spark6 { get; private set; }
    [field: SerializeField] public EventReference weld2 { get; private set; }
    public override void Awake()
    {
        base.Awake();

        if (m_oInstance != null)
        {
            Debug.Log("There can only be one FMOD Events");
        }
        else
            m_oInstance = this;
    }
}
