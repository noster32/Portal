using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalGunAnim : CComponent
{
    Animation PortalGun;

    public override void Start()
    {
        base.Start();

        PortalGun = this.gameObject.GetComponent<Animation>();

    }

    public override void Update()
    {
        base.Update();

        PortalGunShoot();
    }

    private void PortalGunShoot()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            PortalGun.Play("v_portalgun.qc_skeleton|fire1");
        }
    }
}
