using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CLab00 : CComponent
{
    [SerializeField] CCameraFade cameraFade;
    [SerializeField] GameObject playerCharacter;
    [SerializeField] GameObject introCamera;

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

    [Header("Aperture Sound")]
    [SerializeField] private AudioClip blipSound;
    [SerializeField] private AudioClip[] part1AISoundEntry;
    [SerializeField] private AudioClip[] part1AISoundSuccess;
    [SerializeField] private AudioClip part2AISoundEntry;
    [SerializeField] private AudioClip part2AISoundSuccess;

    [Header("Ambient Sound")]
    [SerializeField] private CAmbienceSound ambienceSound;
    [SerializeField] private CAmbienceSound part2RoomAmbienceSound;
    [SerializeField] private CAmbienceSound part2HallwayAmbienceSound;
    [SerializeField] private AudioClip changeAmbient;
    [SerializeField] private AudioClip changeAmbientPart2Success;
    [SerializeField] private AudioClip changeAmbientPart2HallwaySuccess;

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
            StartCoroutine(Part2EntryCoroutine());
        }
    }

    public void PlayPart2Succes()
    {
        if(!isPart2Success)
        {
            isPart1Success = true;
            StartCoroutine(Part2SuccessCoroutine());
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

        yield return new WaitForSeconds(2f);

        bedCloseEvent.Invoke();

        yield return new WaitForSeconds(15.5f);

        StartCoroutine(AIvoiceCoroutine());

        yield return null;
    }

    private IEnumerator AIvoiceCoroutine()
    {
        CSoundLoader.Instance.PlaySoundOneShot(blipSound);

        yield return new WaitForSeconds(blipSound.length);

        for (int i = 0; i < 5; i++)
        {
            CSoundLoader.Instance.PlaySoundOneShot(part1AISoundEntry[i]);

            yield return new WaitForSeconds(part1AISoundEntry[i].length);
        }

        StartCoroutine(SparkCoroutine());
        CSoundLoader.Instance.PlaySoundOneShot(part1AISoundEntry[5]);

        yield return new WaitForSeconds(part1AISoundEntry[5].length);

        CSoundLoader.Instance.PlaySoundOneShot(part1AISoundEntry[6]);

        yield return new WaitForSeconds(part1AISoundEntry[6].length);

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

        CSoundLoader.Instance.PlaySoundOneShot(blipSound);
        ambienceSound.ChangeAmbient(changeAmbient);

        yield return new WaitForSeconds(blipSound.length);

        foreach (AudioClip aiClip in part1AISoundSuccess)
        {
            CSoundLoader.Instance.PlaySoundOneShot(aiClip);

            yield return new WaitForSeconds(aiClip.length);
        }

        CSoundLoader.Instance.PlaySoundOneShot(blipSound);

        yield return null;
    }

    private IEnumerator Part2EntryCoroutine()
    {
        CSoundLoader.Instance.PlaySoundOneShot(blipSound);
        autoPortalEventPart2.Invoke();
        StartCoroutine(Part2AutoPortal());

        yield return new WaitForSeconds(blipSound.length);

        CSoundLoader.Instance.PlaySoundOneShot(part2AISoundEntry);

        yield return new WaitForSeconds(part2AISoundEntry.length);

        CSoundLoader.Instance.PlaySoundOneShot(blipSound);

        yield return null;

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

    private IEnumerator Part2SuccessCoroutine()
    {
        CSoundLoader.Instance.PlaySoundOneShot(blipSound);
        part2RoomAmbienceSound.ChangeAmbient(changeAmbientPart2Success);
        part2HallwayAmbienceSound.ChangeAmbient(changeAmbientPart2HallwaySuccess);

        yield return new WaitForSeconds(blipSound.length);

        CSoundLoader.Instance.PlaySoundOneShot(part2AISoundSuccess);

        yield return new WaitForSeconds(part2AISoundSuccess.length);

        CSoundLoader.Instance.PlaySoundOneShot(blipSound);
    }
}
