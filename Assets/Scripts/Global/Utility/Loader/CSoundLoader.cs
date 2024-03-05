using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CSoundLoader : CSingleton<CSoundLoader>
{
    [Range(0f, 1f)] public float effectSoundVolume = 1f;
    [Range(0f, 1f)] public float musicSoundVolume = 1f;

    private AudioSource m_audioSource;
    private Transform m_ListenerTransform;

    public override void Awake()
    {
        base.Awake();

        m_oInstance = this;
        m_audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();

        if (m_audioSource == null)
            m_oInstance = this;
    }

    public void SetListener()
    {
        Camera mainCamera = Camera.main;
        AudioListener cameraListener = mainCamera.GetComponent<AudioListener>();
        
        if (cameraListener)
        {
            m_ListenerTransform = cameraListener.transform;
            Debug.Log(m_ListenerTransform);
        }
        else
            Debug.LogError("AudioListener not found.");
    }

    public void SetVolme(float value)
    {
        m_audioSource.volume = value;
    }

    public float GetEffectVolume(float volume)
    {
        float result = volume / effectSoundVolume;
        return result * effectSoundVolume;
    }

    public float GetMusicVolume(float volume)
    {
        return volume * musicSoundVolume;
    }

    public void PlaySound(AudioClip audioClip, float volume)
    {
        m_audioSource.clip = audioClip;
        m_audioSource.volume = volume * effectSoundVolume;

        if (!m_audioSource.isPlaying)
        {
            m_audioSource.Play();
        }
    }

    public void PlaySoundOneShot(AudioClip audioClip, float volumeScale = 1f)
    {
        m_audioSource.PlayOneShot(audioClip, GetEffectVolume(volumeScale));
    }


    public void PlaySound3D(Vector3 mainPos, float volumeMultipiler)
    {
        //Debug.Log(m_ListenerTransform);
        if (!m_ListenerTransform)
        {
            //Debug.Log("return test");
            return;
        }

        if(!m_audioSource.isPlaying)
        {
            m_audioSource.Play();
        }

        if (m_audioSource.isPlaying)
        {
            float distance = Vector3.Distance(m_ListenerTransform.position, mainPos);

            float volume = Mathf.Clamp01((m_audioSource.maxDistance - distance) / (m_audioSource.maxDistance - m_audioSource.minDistance)) * volumeMultipiler;

            m_audioSource.volume = volume;
        }
    }

    public void PlaySoundOneShot3D(Vector3 mainPos, float volumeMultipiler)
    {
        m_audioSource.PlayOneShot(m_audioSource.clip);

        float distance = Vector3.Distance(m_ListenerTransform.position, mainPos);

        float volume = Mathf.Clamp01((m_audioSource.maxDistance - distance) / (m_audioSource.maxDistance - m_audioSource.minDistance)) * volumeMultipiler;

        m_audioSource.volume = volume;
    }
}
