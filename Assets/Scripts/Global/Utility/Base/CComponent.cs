using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CComponent : MonoBehaviour
{

    #region pub

    [HideInInspector]public Transform m_oTransform = null;
    [HideInInspector]public Rigidbody m_oRigidBody = null;
    [HideInInspector]public Rigidbody2D m_oRigidBody2D = null;

    #endregion
    public virtual void Awake()
    {
        m_oTransform = this.transform;
        m_oRigidBody = this.GetComponent<Rigidbody>();
        m_oRigidBody2D = this.GetComponent<Rigidbody2D>();
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
