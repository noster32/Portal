using UnityEngine;

public class CPlayerGrab : CComponent
{
    [SerializeField] private float rayDistance = 10.0f;

    private CPortal portal;
    private CPlayerData playerData;

    private Transform mainCameraTransform;

    public override void Awake()
    {
        base.Awake();

        mainCameraTransform = transform.GetChild(1);
    }
    public override void Start()
    {
        base.Start();

        playerData = CPlayerData.GetInstance();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerData.isGrab && playerData.grabObject)
        {
            ObjectGrabTransform();
        }
    }

    private void GrabObjectInit()
    {
        playerData.isGrab = true;
        playerData.grabObject.isGrabbed = true;
        playerData.grabObject.objRigidbody.useGravity = false;
        playerData.grabObject.playerTransform = transform;

        if (playerData.grabObject.tag == "Cube" || playerData.grabObject.tag == "Turret")
            playerData.grabObject.originalRotation = transform.InverseTransformDirection(playerData.grabObject.transform.forward);
        else
            playerData.grabObject.GrabObjectRotationLerp();
    }

    public void ReleaseGrab()
    {
        playerData.grabObject.objRigidbody.useGravity = true;
        playerData.grabObject.objRigidbody.drag = 1;
        playerData.grabObject.isGrabbed = false;
        playerData.grabObject.isGrabbedTeleport = false;

        playerData.isGrab = false;
        playerData.grabObject = null;
    }

    public void GrabObject(RaycastHit hit)
    {
        GameObject hitObject = hit.collider.gameObject;

        if(hitObject.layer == LayerMask.NameToLayer("Portal"))
        {
            portal = hitObject.GetComponent<CPortal>();

            TryGrabObjectThroughPortal(hit.point, hit.distance);
        }
        else if(hitObject.layer == LayerMask.NameToLayer("Turret"))
        {
            playerData.grabObject = hitObject.GetComponentInParent<CGrabableObject>();

            if (playerData.grabObject)
            {
                GrabObjectInit();
            }
            else
                Debug.Log("Turret grab fail");
        }
        else
        {
            Debug.Log(hitObject);
            playerData.grabObject = hitObject.GetComponent<CGrabableObject>();

            if (playerData.grabObject)
            {
                GrabObjectInit();
            }
            else
                Debug.Log("Object grab fail");
        }
        
    }
    private void TryGrabObjectThroughPortal(Vector3 hitPoint, float distance)
    {
        var relativePos = portal.transform.InverseTransformPoint(hitPoint - new Vector3(0.5f, 0f, 0f));
        relativePos = Quaternion.Euler(0f, 180f, 0f) * relativePos;
        Vector3 otherPortal = portal.otherPortal.transform.TransformPoint(relativePos);
        var otherPortalRay = new Ray { origin = otherPortal, direction = mainCameraTransform.transform.forward.normalized };

        RaycastHit hit;

        if(Physics.Raycast(otherPortalRay, out hit, rayDistance - distance, LayerMask.GetMask("InteractionObject")))
        {
            GameObject hitObject = hit.collider.gameObject;

            playerData.grabObject = hitObject.GetComponent<CGrabableObject>();

            if (playerData.grabObject)
            {
                GrabObjectInit();
                playerData.grabObject.isGrabbedTeleport = true;
            }
        }
    }
    public void ObjectGrabTransform()
    {
        Vector3 grabPos = mainCameraTransform.TransformPoint(0f, 0f, 2f);

        playerData.grabObject.grabPosition = grabPos;
    }
}
