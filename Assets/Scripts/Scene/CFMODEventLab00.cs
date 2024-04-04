using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFMODEventLab00 : CSingleton<CFMODEventLab00>
{
    [field: Header("Aperture_Ai VO")]
    [field: SerializeField] public EventReference part1Entry1 { get; private set; }
    [field: SerializeField] public EventReference part1Entry2 { get; private set; }
    [field: SerializeField] public EventReference part1Entry3 { get; private set; }
    [field: SerializeField] public EventReference part1Entry4 { get; private set; }
    [field: SerializeField] public EventReference part1Entry5 { get; private set; }
    [field: SerializeField] public EventReference part1Entry6 { get; private set; }
    [field: SerializeField] public EventReference part1Entry7 { get; private set; }
    [field: SerializeField] public EventReference part1Success1 { get; private set; }
    [field: SerializeField] public EventReference part1Success2 { get; private set; }
    [field: SerializeField] public EventReference part1Success3 { get; private set; }
    [field: SerializeField] public EventReference part2Entry { get; private set; }
    [field: SerializeField] public EventReference part2Success { get; private set; }

    [field: Header("Props SFX")]
    [field: SerializeField] public EventReference useToiletFlush { get; private set; }
    [field: SerializeField] public EventReference useToiletThanks { get; private set; }



    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
            m_oInstance = this;

    }
}
