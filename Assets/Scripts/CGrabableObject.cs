using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class CGrabableObject : CTeleportObject
{
    [HideInInspector] public bool isCollide;
    [HideInInspector] public Vector3 collidePosition;

    protected Vector3 defaultRotation = new Vector3(0f, 0f, 0f);

    public Transform playerTransform;

    public override void Update()
    {
        base.Update();

        if (tag != "Cube" && isGrabbed)
        {
            GrabRotation();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();


        if (isGrabbed && !isGrabbedTeleport)
        {
            GrabPositon(grabPosition);
        }
        else if (isGrabbed && isGrabbedTeleport)
        {
            Vector3 relativePos = portal2.transform.InverseTransformPoint(grabPosition);
            relativePos = reverse * relativePos;
            GrabPositon(portal1.transform.TransformPoint(relativePos));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isCollide = true;
        collidePosition = transform.position;
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollide = false;
    }

    private void GrabPositon(Vector3 pos)
    {
        Vector3 spd = (pos - transform.position) / Time.deltaTime;

        objRigidbody.velocity = spd;
    }

    public Vector3 GetDefaultGrabRotation()
    {
        Debug.Log(defaultRotation);
        return Quaternion.Euler(defaultRotation) * -Vector3.forward;
    }

    private void GrabRotation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        Quaternion yOnlyRot = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
        Debug.Log(yOnlyRot.eulerAngles);
        float lerpFactor = 20f * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, yOnlyRot, lerpFactor);

        //if (transform.rotation.eulerAngles.x > 0f || transform.rotation.eulerAngles.z > 0f) 
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f), lerpFactor);
    }
}
