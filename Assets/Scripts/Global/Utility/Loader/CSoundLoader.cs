using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSoundLoader : CSingleton<CSoundLoader>
{
    public void LoadSound(AudioSource audioSource, AudioClip audioclip, float volume = 1f, bool loop = false)
    {
        audioSource.clip = audioclip;
        audioSource.volume = volume;
        audioSource.loop = loop;
    }

    public void LoadSound(AudioSource audioSource, AudioClip audioclip, float volume, bool loop, float minDis, float maxDis)
    {
        audioSource.clip = audioclip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.minDistance = minDis;
        audioSource.maxDistance = maxDis;
    }

    public void PlaySound(AudioSource audioSource)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PlaySoundEffect(AudioSource audioSource)
    {
        audioSource.PlayOneShot(audioSource.clip);
    }


    public void PlaySound3D(AudioSource audioSource, Vector3 mainPos, Vector3 targetPos, float volumeMultipiler)
    {
        if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        if (audioSource.isPlaying)
        {
            float distance = Vector3.Distance(targetPos, mainPos);

            float volume = Mathf.Clamp01((audioSource.maxDistance - distance) / (audioSource.maxDistance - audioSource.minDistance)) * volumeMultipiler;

            audioSource.volume = volume;
        }
    }

    public void PlaySoundEffect3D(AudioSource audioSource, Vector3 mainPos, Vector3 targetPos, float volumeMultipiler)
    {
        audioSource.PlayOneShot(audioSource.clip);

        float distance = Vector3.Distance(targetPos, mainPos);

        float volume = Mathf.Clamp01((audioSource.maxDistance - distance) / (audioSource.maxDistance - audioSource.minDistance)) * volumeMultipiler;

        audioSource.volume = volume;
    }
}
