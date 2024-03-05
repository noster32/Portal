using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CLab00Part1 : CComponent
{
    [SerializeField] private UnityEvent enterEvent;
    [SerializeField] private UnityEvent boxDropperEvent;

    private bool m_isActive = false;


    private void OnTriggerEnter(Collider other)
    {
        if(!m_isActive && other.tag == "Player")
        {
            m_isActive = true;
            StartCoroutine(enterEventCoroutine());
        }
    }

    private IEnumerator enterEventCoroutine()
    {
        enterEvent.Invoke();

        yield return new WaitForSeconds(2f);

        boxDropperEvent.Invoke();

        yield return null;
    }
}
