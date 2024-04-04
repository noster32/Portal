using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalPair : CComponent
{
    public CPortal[] portals { get; set; }

    public override void Awake()
    {
        portals = GetComponentsInChildren<CPortal>();
    }

    public bool PlacedBothPortal() => portals[0].IsPlaced() && portals[1].IsPlaced();

    public bool PlacedOncePortal() => portals[0].IsPlaced() || portals[1].IsPlaced();

    public void CleanBothPortal()
    {
        portals[0].ClosePortal();
        portals[1].ClosePortal();
    }

    public CPortal CheckPortalTag(string tag)
    {
        if (tag == portals[0].tag)
            return portals[0];
        else
            return portals[1];
    }

    public CPortal CheckPortalTag(string tag, out CPortal otherPortal)
    {
        if(tag == portals[0].tag)
        {
            otherPortal = portals[1];
            return portals[0];
        }
        else
        {
            otherPortal = portals[0];
            return portals[1];
        }
    }

}
