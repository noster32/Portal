using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBall : CComponent
{
    new Rigidbody rigidbody;

    public override void Awake()
    {
        base.Awake();

        //rigidbody = GetComponent<Rigidbody>();
    }
    public override void Update()
    {
        base.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("aaa");
        //rigidbody.velocity = -rigidbody.velocity;
    }
}
