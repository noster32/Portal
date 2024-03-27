using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CComponent : MonoBehaviour
{
    [HideInInspector]public Transform m_oTransform = null;
    [HideInInspector]public Rigidbody m_oRigidBody = null;

    public virtual void Awake()
    {
        m_oTransform = this.transform;
        m_oRigidBody = this.GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        //Do Nothing
    }

    public virtual void Update()
    {

        //Do Nothing
    }

    public virtual void FixedUpdate()
    {
        //Do Nothing
    }

    public virtual void LateUpdate()
    {
        //Do Nothing
    }

}
