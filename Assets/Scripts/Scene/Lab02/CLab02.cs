using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CLab02 : CComponent
{
    [Header("Setting")]
    [SerializeField] private GameObject portalGunCrosshair;

    [Header("Event")]
    [SerializeField] private UnityEvent entryDoorEvent;

    private EventInstance musicPortalTasteOfBloodInstance;

    private bool isPart1Entry = false;
    private bool isPart1GetPortalGun = false;
    private bool isPart2Entry = false;
    private bool isPart2Success = false;

    public override void Start()
    {
        base.Start();

        musicPortalTasteOfBloodInstance = CAudioManager.Instance.CreateEventInstance(CFMODEventsLab02.Instance.portalTasteOfBlood);
    }

    public void PlayPart1Entry()
    {
        if(!isPart1Entry)
        {
            isPart1Entry = true;
            StartCoroutine(Part1EntryCoroutine());
        }
    }
    
    public void PlayPart1GetPortalGun()
    {
        if(!isPart1GetPortalGun)
        {
            isPart1GetPortalGun = true;
            StartCoroutine(Part1GetPortalGunCoroutine());
        }
    }

    public void PlayPart2Entry()
    {
        if(!isPart2Entry) 
        {
            isPart2Entry = true;
            StartCoroutine(Part2EntryCoroutine());
        }
    }

    public void PlayPart2Success()
    {
        if(!isPart2Success)
        {
            isPart2Success = true;
            StartCoroutine(Part2SuccessCoroutine());
        }
    }
    public void Part2EntryMusic()
    {
        musicPortalTasteOfBloodInstance.start();
    }

    private IEnumerator Part1EntryCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1Entry1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab02_part1_entry_1"),
                                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1Entry1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1Entry1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1Entry2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab02_part1_entry_2"),
                                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1Entry2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1Entry2));

        entryDoorEvent.Invoke();

        yield return null;
    }

    private IEnumerator Part1GetPortalGunCoroutine()
    {
        portalGunCrosshair.SetActive(true);

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_1"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_2"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun2));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_3"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun3));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun3));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun4, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_4"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun4));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun4));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun5, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_5"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun5));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun5));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun6, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_6"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun6));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun6));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun7, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_7"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun7));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun7));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part1GetPortalGun8, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab00_part1_getportalgunB_8"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun8));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part1GetPortalGun8));
    }

    private IEnumerator Part2EntryCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part2Entry, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab02_part2_entry_1"),
                                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part2Entry));
        yield return null; 
    }

    private IEnumerator Part2SuccessCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab02.Instance.part2Success, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab02", "lab02_part2_success_1"),
                                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab02.Instance.part2Success));
        yield return null;
    }

}
