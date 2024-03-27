using System.Collections;
using UnityEngine;

public class CLab11 : CComponent
{
    [Header("Ambient Sound")]
    [SerializeField] private CAmbientSoundWithClear part2ClearAmbientSound;

    private bool isPart1Entry = false;
    private bool isPart1GetDeviceComponent = false;
    private bool isPart2Entry = false;
    private bool isPart2Success = false;
    private bool isPart2ButtonPress = false;

    public void PlayPart1Entry()
    {
        if (!isPart1Entry)
        {
            isPart1Entry = true;
            StartCoroutine(Part1EntryCoroutine());
        }
    }

    public void PlayPart1GetDeviceComponent()
    {
        if (!isPart1GetDeviceComponent)
        {
            isPart1GetDeviceComponent = true;
            StartCoroutine(Part1GetDeviceComponentCoroutine());
        }
    }

    public void PlayPart2Entry()
    {
        if (!isPart2Entry)
        {
            isPart2Entry = true;
            StartCoroutine(Part2EntryCoroutine());
        }
    }

    public void PlayPart2Success()
    {
        Debug.Log(isPart2ButtonPress);
        if (!isPart2Success && isPart2ButtonPress)
        {
            isPart2Success = true;
            part2ClearAmbientSound.PartClear();
            CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part2Success, this.transform.position);
            CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part2_success_1"),
                                                   CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part2Success));
        }
    }

    public void PlayMusicSubjectNameHere()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.portalSubjectNameHere, this.transform.position);
    }

    public void Part2ButtonPress()
    {
        isPart2ButtonPress = true;
    }

    public void Part2ButtonRelease()
    {
        isPart2ButtonPress = false;
    }

    public IEnumerator Part1EntryCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part1Entry1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part1_entry_1"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1Entry1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1Entry1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part1Entry2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part1_entry_2"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1Entry2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1Entry2));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part1Entry3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part1_entry_3"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1Entry3));
        yield return null;
    }
    public IEnumerator Part1GetDeviceComponentCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part1GetDeviceComponent1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part1_getdevicecomponent_1"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1GetDeviceComponent1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1GetDeviceComponent1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part1GetDeviceComponent2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part1_getdevicecomponent_2"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1GetDeviceComponent2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1GetDeviceComponent2));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part1GetDeviceComponent3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part1_getdevicecomponent_3"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part1GetDeviceComponent3));
        yield return null;
    }

    public IEnumerator Part2EntryCoroutine()
    {
        yield return new WaitForSeconds(5f);

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab11.Instance.part2Entry, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab11", "lab11_part2_entry_1"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab11.Instance.part2Entry));
        yield return null;
    }
}
