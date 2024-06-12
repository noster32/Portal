using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyDeploy : CComponent
{
    public float radius;

    [Range(0, 360)] public float angle;

    public Transform playerRef;

    public LayerMask playerMask;
    public LayerMask portalMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public bool seeThroughPortal;

    public float angleToTargetBackward;
    public float angleToTargetLeft;

    [SerializeField] private CPortalPair portalPair;

    private Vector3 relativeTurretPosition;
    private Vector3 relativeTurretDirection;

    private CPortal lookPortal;

    List<Collider> portalCheckList = new List<Collider>();
    public override void Start()
    {
        base.Start();

        if (playerRef == null)
            playerRef = CSceneManager.Instance.player.transform;
        if (portalPair == null)
            portalPair = CSceneManager.Instance.portalPair;

        StartCoroutine(FOVRoutine());
    }

    public override void Update()
    {
        base.Update();

        //플레이어가 범위 안에 있을 때 어느 각도에 플레이어가 위치하는지 설정
        if(canSeePlayer)
        {
            if(seeThroughPortal)
            {
                Vector3 relativePlayerPos = lookPortal.otherPortal.GetOtherPortalRelativePoint(playerRef.position);
                Vector3 directionToTarget = ((transform.position - new Vector3(0f, 0.75f, 0f)) - relativePlayerPos).normalized;
                Vector3 targetDirection = transform.InverseTransformDirection(relativePlayerPos - transform.position);

                angleToTargetBackward = Vector3.Angle(-transform.forward, directionToTarget);
                angleToTargetLeft = Vector3.Angle(-transform.right, directionToTarget);

                if (targetDirection.x < 0)
                    angleToTargetBackward = -angleToTargetBackward;
            }
            else
            {
                Vector3 directionToTarget = ((transform.position - new Vector3(0f, 0.75f, 0f)) - playerRef.position).normalized;
                int dotValue = System.Math.Sign(Vector3.Dot(-transform.right, directionToTarget));

                angleToTargetBackward = Vector3.Angle(-transform.forward, directionToTarget);
                angleToTargetLeft = Vector3.Angle(-transform.right, directionToTarget);

                if (dotValue < 0)
                    angleToTargetBackward = -angleToTargetBackward;
            }
        }
    }

    IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
    
    //적 탐지
    //설정한 각도 안에 적이 있을 경우 canSeePlayer를 true로 한다
    private void FieldOfViewCheck()
    {
        Collider[] playerCheck = Physics.OverlapSphere(transform.position, radius, playerMask);

        if (playerCheck.Length != 0)
        {
            Transform target = playerCheck[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            { 
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (Physics.Raycast(transform.position + new Vector3(0f, 1f, 0f), directionToTarget, distanceToTarget, obstructionMask))
                {
                    seeThroughPortal = false;
                    canSeePlayer = false;
                }
                else
                {
                    seeThroughPortal = false;
                    canSeePlayer = true;
                }
            }
            else
            {
                if (!portalPair.PlacedBothPortal())
                {
                    seeThroughPortal = false;
                    canSeePlayer = false;
                    return;
                }
                PlayerCheckThroughPortal();
            }
        }
        else
        {
            if (canSeePlayer)
                canSeePlayer = false;

            if (!portalPair.PlacedBothPortal())
                return;

            PlayerCheckThroughPortal();
        }
    }

    //포탈을 넘어서 적 탐지
    //범위 밖에 있는 경우 리스트에서 제거한다
    //포탈이 설정한 범위안에 있을 경우 포탈 건너편으로 적을 탐지한다
    //포탈이 범위 안에 두 개가 있을 경우 가장 터렛의 forward와 가까운 포탈 너머로 계산한다
    private void PlayerCheckThroughPortal()
    {
        Collider[] portalCheck = Physics.OverlapSphere(transform.position, radius, portalMask);
        portalCheckList.AddRange(portalCheck);

        if (portalCheckList.Count != 0)
        {
            for (int i = 0; i < portalCheckList.Count; ++i)
            {
                Vector3 directionToPortal = (portalCheckList[i].transform.position - relativeTurretPosition).normalized;

                if (!(Vector3.Angle(relativeTurretDirection, directionToPortal) < angle / 2))
                    portalCheckList.RemoveAt(i);
            }

            if (portalCheckList.Count == 0)
            {
                seeThroughPortal = false;
                canSeePlayer = false;
                return;
            }
            else if (portalCheckList.Count == 2)
            {
                Vector3 directionToPortal1 = (portalCheckList[0].transform.position - relativeTurretPosition).normalized;
                Vector3 directionToPortal2 = (portalCheckList[1].transform.position - relativeTurretPosition).normalized;
                float angle1 = Vector3.Angle(relativeTurretDirection, directionToPortal1);
                float angle2 = Vector3.Angle(relativeTurretDirection, directionToPortal2);

                if (Mathf.Abs(angle1) > Mathf.Abs(angle2))
                    portalCheckList.RemoveAt(0);
                else
                    portalCheckList.RemoveAt(1);
            }

            lookPortal = CSceneManager.Instance.portalPair.CheckPortalTag(portalCheckList[0].tag);
            relativeTurretPosition = lookPortal.GetOtherPortalRelativePoint(transform.position);

            Collider[] playerCheckOtherPortal = Physics.OverlapSphere(relativeTurretPosition, radius, playerMask);

            if (playerCheckOtherPortal.Length != 0)
            {
                Transform target = playerCheckOtherPortal[0].transform;
                Vector3 directionToTarget = (target.position - relativeTurretPosition).normalized;

                relativeTurretDirection = lookPortal.GetOtherPortalRelativeDirection(transform.forward);

                if (Vector3.Angle(relativeTurretDirection, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(relativeTurretPosition, target.position);
                    RaycastHit hit;
                    if (Physics.Raycast(relativeTurretPosition + new Vector3(0f, 1f, 0f), 
                        directionToTarget, out hit, distanceToTarget, portalMask))
                    {
                        if(hit.collider.CompareTag(lookPortal.tag))
                        {
                            seeThroughPortal = false;
                            canSeePlayer = false;
                        }
                        else
                        {
                            seeThroughPortal = true;
                            canSeePlayer = true;
                        }
                    }
                    else
                    {
                        seeThroughPortal = false;
                        canSeePlayer = false;
                    }
                }
                else
                {
                    seeThroughPortal = false;
                    canSeePlayer = false;
                }
            }
        }
        else
        {
            seeThroughPortal = false;
            canSeePlayer = false;
        }
    }
}
