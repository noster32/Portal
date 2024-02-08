using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class CPlayerInteraction : CComponent
{
    CPortalPlacement portalPlacement;
    CPlayerGrab playerGrab;
    

    public UnityEvent toiletEvent;

    private Transform mainCamera;

    private Ray ray;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        mainCamera = transform.GetChild(1);
    }

    public override void Update()
    {
        base.Update();

        ray = new Ray { origin = mainCamera.transform.position, direction = mainCamera.transform.forward };

        if(Input.GetKeyDown(KeyCode.E))
        {
            int includedLayer = LayerMask.GetMask("InteractionObject", "Portal");

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10f, includedLayer))
            {
                InteractionObject(hit);
            }

            if(CPlayerData.GetInstance().isGrab && CPlayerData.GetInstance().grabObject)
            {
                playerGrab.ObjectGrabTransform();
            }
        }
    }

    public void InteractionObject(RaycastHit h)
    {
        //각 오브젝트에 대한 접속시간이 같기 때문에 switch사용
        switch (h.collider.tag)
        {
            case "Toilet":
                toiletEvent.Invoke();
                break;
            case "Button":
                Debug.Log("not worked");
                break;
            default:
                //Do Grab
                break;
        }
    }
}
