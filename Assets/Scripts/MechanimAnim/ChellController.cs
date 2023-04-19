using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChellController : PortalableObject
{
    private ChellMove chellMove;
    public override void Awake()
    {
        base.Awake();

        chellMove = GetComponent<ChellMove>();
    }

    public override void Warp()
    {
        base.Warp();

        chellMove.ResetTargetRotation();
    }
}
