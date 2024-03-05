using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAutoPortal : CComponent
{
    [SerializeField] private CPortal portal;
    private AudioSource audioSource;

    [SerializeField] private AudioClip portalCloseClip;

    private bool isPlaced;

    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public void AutoPlacePortal()
    {
        RaycastHit hit;

        if(portal.IsPlaced())
        {
            portal.CleanPortal();
            audioSource.PlayOneShot(portalCloseClip, CSoundLoader.Instance.GetEffectVolume(0.4f));
        }

        if(Physics.Raycast(transform.position, -transform.forward, out hit, 5f, LayerMask.GetMask("PortalPlaceable")))
        {
            portal.PlacePortal(hit.collider, hit.point - new Vector3(0f, 0.25f, 0f), hit.transform.rotation);
        }
    }

    public void AutoRemovePortal()
    {
        if(portal.IsPlaced())
        {
            portal.CleanPortal();
            audioSource.PlayOneShot(portalCloseClip, CSoundLoader.Instance.GetEffectVolume(0.7f));
        }
    }
}
