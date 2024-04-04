using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAudioSetting : CComponent
{
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        AMBIENT,
        SFX
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;

    public override void Awake()
    {
        base.Awake(); 
        volumeSlider = GetComponentInChildren<Slider>();
    }

    public override void Start()
    {
        base.Start();

        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = CAudioManager.Instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = CAudioManager.Instance.musicVolume;
                break;
            case VolumeType.AMBIENT:
                volumeSlider.value = CAudioManager.Instance.ambientVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = CAudioManager.Instance.sfxVolume;
                break;
            default:
                break;
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public void OnSliderValueChanged()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                CAudioManager.Instance.masterVolume = volumeSlider.value;
                break;
            case VolumeType.MUSIC:
                CAudioManager.Instance.musicVolume = volumeSlider.value;
                break;
            case VolumeType.AMBIENT:
                CAudioManager.Instance.ambientVolume = volumeSlider.value;
                break;
            case VolumeType.SFX:
                CAudioManager.Instance.sfxVolume = volumeSlider.value;
                break;
            default:
                break;
        }
    }

    public void ApplyChange()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                CAudioManager.Instance.masterVolume = volumeSlider.value;
                break;
            case VolumeType.MUSIC:
                CAudioManager.Instance.musicVolume = volumeSlider.value;
                break;
            case VolumeType.AMBIENT:
                CAudioManager.Instance.ambientVolume = volumeSlider.value;
                break;
            case VolumeType.SFX:
                CAudioManager.Instance.sfxVolume = volumeSlider.value;
                break;
            default:
                break;
        }
    }

    public void CancelChange()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = CAudioManager.Instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = CAudioManager.Instance.musicVolume;
                break;
            case VolumeType.AMBIENT:
                volumeSlider.value = CAudioManager.Instance.ambientVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = CAudioManager.Instance.sfxVolume;
                break;
            default:
                break;
        }
    }
}

