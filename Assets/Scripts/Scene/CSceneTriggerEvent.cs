using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CSceneTriggerEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEvent;

    private bool m_isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!m_isActive && other.tag == "Player")
        {
            m_isActive = true;
            triggerEvent.Invoke();
        }
    }
}
