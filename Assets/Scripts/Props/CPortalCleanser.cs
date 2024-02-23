using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalCleanser : CComponent
{
    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private CPortalGunAnim portalGun;

    private int count;

    private void OnTriggerEnter(Collider other)
    {

        for (int i = 0; i < portalPair.portals.Length; i++)
        {
            if (!portalPair.portals[i].IsLevelPlaced() && portalPair.portals[i].IsPlaced())
            {
                portalPair.portals[i].CleanPortal();
                count++;
            }
        }

        if(count > 0)
        {
            portalGun.PortalGunFizzle();
            count = 0;
        }
    }
}
