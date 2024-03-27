using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CSceneTriggerMultiple : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            triggerEvent.Invoke();
        }
    }
}
