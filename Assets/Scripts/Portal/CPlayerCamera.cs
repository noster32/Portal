using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCamera : CComponent
{
    #region public

    public float sensitivity;
    public Transform orientation;
    public Quaternion characterRot;

    #endregion

    float xRotation;
    float yRotation;


    public override void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void ResetTargetRotation()
    {
        orientation.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}
