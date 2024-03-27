using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CSettingWindow : CComponent
{
    [SerializeField] private List<CSettingWindowTabButton> tabButtons;

    [SerializeField] private CVideoSetting videoSetting;
    [SerializeField] private CAudioSettingGroup audioSetting;

    public override void Awake()
    {
        base.Awake();

        if(videoSetting == null)
            videoSetting = GetComponentInChildren<CVideoSetting>();
        if(audioSetting == null)
            audioSetting = GetComponentInChildren<CAudioSettingGroup>();
    }

    public override void Start()
    {
        base.Start();

        ResetButton();
        tabButtons[0].ButtonSelected();
    }

    public void OpenWindow()
    {
        this.gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        videoSetting.CancelChange();
        audioSetting.CancelChanges();
        this.gameObject.SetActive(false);
    }

    public void ApplyClose()
    {
        videoSetting.ApplyChange();
        audioSetting.ApplyChanges();
        this.gameObject.SetActive(false);
    }

    public void ApplyChange()
    {
        videoSetting.ApplyChange();
        audioSetting.ApplyChanges();
    }

    public void OnTabSelected(CSettingWindowTabButton button)
    {
        ResetButton();
        button.ButtonSelected();
    }

    private void ResetButton()
    {
        foreach(var button in tabButtons)
        {
            button.ButtonReset();
        }
    }
}
