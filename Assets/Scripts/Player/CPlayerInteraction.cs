using System;
using UnityEngine;

public class CPlayerInteraction : CComponent
{
    [Header("PortalGun")]
    [SerializeField] private GameObject portalGunObject;
    [SerializeField] private GameObject portalGunFirstPerson;
    [SerializeField] private CPortalGunAnim portalGunAnim;
    private CPortalPlacement portalPlacement;
    private CPlayerGrab playerGrab;
    private CPlayerState playerState;
    private CPlayerMouseLook playerRecoil;

    private Transform mainCamera;
    private Ray ray;

    public override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main.transform;
        playerGrab = GetComponent<CPlayerGrab>();
        portalPlacement = GetComponent<CPortalPlacement>();
        playerState = GetComponent<CPlayerState>();
        playerRecoil = GetComponent<CPlayerMouseLook>();
    }

    public override void Start()
    {
        base.Start();

        if (playerState.GetDrawPortalGun())
        {
            portalGunObject.SetActive(true);
            portalGunFirstPerson.SetActive(true);
        }
    }

    public override void Update()
    {
        base.Update();

        if (CGameManager.Instance.GetIsPaused())
            return;

        if (!portalGunObject.activeSelf)
        {
            if(playerState.GetDrawPortalGun())
            {
                portalGunObject.SetActive(true);
                portalGunFirstPerson.SetActive(true);
            }
        }

        ray = new Ray { origin = mainCamera.transform.position, direction = mainCamera.transform.forward };

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (playerState.GetIsGrab() && playerState.grabObject)
            {
                playerGrab.ReleaseGrab();
                if (playerState.GetDrawPortalGun())
                    portalGunAnim.PortalGunRelease();
            }
            else if(playerState.GetIsDrawBluePortalGun())
            {
                portalPlacement.FirePortal(0);
                portalGunAnim.PortalGunShoot(0);
                playerRecoil.FireRecoil();
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (playerState.GetIsGrab() && playerState.grabObject)
            {
                playerGrab.ReleaseGrab();
                if (playerState.GetDrawPortalGun())
                    portalGunAnim.PortalGunRelease();
            }
            else if(playerState.GetIsDrawOrangePortalGun())
            {
                portalPlacement.FirePortal(1);
                portalGunAnim.PortalGunShoot(1);
                playerRecoil.FireRecoil();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            int includedLayer = LayerMask.GetMask("InteractionObject", "PortalCollider");

            RaycastHit hit;

            if(playerState.GetIsGrab() && playerState.grabObject)
            {
                playerGrab.ReleaseGrab();
                if (playerState.GetDrawPortalGun())
                    portalGunAnim.PortalGunRelease();
            }
            else if(!playerState.GetIsGrab() && !playerState.grabObject)
            {
                if (Physics.Raycast(ray, out hit, 10f, includedLayer))
                {
                    InteractionObject(hit);
                }
                else
                {
                    CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.interactionFail, this.transform.position);
                }
            }
        }
    }

    public void InteractionObject(RaycastHit h)
    {
        GameObject hitObject = h.collider.gameObject;

        if(hitObject.TryGetComponent(out CInteractObject interactScript))
        {
            interactScript.CallInteract();
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.interaction, this.transform.position);
        }
        else if(hitObject.TryGetComponent(out CGrabbableObject grabbableObject))
        {
            if (playerState.GetDrawPortalGun())
                portalGunAnim.PortalGunGrab();

            playerGrab.GrabObject(grabbableObject);
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.interaction, this.transform.position);
        }
        else if (hitObject.CompareTag("Turret"))
        {
            CGrabbableObject turret = hitObject.GetComponentInParent<CGrabbableObject>();

            if (playerState.GetDrawPortalGun())
                portalGunAnim.PortalGunGrab();

            if (turret)
                playerGrab.GrabObject(turret);
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.interaction, this.transform.position);
        }
        else if(hitObject.layer == LayerMask.NameToLayer("PortalCollider"))
        {
            CPortal hitPortal = CSceneManager.Instance.portalPair.CheckPortalTag(hitObject.tag);

            if (hitPortal)
                playerGrab.TryGrabObjectThroughPortal(h.point, hitPortal, h.distance, InteractionObject);
        }
        else
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.interactionFail, this.transform.position);
    }
}
