using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalPair : CComponent
{
    public CPortal[] portals { get; set; }

    public override void Awake()
    {
        portals = GetComponentsInChildren<CPortal>();

        if(portals.Length != 2 )
        {
            Debug.LogError("Portals are not completely contain");
        }
    }

}
