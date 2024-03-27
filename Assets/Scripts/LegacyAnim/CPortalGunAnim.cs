using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalGunAnim : CComponent
{
    private Animation portalGunAnim;

    [SerializeField] private float portalgunDrawTime = 1.0f;
    private EventInstance portalGunHoldSound;

    public override void Awake()
    {
        base.Awake();

        portalGunAnim = GetComponent<Animation>();
        portalGunAnim["portalgun_draw"].speed = portalgunDrawTime;
    }

    public override void Start()
    {
        base.Start();

        if (portalGunAnim) 
        {
            PortalGunDrawUp();
        }

        portalGunHoldSound = CAudioManager.Instance.CreateEventInstance(CFMODEvents.Instance.portalGunHold);
    }

    public void PortalGunShoot(int num)
    {
        portalGunAnim.Play("portalgun_fire1");

        switch (num)
        {
            case 0:
                CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalgunShootBlue, this.transform.position);
                break;
            case 1:
                CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalgunShootRed, this.transform.position);
                break;
        }
    }

    public void PortalGunDrawUp()
    {
        portalGunAnim.Play("portalgun_draw");
    }

    public void PortalGunFizzle()
    {
        portalGunAnim.Play("portalgun_fizzle");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalgunFizzle, this.transform.position);
    }

    public void PortalGunGrab()
    {
        portalGunAnim.Play("portalgun_pickup");
        portalGunHoldSound.start();
    }

    public void PortalGunRelease()
    {
        portalGunAnim.Play("portalgun_release");
        portalGunHoldSound.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
