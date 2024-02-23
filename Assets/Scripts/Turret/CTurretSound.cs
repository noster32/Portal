using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTurretSound : CComponent
{
    [Header("Turret Voice")]
    [SerializeField] private AudioClip[] turretActiveClip;
    [SerializeField] private AudioClip[] turretDisableClip;
    [SerializeField] private AudioClip[] turretSearchClip;
    [SerializeField] private AudioClip[] turretSearchRetireClip;
    [SerializeField] private AudioClip[] turretShootAtClip;
    [SerializeField] private AudioClip[] turretFallClip;
    [SerializeField] private AudioClip[] turretPickUpClip;
    [SerializeField] private AudioClip turretFizzleClip;

    [Header("Turre Sound")]
    [SerializeField] private AudioClip[] turretGunSoundClip;
    [SerializeField] private AudioClip turretGunRotateClip;
    [SerializeField] private AudioClip turretDeployClip;
    [SerializeField] private AudioClip turretPingClip;
    [SerializeField] private AudioClip turretRetractClip;
    [SerializeField] private AudioClip turretDieClip;


    private System.Random rand;
    private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();
        rand = new System.Random();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayTurretActiveVoiceSound()
    {
        PlayRandomSound(turretActiveClip);
    }

    public void PlayTurretDIsableVoiceSound()
    {
        PlayRandomSound(turretDisableClip);
    }

    public void PlayTurretSearchingVoiceSound()
    {
        PlayRandomSound(turretSearchClip);
    }

    public void PlayTurretSearchingRetireVoiceSound()
    {
        PlayRandomSound(turretSearchRetireClip);
    }

    public void PlayTurretPickUpVoiceSound()
    {
        PlayRandomSound(turretPickUpClip);
    }

    public void PlayTurretFallDownVoiceSound()
    {
        PlayRandomSound(turretFallClip);
    }

    public void PlayTurretGunSound()
    {
        PlayRandomSound(turretGunSoundClip);
    }

    public void PlayTurretGunRotationSound()
    {
        audioSource.PlayOneShot(turretGunRotateClip);
    }

    public void PlayTurretPingSound()
    {
        audioSource.PlayOneShot(turretPingClip);
    }
    public void PlayTurretDeploySound()
    {
        audioSource.PlayOneShot(turretDeployClip);
    }
    public void PlayTurretRetractSound()
    {
        audioSource.PlayOneShot(turretRetractClip);
    }

    public void PlayTurretDieSound()
    {
        audioSource.PlayOneShot(turretDieClip);
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        int num = rand.Next(clips.Length);
        audioSource.PlayOneShot(clips[num]);
    }

}
