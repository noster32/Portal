using System.Collections;
using UnityEngine;

public class CGrabableObject : CTeleportObject
{
    [HideInInspector] public bool isCollide;
    [HideInInspector] public Vector3 originalRotation;

    protected Vector3 defaultRotation = new Vector3(0f, 0f, 0f);

    [HideInInspector] public Transform playerTransform;
    private Coroutine rotationCoroutine;

    [HideInInspector] public Vector3 grabPosition;                    //그랩 위치
    [HideInInspector] public bool isGrabbed;                          //현재 그랩되어 있음

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(isGrabbed)
        {
            GrabPositon(grabPosition);
            GrabRotation();
            //if (!isGrabbedTeleport)
            //    GrabRotation();
            //if (isGrabbedTeleport)
            //    GrabRotationInPortal(grabPosition);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        isCollide = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        isCollide = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollide = false;
    }

    private void GrabPositon(Vector3 pos)
    {
        Vector3 spd = ((pos - objectCenter) - transform.position) / Time.deltaTime;
        spd *= 0.8f;

        m_oRigidBody.velocity = spd;
        //m_oRigidBody.AddForce(spd - m_oRigidBody.velocity, ForceMode.VelocityChange);
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

    public void SetPortal1(CPortal portal)
    {
        portal1 = portal;
        portal2 = portal.otherPortal;
    }

    public void SetPortal2(CPortal portal)
    {
        portal1 = portal.otherPortal;
        portal2 = portal;
    }

    public override void Teleport()
    {
        base.Teleport();
    }
}
