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
    }


    public override void Update()
    {
        CSoundLoader.Instance.PlaySound3D(audioSource, transform.position, targetObj.position, volumeMultipiler);
    }
}
