using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalIgnoreCollision : CComponent
{
    [SerializeField] private CPortal portal;
    private void OnTriggerEnter(Collider other)
    {
       
        if(!portal.IsPlaced() && !portal.otherPortal.IsPlaced())
        {
            return;
        }
        
        
        CTeleportObject tpObject;
        if (other.tag == "Turret")
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();

        if (tpObject != null)
        {
            tpObject.EnterPortalIgnoreCollision(portal.wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!portal.IsPlaced())
        {
            return;
        }

        CTeleportObject tpObject;

        if (other.tag == "Turret")
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();

        if (tpObject != null)
        {
            tpObject.ExitPortalIgnoreCollision(portal.wallCollider);
        }
    }
}

