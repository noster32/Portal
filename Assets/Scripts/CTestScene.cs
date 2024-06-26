using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestScene : CComponent
{
    [SerializeField] private CAutoPortal[] portals;

    public override void Start()
    {
        base.Start();

        foreach (var portal in portals)
        {
            portal.AutoPlacePortal();
        }
    }
}
