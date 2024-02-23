using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCube : CGrabableObject
{
    [SerializeField] AudioClip[] collisionSoundClips;
    private AudioSource audioSource;
    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
            return;

        int num = Random.Range(0, collisionSoundClips.Length);
        audioSource.PlayOneShot(collisionSoundClips[num], SoundUtility.CalculateCollisionVolume(objRigidbody));
    }
}
