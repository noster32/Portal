using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PortalableObject
{
    private CPlayerMovemnet playerMove;
    private CPlayerCamera cameraMove;

    public override void Awake()
    {
        base.Awake();

        cameraMove = GetComponent<CCameraPosition>();
    }

    public override void Warp()
    {
        base.Warp();

        cameraMove.ResetTargetRotation();
    }
}
