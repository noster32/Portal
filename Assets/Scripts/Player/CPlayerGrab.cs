using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CPlayerGrab : CComponent
{
    [SerializeField] private float rayDistance = 10.0f;
    [SerializeField] private float grabDistance = 2f;
    private CPlayerState playerState;
    private Transform mainCameraTransform;
    [SerializeField] private LayerMask grabPositionLayer;

    public override void Awake()
    {
        base.Awake();

        mainCameraTransform = Camera.main.transform;

        playerState = GetComponent<CPlayerState>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerState.isGrab && playerState.grabObject)
        {
            ObjectGrabTransform();
        }
    }

    private void GrabObjectInit()
    {
        playerState.isGrab = true;                                                                      //플레이어 그랩 여부 true
        playerState.grabObject.isGrabbed = true;                                                        //오브젝트의 그랩 상태 true
        playerState.grabObject.grabPosition = mainCameraTransform.TransformPoint(0f, 0f, grabDistance); //초기 그랩 위치 설정
        playerState.grabObject.m_oRigidBody.useGravity = false;                                         //오브젝트 중력 false
        playerState.grabObject.m_oRigidBody.freezeRotation = true;                                      //리지드바디 로테이션 잠금
        playerState.grabObject.playerTransform = transform;                                             //오브젝트 플레이어 트랜스폼 설정

        //큐브와 터렛의 경우 로테이션 고정을 위한 초기 방향 설정 나머지는 플레이어만 바라보게
        if (playerState.grabObject.tag == "Cube" || playerState.grabObject.tag == "Turret")
        {
            playerState.grabObject.originalRotation = transform.InverseTransformDirection(playerState.grabObject.transform.forward);
        }
        else
            playerState.grabObject.GrabObjectRotationLerp();
    }

    public void ReleaseGrab()
    {
        playerState.grabObject.m_oRigidBody.useGravity = true;
        playerState.grabObject.m_oRigidBody.freezeRotation = false;
        playerState.grabObject.isGrabbed = false;

        playerState.isGrab = false;
        playerState.grabObject = null;
    }

    public void GrabObject(RaycastHit hit)
    {
        GameObject hitObject = hit.collider.gameObject;

        if(hitObject.layer == LayerMask.NameToLayer("PortalCollider"))
        {
            var portalComponent = hitObject.GetComponentInParent<CPortal>();
            if(portalComponent != null)
            {
                TryGrabObjectThroughPortal(hit.point, portalComponent, hit.distance);
            }
        }
        else if(hitObject.tag == "Turret")
        {
            playerState.grabObject = hitObject.GetComponentInParent<CGrabableObject>();

            if (playerState.grabObject)
            {
                GrabObjectInit();
            }
        }
        else
        {
            if (hitObject.TryGetComponent(out CGrabableObject obj))
            {
                playerState.grabObject = obj;
                GrabObjectInit();
            }
            else
            {
                playerState.grabObject = hitObject.GetComponentInParent<CGrabableObject>();

                if(playerState.grabObject)
                {
                    GrabObjectInit();
                }
            }
        }
        
    }

    //포탈 너머의 물체 그랩 함수
    private void TryGrabObjectThroughPortal(Vector3 hitPoint, CPortal portal, float distance)
    {
        //Interaction의 ray가 포탈에 맞은 경우 다른 쪽 포탈로 위치랑 ray의 방향 변환
        Vector3 otherPortalPoint = portal.GetOtherPortalRelativePoint(hitPoint);
        Vector3 otherPortalDirection = portal.GetOtherPortalRelativeDirection(mainCameraTransform.forward).normalized;
        
        var otherPortalRay = new Ray { origin = otherPortalPoint, direction = otherPortalDirection };

        RaycastHit hit;

        //ray에 맞은 경우 그랩
        if(Physics.Raycast(otherPortalRay, out hit, rayDistance - distance, LayerMask.GetMask("InteractionObject")))
        {
            GameObject hitObject = hit.collider.gameObject;

            playerState.grabObject = hitObject.GetComponent<CGrabableObject>();
            if (playerState.grabObject)
            {
                GrabObjectInit();
            }
        }
    }

    //그랩 위치 설정 함수
    public void ObjectGrabTransform()
    {
        RaycastHit hit;
        Vector3 grabPos = Vector3.zero;

        //오브젝트 그랩중일 때 그랩 포지션보다 조금 더 앞에서 포탈을 체크
        if (Physics.Raycast(mainCameraTransform.position, mainCameraTransform.forward, out hit, grabDistance + 1f, LayerMask.GetMask("PortalCollider")))
        {
            var lookPortal = CSceneManager.Instance.portalPair.CheckPortalTag(hit.collider.tag);

            //포탈이 있을 경우에 그랩 위치와 포탈간의 내적을 구해서 -값일 경우에 반대편 포탈로 위치 변경
            if (lookPortal.GetPortalDotValue(mainCameraTransform.TransformPoint(0f, 0f, grabDistance)) < 0)
            {
                grabPos = lookPortal.GetOtherPortalRelativePoint(mainCameraTransform.TransformPoint(0f, 0f, grabDistance));
            }
            else
            {
                grabPos = mainCameraTransform.TransformPoint(0f, 0f, grabDistance);
            }
        }
        else
        {
            grabPos = mainCameraTransform.TransformPoint(0f, 0f, grabDistance);
        }

        playerState.grabObject.grabPosition = grabPos;
    }
}
