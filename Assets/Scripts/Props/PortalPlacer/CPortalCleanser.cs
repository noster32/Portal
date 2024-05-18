using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CPortalCleanser : CComponent
{
    [SerializeField] private CPortal[] cleanPortal;
    [SerializeField] private UnityEvent deleteEvent;
    [SerializeField] private CPortalGunAnim portalGun;

    public override void Start()
    {
        base.Start();

        portalGun = CSceneManager.Instance.player.transform.GetChild(1).GetChild(1).GetComponent<CPortalGunAnim>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (cleanPortal.Length != 0)
            {
                int count = 0;

                for (int i = 0; i < cleanPortal.Length; i++)
                {
                    if (cleanPortal[i] == null)
                        break;

                    if (cleanPortal[i].IsPlaced())
                    {
                        cleanPortal[i].ClosePortal();
                        count++;
                    }
                }

                if (count != 0)
                {
                    portalGun.PortalGunFizzle();
                    deleteEvent?.Invoke();
                }
            }
        }
    }
}
