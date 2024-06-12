using System;
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

        if (playerState.GetIsGrab() && playerState.grabObject)
        {
            ObjectGrabTransform();
        }
    }
    public void GrabObject(CGrabbableObject obj)
    {
        playerState.grabObject = obj;
        GrabObjectInit();
    }

    //포탈 너머의 물체 그랩 함수
    //Interaction의 ray가 포탈에 맞은 경우 다른 쪽 포탈로 위치랑 ray의 방향 변환
    //ray에 맞은 경우 그랩
    public void TryGrabObjectThroughPortal(Vector3 hitPoint, CPortal portal, float distance, Action<RaycastHit> interact)
    {
        Vector3 otherPortalPoint = portal.GetOtherPortalRelativePoint(hitPoint);
        Vector3 otherPortalDirection = portal.GetOtherPortalRelativeDirection(mainCameraTransform.forward).normalized;

        var otherPortalRay = new Ray { origin = otherPortalPoint, direction = otherPortalDirection };

        RaycastHit hit;
        if (Physics.Raycast(otherPortalRay, out hit, rayDistance - distance, LayerMask.GetMask("InteractionObject")))
        {
            interact(hit);
        }
        else
        {
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.interactionFail, this.transform.position);
        }
    }

    private void GrabObjectInit()
    {
        playerState.SetIsGrab(true);                                                                    //플레이어 그랩 여부 true
        playerState.grabObject.isGrabbed = true;                                                        //오브젝트의 그랩 상태 true
        playerState.grabObject.grabPosition = mainCameraTransform.TransformPoint(0f, 0f, grabDistance); //초기 그랩 위치 설정
        playerState.grabObject.m_oRigidBody.useGravity = false;                                         //오브젝트 중력 false
        playerState.grabObject.m_oRigidBody.freezeRotation = true;                                      //리지드바디 로테이션 잠금

        Quaternion lookRotation = Quaternion.LookRotation(transform.GetChild(1).position - 
                                                          mainCameraTransform.TransformPoint(0f, 0f, grabDistance));

        if (playerState.grabObject.CompareTag("Cube") || playerState.grabObject.CompareTag("Turret"))
        {
            playerState.grabObject.originalDirection = 
                transform.InverseTransformDirection(playerState.grabObject.transform.forward);          //큐브와 터렛은 잡았을 떄 같은 방향을 바라보게 방향 저장
            playerState.grabObject.grabDirection = 
                transform.TransformDirection(playerState.grabObject.originalDirection);
        }
        else
            playerState.grabObject.GrabObjectRotationLerp(lookRotation);
    }

    public void ReleaseGrab()
    {
        playerState.grabObject.m_oRigidBody.useGravity = true;
        playerState.grabObject.m_oRigidBody.freezeRotation = false;
        playerState.grabObject.isGrabbed = false;
        playerState.grabObject.isGrabbedTeleport = false;

        playerState.SetIsGrab(false);
        playerState.grabObject = null;
    }

    //그랩 위치 로테이션 설정 함수
    //오브젝트 그랩중일 때 그랩 포지션보다 조금 더 앞에서 포탈을 체크
    //포탈이 있을 경우에 그랩 위치와 포탈간의 내적을 구해서 -값일 경우에 반대편 포탈로 위치 변경
    //로테이션은 큐브와 터렛은 플레이어가 화면을 돌려도 같은 방향을 유지하도록 설정
    //큐브와 터렛 외의 나머지 오브젝트는 플레이어를 바라보도록 설정
    //오브젝트가 반대편 포탈에 있을 경우에는 grabPos처럼 반대편 포탈 기준으로 로테이션 설정
    public void ObjectGrabTransform()
    {
        RaycastHit hit;
        Vector3 grabPos = Vector3.zero;

        if (Physics.Raycast(mainCameraTransform.position, mainCameraTransform.forward, out hit,
            grabDistance + 1f, LayerMask.GetMask("PortalCollider")))
        {
            var lookPortal = CSceneManager.Instance.portalPair.CheckPortalTag(hit.collider.tag);

            if (lookPortal.GetPortalDotValue(mainCameraTransform.TransformPoint(0f, 0f, grabDistance)) < 0)
            {
                playerState.grabObject.isGrabbedTeleport = true;
                grabPos = lookPortal.GetOtherPortalRelativePoint(mainCameraTransform.TransformPoint(0f, 0f, grabDistance));             

                if (playerState.grabObject.CompareTag("Cube") || playerState.gameObject.CompareTag("Turret"))                           
                {
                    Vector3 dir = lookPortal.GetOtherPortalRelativeDirection(playerState.grabObject.originalDirection);
                    playerState.grabObject.grabDirection = transform.TransformDirection(dir);
                }
                else
                    playerState.grabObject.grabRotation = 
                        Quaternion.LookRotation(lookPortal.GetOtherPortalRelativePoint(transform.GetChild(1).position) - grabPos);
            }
            else
            {
                playerState.grabObject.isGrabbedTeleport = false;
                grabPos = mainCameraTransform.TransformPoint(0f, 0f, grabDistance);

                if (playerState.grabObject.CompareTag("Cube") || playerState.grabObject.CompareTag("Turret"))
                    playerState.grabObject.grabDirection = transform.TransformDirection(playerState.grabObject.originalDirection);
                else
                    playerState.grabObject.grabRotation = 
                        Quaternion.LookRotation(transform.GetChild(1).position - grabPos);
            }
        }
        else
        {
            grabPos = mainCameraTransform.TransformPoint(0f, 0f, grabDistance);
            if (playerState.grabObject.CompareTag("Cube") || playerState.grabObject.CompareTag("Turret"))
                playerState.grabObject.grabDirection = transform.TransformDirection(playerState.grabObject.originalDirection);
            else
                playerState.grabObject.grabRotation = 
                    Quaternion.LookRotation(transform.GetChild(1).position - grabPos);
        }

        playerState.grabObject.grabPosition = grabPos;
    }
}
