using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalGunAnim : CComponent
{
    Animation portalGunAnim;

    private float portalgunDrawTime = 1.6f;

    public override void Awake()
    {
        base.Awake();

        portalGunAnim = GetComponent<Animation>();
        portalGunAnim["portalgun_draw"].speed = portalgunDrawTime;
    }

    public override void Start()
    {
        base.Start();

        if(portalGunAnim) 
        {
            PortalGunDrawUp();
        }
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            PortalGunShoot();
        }
        
    }

    private void PortalGunShoot()
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
    }

    public void PortalGunGrab()
    {
        portalGunAnim.Play("portalgun_pickup");
    }

    public void PortalGunRelease()
    {
        portalGunAnim.Play("portalgun_release");
    }
}
