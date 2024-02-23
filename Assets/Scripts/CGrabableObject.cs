using System.Collections;
using System.Runtime.CompilerServices;
using TreeEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEngine;
using UnityEngine.UIElements;

public class CGrabableObject : CTeleportObject
{
    [HideInInspector] public bool isCollide;
    [HideInInspector] public Vector3 originalRotation;

    [SerializeField] private Vector3 objectGrabCenter;

    protected Vector3 defaultRotation = new Vector3(0f, 0f, 0f);

    [HideInInspector] public Transform playerTransform;
    private Coroutine rotationCoroutine;

    private Vector3 testVec;

    public override void Update()
    {
        base.Update();

        //Debug.Log(rotationCoroutine != null);
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isGrabbed && !isGrabbedTeleport)
        {
            GrabPositon(grabPosition);
            GrabRotation();
        }
        else if (isGrabbed && isGrabbedTeleport)
        {
            Vector3 relativePos = portal2.transform.InverseTransformPoint(grabPosition);
            relativePos = reverse * relativePos;
            Vector3 result = portal1.transform.TransformPoint(relativePos);
            GrabPositon(result);
            GrabRotationInPortal(result);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isCollide = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollide = false;
    }

    private void GrabPositon(Vector3 pos)
    {
        Vector3 spd = ((pos - objectGrabCenter) - transform.position) / Time.deltaTime;

        objRigidbody.velocity = spd;
    }

    public Vector3 GetDefaultGrabRotation()
    {
        Debug.Log(defaultRotation);
        return Quaternion.Euler(defaultRotation) * -Vector3.forward;
    }

    private void GrabRotation()
    {
        if(tag == "Cube" || tag == "Turret")
        {
            if(!isCollide)
            {
                transform.forward = playerTransform.TransformDirection(originalRotation);
            }
        }
        else
        {
            if(rotationCoroutine == null)
            {
                Quaternion lookRotation = Quaternion.LookRotation(playerTransform.GetChild(1).position - grabPosition);
                transform.rotation = lookRotation;
            }
        }
    }

    private void GrabRotationInPortal(Vector3 pos)
    {
        if (tag == "Cube" || tag == "Turret")
        {
            if (!isCollide)
            {
                Quaternion relativeRot = Quaternion.Inverse(portal2.transform.rotation) * portal1.transform.rotation;
                relativeRot = reverse * relativeRot;
                Vector3 result = relativeRot * originalRotation;
                transform.forward = playerTransform.TransformDirection(result);
            }
        }
        else
        {
            if(rotationCoroutine == null)
            {
                //포탈 카메라 기준으로 돌리기
                Quaternion lookRotation = Quaternion.LookRotation(playerTransform.GetChild(1).GetChild(3).position - pos);
                transform.rotation = lookRotation;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(testVec, 0.2f);
    }

    public void GrabObjectRotationLerp()
    {
        Quaternion lookRotation = Quaternion.LookRotation(playerTransform.GetChild(1).position - grabPosition);
        rotationCoroutine = StartCoroutine(LerpCoroutine(0.3f, transform.rotation, lookRotation));
    }
    
    private IEnumerator LerpCoroutine(float duration, Quaternion start, Quaternion end)
    {
        float timeElapsed = 0f;

        while(timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transform.rotation = Quaternion.Lerp(start, end, t);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.rotation = end;

        rotationCoroutine = null;
    }
}
