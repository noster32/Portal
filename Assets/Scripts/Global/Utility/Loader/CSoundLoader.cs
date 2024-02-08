using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSoundLoader : CSingleton<CSoundLoader>
{
    private AudioSource m_audioSource;
    private Transform m_ListenerTransform;

    public void AudioInit(AudioSource audioSource, AudioClip audioClip, float volume = 1f, bool loop = false, float minDis = 1f, float maxDis = 500f)
    {
        m_audioSource = audioSource;
        m_audioSource.clip = audioClip;
        m_audioSource.volume = volume;
        m_audioSource.loop = loop;
        m_audioSource.minDistance = minDis;
        m_audioSource.maxDistance = maxDis;
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

    public void PlaySound(AudioClip audioClip = null)
    {
        if (audioClip != null)
            m_audioSource.clip = audioClip;

        if (!m_audioSource.isPlaying)
        {
            m_audioSource.Play();
        }
    }

    public void PlaySoundOneShot(AudioClip audioClip = null)
    {
        if (audioClip != null)
            m_audioSource.PlayOneShot(audioClip);
        else
            m_audioSource.PlayOneShot(m_audioSource.clip);
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
