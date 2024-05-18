using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCube : CGrabbableObject
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
            return;

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.metalSolidImpactHard, 0.2f, this.transform.position);
    }
}
