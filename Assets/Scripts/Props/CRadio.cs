using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRadio : CGrabableObject
{
    #region public
    [Header("Sound Setting")]
    public AudioClip radioSongClip;
    public float maxvolume = 1f;
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public Transform targetObj;
    public float volumeMultipiler = 1f;
    #endregion

    private AudioSource audioSource;

    [SerializeField] private AudioClip[] collisionSoundClips;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = radioSongClip;
        audioSource.loop = true;
    }

    public override void Start()
    {
        base.Start();

        audioSource.volume = 0.3f * CSoundLoader.Instance.musicSoundVolume;
        audioSource.Play();
    }

    public override void Update()
    {
        base.Update();

        audioSource.volume = 0.3f * CSoundLoader.Instance.musicSoundVolume;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
            return;

        audioSource.PlayOneShot(collisionSoundClips[0], 0.5f * CSoundLoader.Instance.effectSoundVolume);
    }

}
