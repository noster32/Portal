using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CameraMove))]

public class PortalPlacemnet : CComponent
{
    [SerializeField]
    private PortalPair portals;

    [SerializeField]
    private LayerMask layerMask;


    //[SerializeField]
    //private Crosshair crosshair;

    private CameraMove playerCamera;

    public override void Awake()
    {
        base.Awake();

        playerCamera = transform.GetComponent<CameraMove>();

    }

    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Fire1"))
        {
            FirePortal(0, transform.GetChild(1).position, transform.GetChild(1).forward, 250.0f);
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            FirePortal(1, transform.GetChild(1).position, transform.GetChild(1).forward, 250.0f);
        }
    }

    private void FirePortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit, distance, layerMask);

        if(hit.collider != null && hit.collider.tag == "Concret")
        {
            if(hit.collider.tag == "Portal")
            {
                var inPortal = hit.collider.GetComponent<Portal>();

                if(inPortal == null)
                {
                    return;
                }

                var outPortal = inPortal.GetOtherPortal();

                Vector3 relativePos = inPortal.transform.InverseTransformPoint(hit.point + dir);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                pos = outPortal.transform.TransformPoint(relativePos);

                Vector3 relativeDir = inPortal.transform.InverseTransformDirection(dir);
                relativeDir = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeDir;
                dir = outPortal.transform.TransformDirection(relativeDir);

                distance -= Vector3.Distance(pos, hit.point);

                FirePortal(portalID, pos, dir, distance);

                return;
            }

            var cameraRotation = playerCamera.qCharacterRotation;

            var portalRight = cameraRotation * Vector3.right;

            if (Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);

            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);

            portals.Portals[portalID].PlacePortal(hit.collider, hit.point, portalRotation);
        }
    }

}
