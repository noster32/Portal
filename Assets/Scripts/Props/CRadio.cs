using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRadio : CGrabableObject
{
    #region public
    [Header("Sound Setting")]
    public AudioClip audioClip;
    public float maxvolume = 1f;
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public Transform targetObj;
    public float volumeMultipiler = 1f;
    #endregion

    private AudioSource audioSource;

    [SerializeField] private AudioClip[] collisionSoundClips;
    [SerializeField] private Transform[] testCube;

    public override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();
        CSoundLoader.Instance.AudioInit(audioSource, audioClip, maxvolume, true, minDistance, maxDistance);
        CSoundLoader.Instance.SetListener();
    }


    public override void Update()
    {
        base.Update();

        CSoundLoader.Instance.PlaySound3D(transform.position, volumeMultipiler);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
            return;

        audioSource.PlayOneShot(collisionSoundClips[0], SoundUtility.CalculateCollisionVolume(objRigidbody));
    }

}
