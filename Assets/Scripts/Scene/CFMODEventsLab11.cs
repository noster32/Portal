using FMODUnity;
using UnityEngine;

public class CFMODEventsLab11 : CSingleton<CFMODEventsLab11>
{
    [field: Header("Aperture_Ai VO")]
    [field: SerializeField] public EventReference part1Entry1 { get; private set; }
    [field: SerializeField] public EventReference part1Entry2 { get; private set; }
    [field: SerializeField] public EventReference part1Entry3 { get; private set; }
    [field: SerializeField] public EventReference part1GetDeviceComponent1 { get; private set; }
    [field: SerializeField] public EventReference part1GetDeviceComponent2 { get; private set; }
    [field: SerializeField] public EventReference part1GetDeviceComponent3 { get; private set; }
    [field: SerializeField] public EventReference part2Entry { get; private set; }
    [field: SerializeField] public EventReference part2Success { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference portalSubjectNameHere { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance != null)
        {
            Debug.Log("There can only be one Lab11 FMOD Events");
        }
        else
            m_oInstance = this;
    }
}
