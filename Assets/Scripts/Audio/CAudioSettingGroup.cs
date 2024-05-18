using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAudioSettingGroup : CComponent
{
    [SerializeField] private List<CAudioSetting> audioSettings;

    public void ApplyChanges()
    {
        for(int i = 0; i < audioSettings.Count; ++i)
        {
            audioSettings[i].ApplyChange();
        }
    }

    public void CancelChanges()
    {
        for (int i = 0; i < audioSettings.Count; ++i)
        {
            audioSettings[i].CancelChange();
        }
    }
}
