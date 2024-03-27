using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFMODEventsMenu : CSingleton<CFMODEventsMenu>
{
    [field: Header("UI")]
    [field: SerializeField] public EventReference buttonClickRelease { get; private set; }
    [field: SerializeField] public EventReference buttonRollOver { get; private set; }
    

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance != null)
        {
            Debug.Log("There can only be one FMOD Events Menu");
            m_oInstance = null;
        }
        else
        {
            m_oInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
