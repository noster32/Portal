using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAudioSettingGroup : CComponent
{
    [SerializeField] private List<CAudioSetting> audioSettings;

    public void ApplyChanges()
    {
        foreach(var setting in audioSettings)
        {
            setting.ApplyChange();
        }
    }

    public void CancelChanges()
    {
        foreach (var setting in audioSettings)
        {
            setting.CancelChange();
        }
    }
}
