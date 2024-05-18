using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

// field: <- { get; private set; } 인경우 자동으로 유니티 에디터에 표시되지 않는데 field: 키워드를 추가하면 표시된다

public class CFMODEvents : CSingleton<CFMODEvents>
{
    [field: Header("Radio Ambient")]
    [field: SerializeField] public EventReference loopingRadioMix { get; private set; }

    [field: Header("Machine Ambient")]
    [field: SerializeField] public EventReference fluorescentHum { get; private set; }
    [field: SerializeField] public EventReference portalgunRotateLoop { get; private set; }
    [field: SerializeField] public EventReference ticktock1 { get; private set; }
    [field: SerializeField] public EventReference beamPlatformLoop1 { get; private set; }


    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference stepSoundConcrete { get; private set; }
    [field: SerializeField] public EventReference stepSoundMetal { get; private set; }
    [field: SerializeField] public EventReference stepSoundMetalGrate { get; private set; }
    [field: SerializeField] public EventReference teleport { get; private set; }
    [field: SerializeField] public EventReference interaction { get; private set; }
    [field: SerializeField] public EventReference interactionFail { get; private set; }
    [field: SerializeField] public EventReference bodyMediumImpactHard { get; private set; }
    [field: SerializeField] public EventReference bodyMediumImpactSoft { get; private set; }
    [field: SerializeField] public EventReference nearmiss { get; private set; }

    [field: Header("PortalGun SFX")]
    [field: SerializeField] public EventReference portalgunShootBlue { get; private set; }
    [field: SerializeField] public EventReference portalgunShootRed { get; private set; }
    [field: SerializeField] public EventReference portalGunHold { get; private set; }
    [field: SerializeField] public EventReference portalgunFizzle { get; private set; }
    [field: SerializeField] public EventReference portalOpen1 { get; private set; }
    [field: SerializeField] public EventReference portalOpen3 { get; private set; }
    [field: SerializeField] public EventReference portalClose { get; private set; }
    [field: SerializeField] public EventReference portalCharging { get; private set; }

    [field: Header("Button SFX")]
    [field: SerializeField] public EventReference button3 { get; private set; }
    [field: SerializeField] public EventReference button10 { get; private set; }
    [field: SerializeField] public EventReference elevbell { get; private set; }

    [field: Header("Door SFX")]
    [field: SerializeField] public EventReference door1 { get; private set; }
    [field: SerializeField] public EventReference door2 { get; private set; }
    [field: SerializeField] public EventReference door3 { get; private set; }
    [field: SerializeField] public EventReference lever1 { get; private set; }
    [field: SerializeField] public EventReference lever2 { get; private set; }

    [field: Header("Elevator SFX")]
    [field: SerializeField] public EventReference doorLatch1 { get; private set; }
    [field: SerializeField] public EventReference elevatorDoor { get; private set; }
    [field: SerializeField] public EventReference elevatorMove { get; private set; }
    [field: SerializeField] public EventReference garageStop1 { get; private set; }
    [field: SerializeField] public EventReference portalElevatorChime { get; private set; }

    [field: Header("Collision SFX")]
    [field: SerializeField] public EventReference metalSolidImpactHard { get; private set; }
    [field: SerializeField] public EventReference metalSolidImpactSoft { get; private set; }

    [field: Header("Wall SFX")]
    [field: SerializeField] public EventReference apcStart { get; private set; }
    [field: SerializeField] public EventReference apcShutdown { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField] public EventReference apcFirstgearLoop1 { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference buttonClickRelease { get; private set; }
    [field: SerializeField] public EventReference buttonRollOver { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
        {
            m_oInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
