using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFMODEventsMainMenu : CSingleton<CFMODEventsMainMenu>
{
    [field: Header("Ambient")]
    [field: SerializeField] public EventReference holeHit { get; private set; }
    [field: SerializeField] public EventReference fluorescentHum2D { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference portalProceduralJiggleBone { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
            m_oInstance = this;
    }
}
