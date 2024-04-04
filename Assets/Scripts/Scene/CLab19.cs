using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class CLab19 : CComponent
{
    private bool isPart1Entry = false;
    private bool isPart1IntoTheFire = false;
    private bool isPart1FireDamage = false;
    private bool isPart1Die = false;

    [SerializeField] private EventReference lab00StartVoice;

    private EventInstance portalProceduralJiggleBoneInstance;
    private EventInstance portal4000DegreesKelvinInstance;
    private EventInstance lab00StartVoiceInstance;

    private Coroutine fireDamageEffectCoroutine;



    private CPlayerState playerState;
    private CPlayerDie playerDie;
    [SerializeField] private CCameraFade cameraFade;
    [SerializeField] private CClaw claw;
    [SerializeField] private GameObject dropPortalgun;
    [SerializeField] private CAmbientSound fireAmbient;
    [SerializeField] private CRadio radioAmbient;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject portalCrosshair;


    public override void Start()
    {
        base.Start();

        portalProceduralJiggleBoneInstance = CAudioManager.Instance.CreateEventInstance(CFMODEventsLab19.Instance.portalProceduralJiggleBone);
        portal4000DegreesKelvinInstance = CAudioManager.Instance.CreateEventInstance(CFMODEventsLab19.Instance.portal4000DegreesKelvin);
        lab00StartVoiceInstance = CAudioManager.Instance.CreateEventInstance(lab00StartVoice);

        playerState = CSceneManager.Instance.player.transform.GetComponent<CPlayerState>();
        playerDie = CSceneManager.Instance.player.transform.GetComponent<CPlayerDie>();
    }

    public void PlayPart1Entry()
    {
        if (!isPart1Entry)
        {
            isPart1Entry = true;
            StartCoroutine(Part1EntryCoroutine());
        }
    }

    public void PlayPart1IntoTheFire()
    {
        if (!isPart1IntoTheFire)
        {
            isPart1IntoTheFire = true;
            StartCoroutine(Part1IntoTheFireCoroutine());
        }
    }

    public void PlayLab19StartMusic()
    {
        portalProceduralJiggleBoneInstance.start();
    }

    private IEnumerator Part1EntryCoroutine()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1Entry1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_entry_1"),
                                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1Entry1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1Entry1));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1Entry2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_entry_2"),
                                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1Entry2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1Entry2));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1Entry3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_entry_3"),
                                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1Entry3));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1Entry3));
    }

    private IEnumerator Part1IntoTheFireCoroutine()
    {
        if (CAudioManager.Instance.GetIsPlaying(portalProceduralJiggleBoneInstance))
            portalProceduralJiggleBoneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1IntoTheFire1, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_intothefire_1"),
                                CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire1));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire1));

        portal4000DegreesKelvinInstance.start();
        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1IntoTheFire2, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_intothefire_2"),
                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire2));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire2));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1IntoTheFire3, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_intothefire_3"),
                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire3));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire3));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1IntoTheFire4, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_intothefire_4"),
                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire4));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire4));

        CAudioManager.Instance.PlayOneShot(CFMODEventsLab19.Instance.part1IntoTheFire5, this.transform.position);
        CSceneManager.Instance.subtitle.SetText(CScriptManager.Instance.GetText("lab19", "lab19_part1_intothefire_5"),
                        CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire5));
        yield return new WaitForSeconds(CAudioManager.Instance.GetAudioLength(CFMODEventsLab19.Instance.part1IntoTheFire5));
    }

    public void PlayPart1FireDamage()
    {
        if (!isPart1FireDamage)
        {
            isPart1FireDamage = true;
            fireDamageEffectCoroutine = StartCoroutine(FireDamageEffectCoroutine());
        }
    }

    public void PlayPart1Die()
    {
        if (!isPart1Die)
        {
            isPart1Die = true;
            StopCoroutine(fireDamageEffectCoroutine);
            StartCoroutine(PlayerDieCoroutine());
        }
    }

    private IEnumerator FireDamageEffectCoroutine()
    {
        while(true)
        {
            playerState.AimPunchPlayer();

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator PlayerDieCoroutine()
    {
        while (!playerState.GetIsEnding())
        {
            playerState.DealEndingDamageToPlayer();

            yield return new WaitForSeconds(0.5f);
        }

        cameraFade.SetAlpha(1f);
        CSceneManager.Instance.portalPair.CleanBothPortal();
        playerDie.PlayerSpawnDeadCamera2();
        crosshair.SetActive(false);
        portalCrosshair.SetActive(false);
        dropPortalgun.SetActive(true);
        fireAmbient.ambientStop();
        radioAmbient.RadioStop();

        yield return new WaitForSeconds(8f);

        fireAmbient.ambientPlay();
        radioAmbient.RadioPlay();
        cameraFade.StartFadeIn(3f, false);

        yield return new WaitForSeconds(3f);
        claw.ActiveClaw();

        yield return new WaitForSeconds(20f);

        lab00StartVoiceInstance.setVolume(0.3f);
        lab00StartVoiceInstance.start();

        yield return new WaitForSeconds(3f);
        cameraFade.SetAlpha(1f);

        yield return new WaitForSeconds(5f);

        CSceneLoader.Instance.LoadSceneAsync(0, (a_oAsyncOperation) =>
        {
            Debug.LogFormat("Percent : {0}", a_oAsyncOperation.progress);
        });
    }
}
