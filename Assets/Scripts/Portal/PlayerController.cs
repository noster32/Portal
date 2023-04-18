using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PortalableObject
{
    private CameraMove cameraMove;

    public override void Awake()
    {
        base.Awake();

        cameraMove = GetComponent<CameraMove>();
    }

    public override void Warp()
    {
        base.Warp();

        cameraMove.ResetTargetRotation();
    }
}
