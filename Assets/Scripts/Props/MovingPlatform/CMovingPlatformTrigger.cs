using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMovingPlatformTrigger : CComponent
{
    [SerializeField] private CLightRail lightRail;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Turret"))
        {
            other.transform.parent.parent.SetParent(this.transform);
        }
        else
            other.transform.SetParent(this.transform);

        if(lightRail != null)
        {
            if (other.CompareTag("Player"))
            {
                if (lightRail.GetIsActive())
                {
                    if (lightRail.GetIsPlayerWait())
                    {
                        lightRail.SetIsPlayerWaitFalse();
                    }
                }
                else
                {
                    lightRail.StartLightRailRepeat();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Turret"))
        {
            other.transform.parent.parent.SetParent(null);
        }
        else
            other.transform.SetParent(null);
    }
}
