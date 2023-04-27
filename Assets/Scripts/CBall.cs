using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBall : CComponent
{
    new Rigidbody rigidbody;
    float speed = 10f;

    public override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        rigidbody.velocity = transform.forward * speed;
    }
}
