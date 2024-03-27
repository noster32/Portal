using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CLab16 : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private UnityEvent entryDoorEvent;

    private bool isPart1Entry = false;
    private bool isPart1Success = false;

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

    private IEnumerator Part1EntryCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab16.Instance.part1Entry1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab16", "lab16_part1_entry_1"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Entry1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Entry1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab16.Instance.part1Entry2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab16", "lab16_part1_entry_2"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Entry2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Entry2));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab16.Instance.part1Entry3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab16", "lab16_part1_entry_3"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Entry3));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Entry3));

        entryDoorEvent.Invoke();

        yield return null;
    }

    private IEnumerator Part1SuccessCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab16.Instance.part1Success, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab16", "lab16_part1_success_1"),
            CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Success));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab16.Instance.part1Success));

        yield return null;
    }
}
