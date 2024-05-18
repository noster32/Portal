using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFMODEventsLab19 : CSingleton<CFMODEventsLab19>
{
    [field: Header("Aperture_Ai VO")]
    [field: SerializeField] public EventReference part1Entry1 { get; private set; }
    [field: SerializeField] public EventReference part1Entry2 { get; private set; }
    [field: SerializeField] public EventReference part1Entry3 { get; private set; }

    [field: SerializeField] public EventReference part1IntoTheFire1 { get; private set; }
    [field: SerializeField] public EventReference part1IntoTheFire2 { get; private set; }
    [field: SerializeField] public EventReference part1IntoTheFire3 { get; private set; }
    [field: SerializeField] public EventReference part1IntoTheFire4 { get; private set; }
    [field: SerializeField] public EventReference part1IntoTheFire5 { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference portalProceduralJiggleBone { get; private set; }
    [field: SerializeField] public EventReference portal4000DegreesKelvin { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
            m_oInstance = this;

    }
}
