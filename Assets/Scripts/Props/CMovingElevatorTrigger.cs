using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMovingElevatorTrigger : CComponent
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Turret")
        {
            other.transform.parent.parent.SetParent(this.transform);
        }
        else
            other.transform.SetParent(this.transform);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Turret")
        {
            other.transform.parent.parent.SetParent(null);
        }
        else
            other.transform.SetParent(null);
    }
}
