using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCube : CGrabableObject
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
            return;

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.metalSolidImpactHard, 0.2f, this.transform.position);
    }
}
