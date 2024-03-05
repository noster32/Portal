using UnityEngine;

public class CPlayerGrab : CComponent
{
    [SerializeField] private float rayDistance = 10.0f;

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
        playerData.grabObject.m_oRigidBody.useGravity = false;
        playerData.grabObject.m_oRigidBody.freezeRotation = true;
        playerData.grabObject.playerTransform = transform;

        if (playerData.grabObject.tag == "Cube" || playerData.grabObject.tag == "Turret")
        {
            playerData.grabObject.originalRotation = transform.InverseTransformDirection(playerData.grabObject.transform.forward);
        }
        else
            playerData.grabObject.GrabObjectRotationLerp();
    }

    public void ReleaseGrab()
    {
        playerData.grabObject.m_oRigidBody.useGravity = true;
        playerData.grabObject.m_oRigidBody.freezeRotation = false;
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
            if(hitObject.TryGetComponent<CPortal>(out CPortal portalComp))
            {
                TryGrabObjectThroughPortal(hit.point, portalComp, hit.distance);
            }
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
            if (hitObject.TryGetComponent<CGrabableObject>(out CGrabableObject obj))
            {
                playerData.grabObject = obj;
                GrabObjectInit();
            }
            else
                Debug.Log("Object Grab Fail");
        }
        
    }
    private void TryGrabObjectThroughPortal(Vector3 hitPoint, CPortal portal,float distance)
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
        Vector3 grabPos = mainCameraTransform.TransformPoint(0f, 0f, 1.5f);

        playerData.grabObject.grabPosition = grabPos;
    }
}
