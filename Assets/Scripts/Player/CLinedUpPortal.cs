using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CLinedUpPortal : CComponent
{
    [SerializeField] CPortal portal;
    [SerializeField] CPlayerMovement playerMove;

    public override void Start()
    {
        base.Start();

        playerMove = CSceneManager.Instance.player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!portal.IsPlaced() || !portal.otherPortal.IsPlaced())
            return;

        if(portal.transform.forward == Vector3.up)
        {
            if (other.tag == "Player" && portal.isVisibleFromMainCamera(Camera.main))
            {
                playerMove.LineToPortal(portal.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(portal.transform.forward == Vector3.up)
        {
            if (other.tag == "Player")
            {
                playerMove.StopLineToPortal();
            }
        }
    }
}
