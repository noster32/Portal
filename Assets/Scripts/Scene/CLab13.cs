using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CLab13 : CComponent
{
    private bool isPart1Entry = false;
    private bool isPart1Success = false;
    private bool test = false;

    public void PlayPart1Entry()
    {
        if (!isPart1Entry)
        {
            isPart1Entry = true;
            StartCoroutine(Part1EntryCoroutine());
        }
    }

    public void PlayPart1Success()
    {
        if (!isPart1Success)
        {
            isPart1Success = true;
            StartCoroutine(Part1SuccessCoroutine());
        }
    }

    public void SubtitleTest()
    {
        if (!test)
        {
            test = true;
            StartCoroutine(Part1EntryCoroutine());
        }
    }

    private IEnumerator Part1EntryCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab13.Instance.part1Entry1, this.transform.position);
        var script = CScriptManager.Instance.GetText("lab13", "lab13_part1_entry_1");
        CSceneManager.Instance.subtitle.SetText(script, CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Entry1));

        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Entry1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab13.Instance.part1Entry2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab13", "lab13_part1_entry_2"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Entry2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Entry2));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab13.Instance.part1Entry3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab13", "lab13_part1_entry_3"), 
                                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Entry3));

        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Entry3));

        test = false;
    }

    private IEnumerator Part1SuccessCoroutine()
    {
        yield return new WaitForSeconds(2f);

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab13.Instance.part1Success1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab13", "lab13_part1_success_1"),
                                            CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Success1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Success1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab13.Instance.part1Success2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab13", "lab13_part1_success_2"),
                                               CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Success2));

        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab13.Instance.part1Success2));
    }
}
