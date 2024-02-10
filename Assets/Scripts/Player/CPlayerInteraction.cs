using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class CPlayerInteraction : CComponent
{
    [Header("Event")]
    public UnityEvent toiletEvent;

    [Header("Interaction Sound")]
    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioClip interactionFailSound;

    [Header("Portal Shoot Sound")]
    [SerializeField] private AudioClip portalShootBlueClip;
    [SerializeField] private AudioClip portalShootOrangeClip;

    [Header("PortalGun Animation")]
    [SerializeField] private CPortalGunAnim portalGun;
    private CPortalPlacement portalPlacement;
    private CPlayerGrab playerGrab;
    private CPlayerData playerData;

    private Transform mainCamera;
    private AudioSource audioSource;
    private Ray ray;

    public override void Awake()
    {
        base.Awake();

        mainCamera = transform.GetChild(1);
        audioSource = GetComponent<AudioSource>();
        playerGrab = GetComponent<CPlayerGrab>();
        portalPlacement = GetComponent<CPortalPlacement>();
    }

    public override void Start()
    {
        base.Start();

        playerData = CPlayerData.GetInstance();
    }

    public override void Update()
    {
        base.Update();

        ray = new Ray { origin = mainCamera.transform.position, direction = mainCamera.transform.forward };

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (playerData.isGrab && playerData.grabObject)
            {
                playerGrab.ReleaseGrab();
                portalGun.PortalGunRelease();
            }
            else
            {
                portalPlacement.FirePortal(0);
                portalGun.PortalGunShoot();
                audioSource.PlayOneShot(portalShootBlueClip);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (playerData.isGrab && playerData.grabObject)
            {
                playerGrab.ReleaseGrab();
                portalGun.PortalGunRelease();
            }
            else
            {
                portalPlacement.FirePortal(1);
                portalGun.PortalGunShoot();
                audioSource.PlayOneShot(portalShootOrangeClip);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            int includedLayer = LayerMask.GetMask("InteractionObject", "Portal");

            RaycastHit hit;

            if(playerData.isGrab && playerData.grabObject)
            {
                playerGrab.ReleaseGrab();
                portalGun.PortalGunRelease();
            }
            else if(!CPlayerData.GetInstance().isGrab && !CPlayerData.GetInstance().grabObject)
            {
                if (Physics.Raycast(ray, out hit, 10f, includedLayer))
                {
                    InteractionObject(hit);
                    audioSource.PlayOneShot(interactionSound, 0.6f);
                }
                else
                {
                    audioSource.PlayOneShot(interactionFailSound, 0.6f);
                }
            }
            else
            {
                Debug.LogError("PlayerData IsGrab or grabObject Error "
                                + CPlayerData.GetInstance().isGrab + " : " + CPlayerData.GetInstance().grabObject);
            }
        }
    }

    public void InteractionObject(RaycastHit h)
    {
        //각 오브젝트에 대한 접속시간이 같기 때문에 switch사용
        switch (h.collider.tag)
        {
            case "Toilet":
                toiletEvent.Invoke();
                break;
            case "Button":
                Debug.Log("not worked");
                break;
            default:
                portalGun.PortalGunGrab();
                playerGrab.GrabObject(h); 
                break;
        }
    }
}
