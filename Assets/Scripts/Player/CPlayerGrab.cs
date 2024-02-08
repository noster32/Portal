using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CPlayerGrab : CComponent
{
    public CPortal portal;
    private Transform mainCamera;
    private CPlayerData playerData;

    private Ray ray;
    private RaycastHit[] rayCastArr;
    private RaycastHit[] rayCastArr2;
    private float rayDistance = 10.0f;

    private Vector3 originalRot;

    [SerializeField] private Transform grabArea;

    [SerializeField] private float grapRange = 5.0f;
    [SerializeField] private float grabForce = 250f;

    private AudioSource audioSource;

    [SerializeField] AudioClip grabSoundClip;
    [SerializeField] AudioClip grabFailSoundClip;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        mainCamera = transform.GetChild(1);
        audioSource = GetComponent<AudioSource>();

        playerData = CPlayerData.GetInstance();
    }

    public override void Update()
    {
        base.Update();
        
        ray = new Ray { origin = mainCamera.transform.position, direction = mainCamera.transform.forward };
        
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            ObjectGrab();
        }

        if (playerData.isGrab && playerData.grabObject)
        {
            ObjectGrabTransform();
        }
    }

    private void ObjectGrab()
    {
        if (playerData.isGrab && playerData.grabObject)
        {
            ReleaseGrab();
        }
        else if (!playerData.isGrab && !playerData.grabObject)
        {
            TryGrabObject();
        }
    }

    private void ReleaseGrab()
    {
        playerData.grabObject.objRigidbody.useGravity = true;
        playerData.grabObject.objRigidbody.drag = 1;
        playerData.grabObject.isGrabbed = false;
        playerData.grabObject.isGrabbedTeleport = false;

        playerData.isGrab = false;
        playerData.grabObject = null;

        if (rayCastArr != null)
            Array.Clear(rayCastArr, 0, rayCastArr.Length);
        if (rayCastArr2 != null)
            Array.Clear(rayCastArr2, 0, rayCastArr2.Length);
    }

    private void TryGrabObject()
    {
        rayCastArr = Physics.RaycastAll(ray, rayDistance);
        for (int i = 0; i < rayCastArr.Length; i++)
        {
            RaycastHit hit = rayCastArr[i];

            GameObject hitObject = hit.collider.gameObject;

            playerData.grabObject = hitObject.GetComponent<CGrabableObject>();
            portal = hitObject.GetComponent<CPortal>();
            Debug.Log(playerData.grabObject);
            if (playerData.grabObject)
            {
                GrabObjectInit();
                audioSource.PlayOneShot(grabSoundClip);
                break;
            }
            else if (portal)
            {
                TryGrabObjectThroughPortal(hit.point, hit.distance);
                if (playerData.grabObject)
                    break;
            }
        }

        if(!playerData.grabObject)
            audioSource.PlayOneShot(grabFailSoundClip);
    }

    private void GrabObjectInit()
    {
        playerData.isGrab = true;
        playerData.grabObject.isGrabbed = true;
        playerData.grabObject.objRigidbody.useGravity = false;
        playerData.grabObject.playerTransform = transform;

        if (playerData.grabObject.tag == "Cube")
            originalRot = transform.InverseTransformDirection(playerData.grabObject.transform.forward);
    }

    private void TryGrabObjectThroughPortal(Vector3 hitPoint, float distance)
    {
        var relativePos = portal.transform.InverseTransformPoint(hitPoint - new Vector3(0.5f, 0f, 0f));
        relativePos = Quaternion.Euler(0f, 180f, 0f) * relativePos;
        Vector3 otherPortal = portal.otherPortal.transform.TransformPoint(relativePos);
        var otherPortalRay = new Ray { origin = otherPortal, direction = mainCamera.transform.forward.normalized };

        rayCastArr2 = Physics.RaycastAll(otherPortalRay, rayDistance - distance);
        for (int i = 0; i < rayCastArr2.Length; i++)
        {
            RaycastHit hit = rayCastArr2[i];

            GameObject hitObject = hit.collider.gameObject;

            playerData.grabObject = hitObject.GetComponent<CGrabableObject>();

            if (playerData.grabObject)
            {
                GrabObjectInit();
                playerData.grabObject.isGrabbedTeleport = true;
                break;
            }
        }
    }
    public void ObjectGrabTransform()
    {
        Vector3 grabPos = mainCamera.TransformPoint(0f, 0f, 2f);

        playerData.grabObject.grabPosition = grabPos;

        if(!playerData.grabObject.isCollide)
        {
            if (playerData.grabObject.tag == "Cube")
                playerData.grabObject.transform.forward = transform.TransformDirection(originalRot);
        } 
    }
}
