using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRadioSong : CComponent
{
    #region public
    public AudioClip audioClip;
    public float maxvolume = 1f;
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public Transform targetObj;
    public float volumeMultipiler = 1f;
    #endregion

    private AudioSource audioSource;


    public override void Start()
    {
        audioSource = GetComponent<AudioSource>();
        CSoundLoader.Instance.LoadSound(audioSource, audioClip, maxvolume, true, minDistance, maxDistance);
        
        //audioSource.volume = maxvolume;
        //audioSource.clip = audioClip;
        //audioSource.loop = true;
        //audioSource.minDistance = minDistance;
        //audioSource.maxDistance = maxDistance;
        
    }


    public override void Update()
    {
        CSoundLoader.Instance.PlaySound3D(audioSource, transform.position, targetObj.position, volumeMultipiler);


        //if (!audioSource.isPlaying)
        //{
        //    audioSource.Play();
        //}
        //
        //if (targetObj != null && audioSource.isPlaying)
        //{
        //    float distance = Vector3.Distance(targetObj.position, transform.position);
        //
        //    float volume = Mathf.Clamp01((maxDistance - distance) / (maxDistance - minDistance)) * volumeMultipiler;
        //
        //    audioSource.volume = volume;
        //}
    }
}
