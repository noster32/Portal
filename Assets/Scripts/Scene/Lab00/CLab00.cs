using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CLab00 : CComponent
{
    [Header("Setting")]
    [SerializeField] CCameraFade cameraFade;
    [SerializeField] GameObject playerCharacter;
    [SerializeField] GameObject introCamera;
    [SerializeField] GameObject crosshairHud;

    [Header("TimeLine")]
    [SerializeField] PlayableDirector playableDirector;

    [Header("Camera")]
    [SerializeField] private CPlayerCameraEffect cameraEffect;

    [Header("Event")]
    [SerializeField] UnityEvent bedOpenEvent;
    [SerializeField] UnityEvent bedCloseEvent;
    [SerializeField] UnityEvent timerStartEvent;
    [SerializeField] UnityEvent sparkEvent;
    [SerializeField] UnityEvent signageEvent;
    [SerializeField] UnityEvent autoPortalEvent;
    [SerializeField] UnityEvent autoPortalEventPart2;
    [SerializeField] UnityEvent[] autoPortalEventPart2B;
    [SerializeField] UnityEvent autoPortalClearEvent;

    [Header("Ambient Sound")]
    [SerializeField] private CAmbientSoundWithClear part1RoomAmbientSound;
    [SerializeField] private CAmbientSoundWithClear part2RoomAmbientSound;
    [SerializeField] private CAmbientSoundWithClear part2HallwayAmbientSound;

    private bool isPart1Success = false;
    private bool isPart2Entry = false;
    private bool isPart2Success = false;

    private bool portalPlaceLoop = true;
    private int count;

    public override void Start()
    {
        base.Start();

        StartCoroutine(SceneIntroCoroutine());
    }

    public void PlayPart1Success()
    {
        if (!isPart1Success)
        {
            isPart1Success = true;
            StartCoroutine(Part1SuccessCoroutine());
        }
    }

    public void PlayPart2Entry()
    {
        if (!isPart2Entry)
        {
            isPart2Entry = true;
            autoPortalEventPart2.Invoke();
            StartCoroutine(Part2AutoPortal());
            CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part2Entry, this.transform.position);
            CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part2_entry_1"),
                CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part2Entry));
        }
    }

    public void PlayPart2Succes()
    {
        if(!isPart2Success)
        {
            isPart2Success = true;
            part2RoomAmbientSound.PartClear();
            part2HallwayAmbientSound.PartClear();

            CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part2Success, this.transform.position);
            CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part2_success_1"),
                CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part2Success));
        }
    }

    public void PlayPart2PortalLoopStop()
    {
        portalPlaceLoop = false;
        autoPortalClearEvent.Invoke();
    }

    private IEnumerator SceneIntroCoroutine()
    {
        yield return new WaitForSeconds(3f);

        cameraFade.StartFadeIn(1.5f);

        yield return new WaitForSeconds(2f);

        bedOpenEvent.Invoke();

        yield return new WaitForSeconds(1f);

        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        timerStartEvent.Invoke();

        yield return new WaitForSeconds(2.5f);

        introCamera.SetActive(false);
        playerCharacter.SetActive(true);
        crosshairHud.SetActive(true);

        yield return new WaitForSeconds(2f);

        bedCloseEvent.Invoke();

        yield return new WaitForSeconds(15.5f);

        StartCoroutine(AIvoiceCoroutine());

        yield return null;
    }

    private IEnumerator AIvoiceCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Entry1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_entry_1"), 
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry1));

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Entry2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_entry_2"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry2));

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Entry3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_entry_3"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry3));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry3));

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Entry4, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_entry_4"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry4));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry4));

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Entry5, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_entry_5"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry5));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry5));

        StartCoroutine(SparkCoroutine());

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Entry6, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_entry_6"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry6));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry6));

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Entry7, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_entry_7"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry7));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Entry7));

        autoPortalEvent.Invoke();

        yield return null;
    }

    private IEnumerator SparkCoroutine()
    {
        sparkEvent.Invoke();
        cameraFade.StartFlicking(0.66f);

        yield return new WaitForSeconds(0.183f);

        sparkEvent.Invoke();

        yield return new WaitForSeconds(0.283f);

        sparkEvent.Invoke();

        yield return new WaitForSeconds(0.5f);

        signageEvent.Invoke();
        cameraFade.StartFlicking(2.5f, true);

        yield return null;
    }

    private IEnumerator Part1SuccessCoroutine()
    {
        yield return new WaitForSeconds(2f);

        part1RoomAmbientSound.PartClear();
        
        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Success1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_success_1"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Success1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Success1));

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Success2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_success_2"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Success2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Success2));

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.part1Success3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab00", "lab00_part1_success_3"),
            CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Success3));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventLab00.Instance.part1Success3));
    }

    private IEnumerator Part2AutoPortal()
    {
        yield return new WaitForSeconds(5f);

        count = 0;
        while (portalPlaceLoop)
        {
            count++;

            if (count == 3)
                count = 0;

            autoPortalEventPart2B[count].Invoke();
            cameraEffect.PlayCameraShake(0.5f, 0.1f);
            
            yield return new WaitForSeconds(5f);
        }
    }
}
