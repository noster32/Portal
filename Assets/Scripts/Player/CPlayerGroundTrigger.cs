using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerGroundTrigger : CComponent
{
    private CPlayerState playerState;
    private CPlayerMovement playerMovement;

    public override void Awake()
    {
        base.Awake();

        playerState = GetComponentInParent<CPlayerState>();
        playerMovement = GetComponentInParent<CPlayerMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //playerMovement.SetIsJump(false);
    }
}
