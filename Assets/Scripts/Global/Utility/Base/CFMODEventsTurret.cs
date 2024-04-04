using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFMODEventsTurret : CSingleton<CFMODEventsTurret>
{
    [field: Header("Turret Voice SFX")]
    [field: SerializeField] public EventReference turretActive { get; private set; }
    [field: SerializeField] public EventReference turretDisabled { get; private set; }
    [field: SerializeField] public EventReference turretFizzler { get; private set; }
    [field: SerializeField] public EventReference turretPickup { get; private set; }
    [field: SerializeField] public EventReference turretRetire { get; private set; }
    [field: SerializeField] public EventReference turretSearch { get; private set; }
    [field: SerializeField] public EventReference turretShotat { get; private set; }
    [field: SerializeField] public EventReference turretTipped { get; private set; }

    [field: Header("Turret SFX")]
    [field: SerializeField] public EventReference active { get; private set; }
    [field: SerializeField] public EventReference deploy { get; private set; }
    [field: SerializeField] public EventReference die { get; private set; }
    [field: SerializeField] public EventReference ping { get; private set; }
    [field: SerializeField] public EventReference retract { get; private set; }
    [field: SerializeField] public EventReference shoot { get; private set; }

    [field: Header("Bullet SFX")]
    [field: SerializeField] public EventReference concreteImpactBullet { get; private set; }
    [field: SerializeField] public EventReference glassImpactBullet { get; private set; }
    [field: SerializeField] public EventReference metalImpactBullet { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
            m_oInstance = this;

    }
}
