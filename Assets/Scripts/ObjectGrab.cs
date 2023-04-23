using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectGrab : CComponent
{
    private CameraMove playerCom;
    private Transform CameraTransform;
    [SerializeField] 
    private Transform holdArea;
    private GameObject Object;
    private Rigidbody ObjectRigidbody;

    [SerializeField]
    private float pickupRange = 5.0f;
    [SerializeField]
    private float pickupForce = 150.0f;

    public override void Awake()
    {
        base.Awake();

        CameraTransform = transform.GetChild(1);
        playerCom = GetComponent<CameraMove>();
    }
    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Object == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(CameraTransform.position, CameraTransform.TransformDirection(Vector3.forward), out hit, pickupRange))
                {
                    PickUpObject(hit.transform.gameObject);
                }
            }
            else
            {
                DropObject();
            }
        }
        if (Object != null)
        {
            MoveObject();
        }
    }

    private void MoveObject()
    {
        //if(Vector3.Distance(Object.transform.position, holdArea.position) > 10.0f)
        //{
        //    Vector3 moveDirection = (holdArea.position - Object.transform.position);
        //    ObjectRigidbody.AddForce(moveDirection * pickupForce);
        //}

        ObjectRigidbody.velocity = playerCom.playerMoveVector;
    }

    private void PickUpObject(GameObject pickObj)
    {
        if(pickObj.GetComponent<Rigidbody>())
        {
            ObjectRigidbody = pickObj.GetComponent<Rigidbody>();
            ObjectRigidbody.useGravity = false;
            ObjectRigidbody.drag = 0;
            ObjectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            ObjectRigidbody.transform.parent = holdArea;
            Object = pickObj;
        }
    }

    private void DropObject()
    {
        ObjectRigidbody.useGravity = true;
        ObjectRigidbody.drag = 0;
        ObjectRigidbody.constraints = RigidbodyConstraints.None;
        
        Object.transform.parent = null;
        Object = null;
    }



}
