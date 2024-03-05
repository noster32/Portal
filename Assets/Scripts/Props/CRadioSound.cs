using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRadioSound : CComponent
{
    [Header("Sound Setting")]
    public AudioClip audioClip;
    public float maxvolume = 1f;
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public Transform targetObj;
    public float volumeMultipiler = 1f;

    private AudioSource audioSource;

    [SerializeField] private AudioClip[] collisionSoundClips;
    [SerializeField] private Transform[] testCube;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();

        audioSource.clip = audioClip;
        //audioSource.volume = CSceneLoader.Instance.mu
    }


    public override void Update()
    {
        base.Update();

        CSoundLoader.Instance.PlaySound3D(transform.position, volumeMultipiler);

        Debug.Log("test1" + transform.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
            return;

        Debug.Log(CalculateCollisionVolume());
        audioSource.PlayOneShot(collisionSoundClips[0], CalculateCollisionVolume());
    }

    private float CalculateCollisionVolume()
    {
        float vel = m_oRigidBody.velocity.magnitude;
        float volume;
        volume = 0.4f * ((vel = vel / 5f - 1) * vel * vel + 1) + 0f;

        return volume;
    }
}
