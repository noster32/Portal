using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalGunAnim : CComponent
{
    private Animation portalGunAnim;

    [SerializeField] private float portalgunDrawTime = 1.0f;
    [SerializeField] private AudioClip fizzleSound;
    [SerializeField] private AudioClip grabSound;
    private AudioSource audioSource;
    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
        portalGunAnim = GetComponent<Animation>();
        portalGunAnim["portalgun_draw"].speed = portalgunDrawTime;
    }

    public override void Start()
    {
        base.Start();

        audioSource.clip = grabSound;
        audioSource.loop = true;
        
        if(portalGunAnim) 
        {
            PortalGunDrawUp();
        }
    }

    public void PortalGunShoot()
    {
        portalGunAnim.Play("portalgun_fire1");
    }

    public void PortalGunDrawUp()
    {
        portalGunAnim.Play("portalgun_draw");
    }

    public void PortalGunFizzle()
    {
        portalGunAnim.Play("portalgun_fizzle");
        audioSource.PlayOneShot(fizzleSound, 0.1f);
    }

    public void PortalGunGrab()
    {
        portalGunAnim.Play("portalgun_pickup");
        audioSource.Play();
    }

    public void PortalGunRelease()
    {
        portalGunAnim.Play("portalgun_release");
        audioSource.Stop();
    }
}
