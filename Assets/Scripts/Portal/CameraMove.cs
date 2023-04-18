using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraMove : CComponent
{
    private const float moveSpeed = 7.5f;
    private const float cameraSpeed = 3.0f;

    private Vector3 moveVector = Vector3.zero;
    private float moveY = 0.0f;

    public Quaternion TargetRotation { private set; get; }

    private new Rigidbody rigidbody;

    public override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        TargetRotation = transform.rotation;
    }

    public override void Update()
    {
        base.Update();

        var rotation = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        var targetEuler = TargetRotation.eulerAngles + (Vector3)rotation * cameraSpeed;
        if (targetEuler.x > 180.0f)
        {
            targetEuler.x -= 360.0f;
        }
        targetEuler.x = Mathf.Clamp(targetEuler.x, -75.0f, 75.0f);
        TargetRotation = Quaternion.Euler(targetEuler);

        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation,
            Time.deltaTime * 15.0f);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        moveVector = new Vector3(x, 0.0f, z) * moveSpeed;

        moveY = Input.GetAxis("Elevation");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 newVelocity = transform.TransformDirection(moveVector);
        newVelocity.y += moveY * moveSpeed;
        rigidbody.velocity = newVelocity;
    }

    public void ResetTargetRotation()
    {
        TargetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}
