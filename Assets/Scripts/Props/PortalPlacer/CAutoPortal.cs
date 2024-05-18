using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAutoPortal : CComponent
{
    [SerializeField] private CPortal portal;

    public void AutoPlacePortal()
    {
        RaycastHit hit;

        if(portal.IsPlaced())
        {
            portal.ClosePortal();
        }

        if(Physics.Raycast(transform.position, -transform.forward, out hit, 5f, LayerMask.GetMask("PortalPlaceable")))
        {
            portal.OpenPortal(hit.point - new Vector3(0f, 0.25f, 0f), hit.transform.rotation);
        }
    }

    public void AutoRemovePortal()
    {
        if(portal.IsPlaced())
        {
            portal.ClosePortal();
        }
    }
}
