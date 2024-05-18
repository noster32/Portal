using FMODUnity;
using UnityEngine;

public class CFMODEventsLab02 : CSingleton<CFMODEventsLab02>
{
    [field: Header("Aperture_Ai VO")]
    [field: SerializeField] public EventReference part1Entry1 { get; private set; }
    [field: SerializeField] public EventReference part1Entry2 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun1 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun2 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun3 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun4 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun5 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun6 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun7 { get; private set; }
    [field: SerializeField] public EventReference part1GetPortalGun8 { get; private set; }
    [field: SerializeField] public EventReference part2Entry { get; private set; }
    [field: SerializeField] public EventReference part2Success { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference portalTasteOfBlood { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
            m_oInstance = this;

    }
}
