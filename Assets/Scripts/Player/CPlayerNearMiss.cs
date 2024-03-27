using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerNearMiss : CComponent
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.nearmiss, this.transform.position);
        }
    }
}
