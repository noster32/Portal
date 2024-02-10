using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalCleanser : CComponent
{
    [SerializeField] private CPortalPair portalPair;

    private void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < portalPair.portals.Length;i++)
        {
            if (!portalPair.portals[i].IsLevelPlaced() && portalPair.portals[i].IsPlaced())
                portalPair.portals[i].CleanPortal();
        }
    }
}
