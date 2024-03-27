using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CMainMenu : CComponent
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private UnityEvent signageEvent; 
    float elapsedTime = 0f;
    float duration = 15.5f;

    Quaternion startRot = Quaternion.Euler(35f, 131f, 0f);
    Quaternion endRot = Quaternion.Euler(35f, 145f, 0f);

    private EventInstance startMusicInstance;
    private EventInstance ambientEventInstance;
    private EventInstance fluorescentHumInstance;

    public override void Start()
    {
        base.Start();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        startMusicInstance = CAudioManager.Instance.CreateEventInstance(CFMODEventsMainMenu.Instance.portalProceduralJiggleBone);
        ambientEventInstance = CAudioManager.Instance.CreateEventInstance(CFMODEventsMainMenu.Instance.holeHit);
        fluorescentHumInstance = CAudioManager.Instance.CreateEventInstance(CFMODEventsMainMenu.Instance.fluorescentHum2D);
        fluorescentHumInstance.setParameterByName("signage_intensity", 1);

        ambientEventInstance.start();
        startMusicInstance.start();
        StartCoroutine(MainMenuSignageSound());
    }

    public override void Update()
    {
        base.Update();

        if (elapsedTime < duration)
        {
            mainCamera.rotation = Quaternion.Slerp(startRot, endRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            mainCamera.rotation = endRot;
            elapsedTime = 0f;

            var temp = startRot;
            startRot = endRot;
            endRot = temp;
        }
    }

    private void OnDisable()
    {
        startMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ambientEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fluorescentHumInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private IEnumerator MainMenuSignageSound()
    {
        yield return new WaitForSeconds(5f);

        signageEvent.Invoke();
        yield return new WaitForSeconds(1f);

        fluorescentHumInstance.start();
        yield return new WaitForSeconds(0.536f);

        fluorescentHumInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        yield return new WaitForSeconds(0.083f);

        fluorescentHumInstance.start();
        yield return new WaitForSeconds(6f);

        float elapsedTime = 0f;
        float volume = 1f;
        while (elapsedTime < 7f)
        {
            float t = elapsedTime / 7f;
            volume = Mathf.Lerp(1f, 0f, t);
            fluorescentHumInstance.setVolume(volume);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        fluorescentHumInstance.setVolume(0f);
        fluorescentHumInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        yield return null;
    }
}
