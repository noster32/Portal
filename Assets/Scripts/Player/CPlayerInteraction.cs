using UnityEngine;

public class CPlayerInteraction : CComponent
{
    [Header("Interaction Sound")]
    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip interactionFailSound;

    [Header("Portal Shoot Sound")]
    [SerializeField] private AudioClip portalShootBlueClip;
    [SerializeField] private AudioClip portalShootOrangeClip;

    [Header("PortalGun")]
    [SerializeField] private GameObject portalGunObject;
    [SerializeField] private GameObject portalGunFirstPerson;
    [SerializeField] private CPortalGunAnim portalGunAnim;
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

        if (playerData.GetDrawPortalGun())
        {
            portalGunObject.SetActive(true);
            portalGunFirstPerson.SetActive(true);
        }
        //audioSource.clip = grabSound;
        //audioSource.loop = true;
    }

    public override void Update()
    {
        base.Update();

        if(!portalGunObject.activeSelf)
        {
            if(playerData.GetDrawPortalGun())
            {
                portalGunObject.SetActive(true);
                portalGunFirstPerson.SetActive(true);
            }
        }

        ray = new Ray { origin = mainCamera.transform.position, direction = mainCamera.transform.forward };

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (playerData.isGrab && playerData.grabObject)
            {
                playerGrab.ReleaseGrab();
                if (playerData.GetDrawPortalGun())
                    portalGunAnim.PortalGunRelease();
            }
            else if(playerData.GetIsDrawBluePortalGun())
            {
                portalPlacement.FirePortal(0);
                portalGunAnim.PortalGunShoot();
                audioSource.PlayOneShot(portalShootBlueClip, CSoundLoader.Instance.GetEffectVolume(0.8f));
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (playerData.isGrab && playerData.grabObject)
            {
                playerGrab.ReleaseGrab();
                if (playerData.GetDrawPortalGun())
                    portalGunAnim.PortalGunRelease();
            }
            else if(playerData.GetIsDrawOrangePortalGun())
            {
                portalPlacement.FirePortal(1);
                portalGunAnim.PortalGunShoot();
                audioSource.PlayOneShot(portalShootOrangeClip, CSoundLoader.Instance.GetEffectVolume(0.8f));
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            int includedLayer = LayerMask.GetMask("InteractionObject", "Portal", "Turret");

            RaycastHit hit;

            if(playerData.isGrab && playerData.grabObject)
            {
                playerGrab.ReleaseGrab();
                if (playerData.GetDrawPortalGun())
                    portalGunAnim.PortalGunRelease();

                audioSource.Stop();
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
        if(h.collider.gameObject.TryGetComponent<CInteractObject>(out CInteractObject interactScript))
        {
            interactScript.CallInteract(this);
        }
        else
        {
            if (playerData.GetDrawPortalGun())
                portalGunAnim.PortalGunGrab();
            playerGrab.GrabObject(h);
            audioSource.Play();
        }
    }
}
