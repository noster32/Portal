using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFMODEventsLab16 : CSingleton<CFMODEventsLab16>
{
    [field: Header("Aperture_Ai VO")]
    [field: SerializeField] public EventReference part1Entry1 { get; private set; }
    [field: SerializeField] public EventReference part1Entry2 { get; private set; }
    [field: SerializeField] public EventReference part1Entry3 { get; private set; }
    [field: SerializeField] public EventReference part1Success { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
            m_oInstance = this;

    }
}
