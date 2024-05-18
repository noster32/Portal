using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CGrabbableObject : CTeleportObject
{
    [HideInInspector] public bool isCollide;

    protected Vector3 defaultRotation = new Vector3(0f, 0f, 0f);
    private Coroutine rotationCoroutine;

    [HideInInspector] public Vector3 originalDirection;                //큐브와 터렛에서만 사용
    [HideInInspector] public Vector3 grabPosition;                    //그랩 위치
    [HideInInspector] public Vector3 grabDirection;                   //큐브와 터렛에서만 사용
    [HideInInspector] public Quaternion grabRotation;
    [HideInInspector] public bool isGrabbed;                          //현재 그랩되어 있음
    public bool isGrabbedTeleport;


    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(isGrabbed)
        {
            GrabPositon(grabPosition);
            GrabRotation();
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
    }

    private void GrabRotation()
    {
        if(CompareTag("Cube") || CompareTag("Turret"))
                transform.forward = grabDirection;
        else
            if(rotationCoroutine == null)
                transform.rotation = grabRotation;
    }

    public void GrabObjectRotationLerp(Quaternion rotation)
    {
        rotationCoroutine = StartCoroutine(LerpCoroutine(0.3f, transform.rotation, rotation));
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

    public override void Teleport()
    {
        base.Teleport();
    }
}
