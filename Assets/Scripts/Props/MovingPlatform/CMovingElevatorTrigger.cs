using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMovingElevatorTrigger : CComponent
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(this.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
