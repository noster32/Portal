using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : CComponent
{
    public Portal[] Portals { private set; get; }

    public override void Awake()
    {
        base.Awake();

        Portals = GetComponentsInChildren<Portal>();

        if(Portals.Length != 2)
        {
            Debug.LogError("PortalPair children must contain exactly two Portal");
        }
    }
}
