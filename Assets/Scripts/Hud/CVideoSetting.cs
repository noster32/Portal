using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CVideoSetting : CComponent
{
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private TMP_Dropdown displayModeDropDown;

    private Resolution[] resolutions;

    private Resolution changeResolution;
    private int changeScreenMode;

    private enum ScreenState
    {
        FULLSCREEN = 0,
        FULLSCREENWINDOWED = 1,
        WINDOWED = 2
    }

    public override void Awake()
    {
        base.Awake();

        //Resolution
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        int currentResolutionIndex = 0;

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; ++i)
        {
            string option = string.Empty;

            option = resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRate + "Hz";

            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();

        changeResolution = Screen.currentResolution;
        //Display Mode
        displayModeDropDown.ClearOptions();

        List<string> modes = new List<string>();
        int currentDisplayModeIndex = 0;
        modes.Add("전체 화면");
        modes.Add("전체 창 모드");
        modes.Add("창 모드");

        currentDisplayModeIndex = (int)displayModeToDropDown(Screen.fullScreenMode);
        displayModeDropDown.AddOptions(modes);
        displayModeDropDown.value = currentDisplayModeIndex;
        displayModeDropDown.RefreshShownValue();

        changeScreenMode = (int)displayModeToDropDown(Screen.fullScreenMode);
    }

    public void SetResolution(int resoltionIndex)
    {
        changeResolution = resolutions[resoltionIndex];
        Debug.Log(changeResolution.width + " x " + changeResolution.height);
    }

    public void SetScreenMode(int modeIndex)
    {
        changeScreenMode = modeIndex;
        Debug.Log(changeScreenMode);
    }

    private ScreenState displayModeToDropDown(FullScreenMode screenMode)
    {
        return screenMode switch
        {
            FullScreenMode.ExclusiveFullScreen => ScreenState.FULLSCREEN,
            FullScreenMode.FullScreenWindow => ScreenState.FULLSCREENWINDOWED,
            FullScreenMode.Windowed => ScreenState.WINDOWED,
            _ => ScreenState.FULLSCREEN
        };
    }
    private FullScreenMode displayModeToDropDown(ScreenState state)
    {
        return state switch
        {
            ScreenState.FULLSCREEN => FullScreenMode.ExclusiveFullScreen,
            ScreenState.FULLSCREENWINDOWED => FullScreenMode.FullScreenWindow,
            ScreenState.WINDOWED => FullScreenMode.Windowed,
            _ => FullScreenMode.ExclusiveFullScreen
        };
    }

    public void ApplyChange()
    {
        if ((changeResolution.width != Screen.currentResolution.width && 
            changeResolution.height != Screen.currentResolution.height))
            Screen.SetResolution(changeResolution.width, changeResolution.height, false);
        if (changeScreenMode != (int)displayModeToDropDown(Screen.fullScreenMode))
        {
            Debug.Log((ScreenState)changeScreenMode);
            Screen.fullScreenMode = displayModeToDropDown((ScreenState)changeScreenMode);
        }
    }

    public void CancelChange()
    {
        changeResolution = Screen.currentResolution;
        changeScreenMode = (int)displayModeToDropDown(Screen.fullScreenMode);
    }
}
