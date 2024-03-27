using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CAudioManager : CSingleton<CAudioManager>
{
    [Header("Volume")]
    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float ambientVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambientBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    public override void Awake()
    {
        base.Awake();

        if(m_oInstance != null)
        {
            Debug.Log("There can only be one Audio Manager");
            m_oInstance = null;
        }
        else
        {
            m_oInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/music");
        ambientBus = RuntimeManager.GetBus("bus:/ambient");
        sfxBus = RuntimeManager.GetBus("bus:/sfx");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CleanUp();
    }

    public override void Update()
    {
        base.Update();

        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambientBus.setVolume(ambientVolume);
        sfxBus.setVolume(sfxVolume);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //use 2d action or timeline worldPos => Vector3.zero
    public void PlayOneShot(EventReference sound, Vector3 worldPos = default(Vector3)) 
        => RuntimeManager.PlayOneShot(sound, worldPos);

    public void PlayOneShot(EventReference sound, float volume, Vector3 worldPos) 
        => RuntimeManager.PlayOneShot(sound, volume, worldPos);


    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);

        return emitter;
    }

    //Timeline만 가능 Action은 불가능
    public float GetAudioLength(EventReference eventReference)
    {
        EventDescription eventDescription = RuntimeManager.GetEventDescription(eventReference.Guid);
        int audioLength = 0;
        eventDescription.getLength(out audioLength);
        return audioLength / 1000f;
    }

    public bool GetIsPlaying(EventInstance eventInstance)
    {
        PLAYBACK_STATE state;
        eventInstance.getPlaybackState(out state);
        return state != PLAYBACK_STATE.STOPPED;
    }

    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach(StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }
 
    private void OnDestroy()
    {
        CleanUp();
    }
}
