using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PortalableObject : CComponent
{
    private Portal inPortal;
    private Portal outPortal;

    private new Rigidbody rigidbody;

    private static Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    protected new Collider collider;

    private GameObject cloneObject;
    private int inPortalCount = 0;

    public override void Awake()
    {
        base.Awake();

        cloneObject = new GameObject();
        cloneObject.SetActive(false);
        var meshFilter = cloneObject.AddComponent<MeshFilter>();
        var meshRenderer = cloneObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer.materials = GetComponent<MeshRenderer>().materials;

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        if(inPortal == null || outPortal == null) 
        {
            return;
        }

        if(cloneObject.activeSelf && inPortal.IsPlaced() && outPortal.IsPlaced())
        {
            var inTransform = inPortal.transform;
            var outTransform = outPortal.transform;

            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = halfTurn * relativePos;
            cloneObject.transform.position = outTransform.TransformPoint(relativePos);

            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
            relativeRot = halfTurn * relativeRot;
            cloneObject.transform.rotation = outTransform.rotation * relativeRot;
        }
        else
        {
            cloneObject.transform.position = new Vector3(-1000.0f, -1000.0f, -1000.0f);
        }
    }

    public virtual void Warp()
    {
        var inTransform = inPortal.transform;
        var outTransform = outPortal.transform;

        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        relativePos = halfTurn * relativePos;
        transform.position = outTransform.TransformPoint(relativePos);

        Quaternion relativeRotate = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRotate = halfTurn * relativeRotate;
        transform.rotation = outTransform.rotation * relativeRotate;

        Vector3 relativeVel = inTransform.InverseTransformDirection(rigidbody.velocity);
        relativeVel = halfTurn * relativeVel;
        rigidbody.velocity = outTransform.TransformDirection(relativeVel);
    }

    public void SetisInPortal(Portal inPortal, Portal outPortal, Collider wallCollision)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;

        Physics.IgnoreCollision(collider, wallCollision);

        cloneObject.SetActive(true);

        ++inPortalCount;
    }

    public void ExitPortal(Collider wallCollider)
    {
        Physics.IgnoreCollision(collider, wallCollider, false);
        --inPortalCount;

        if(inPortalCount == 0)
        {
            cloneObject.SetActive(false);
        }
    }
}
