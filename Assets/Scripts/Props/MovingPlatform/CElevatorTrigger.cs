using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CElevatorTrigger : CComponent
{
    [Header("Elevator Event")]
    [SerializeField] private UnityEvent elevatorEvent;

    private bool m_isWorked;

    public override void Update()
    {
        base.Update();

        //if(Input.GetKeyDown(KeyCode.I))
        //{
        //    if (!m_isWorked)
        //    {
        //        m_isWorked = true;
        //        elevatorEvent.Invoke();
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!m_isWorked)
        {
            m_isWorked = true;
            elevatorEvent.Invoke();
        }
    }
}
