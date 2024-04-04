using System.Collections;
using UnityEngine;

public class CEnemyFieldOfView : CComponent
{
    #region public

    public float radius;

    [Range(0, 360)] public float angle;

    public Transform playerRef;

    public LayerMask playerMask;
    public LayerMask portalMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public bool seeThroughPortal;

    public float angleToTarget;

    [SerializeField] private CPortalPair portalPair;
    #endregion


    private Vector3 relativeTurretPosition;
    private Vector3 relativeTurretDirection;

    private CPortal nearPortal;
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

        if(canSeePlayer)
        {
            if(seeThroughPortal)
            {
                Vector3 relativePlayerPos = nearPortal.otherPortal.GetOtherPortalRelativePoint(playerRef.position);
                Vector3 directionToTarget = ((transform.position - new Vector3(0f, 0.75f, 0f)) - relativePlayerPos).normalized;
                Vector3 targetDirection = transform.InverseTransformDirection(relativePlayerPos - transform.position);

                angleToTarget = Vector3.Angle(-transform.forward, directionToTarget);
                if (targetDirection.x < 0)
                {
                    angleToTarget = -angleToTarget;
                }
            }
            else
            {
                Vector3 directionToTarget = ((transform.position - new Vector3(0f, 0.75f, 0f)) - playerRef.position).normalized;
                int dotValue = System.Math.Sign(Vector3.Dot(-transform.right, directionToTarget));

                angleToTarget = Vector3.Angle(-transform.forward, directionToTarget);
                if (dotValue < 0)
                {
                    angleToTarget = -angleToTarget;
                }
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

    private void PlayerCheckThroughPortal()
    {
        Collider[] portalCheck = Physics.OverlapSphere(transform.position, radius, portalMask);

        //near포탈로 하면 안될듯 사야에있는 포탈로 해야될거 같은데
        //near포탈로하면 시야안에있는 포탈이 멀고 플레이어 근처에있는 포탈이 가까울때 먹통이 됨
        if (portalCheck.Length != 0)
        {
            float distance0 = Vector3.Distance(transform.position, portalPair.portals[0].transform.position);
            float distance1 = Vector3.Distance(transform.position, portalPair.portals[1].transform.position);

            nearPortal = distance0 < distance1 ? portalPair.portals[0] : portalPair.portals[1];

            relativeTurretPosition = nearPortal.GetOtherPortalRelativePoint(transform.position);

            Collider[] playerCheckOtherPortal = Physics.OverlapSphere(relativeTurretPosition, radius, playerMask);

            if (playerCheckOtherPortal.Length != 0)
            {
                Transform target = playerCheckOtherPortal[0].transform;
                Vector3 directionToTarget = (target.position - relativeTurretPosition).normalized;

                relativeTurretDirection = nearPortal.GetOtherPortalRelativeDirection(transform.forward);

                if (Vector3.Angle(relativeTurretDirection, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(relativeTurretPosition, target.position);
                    RaycastHit hit;
                    if (Physics.Raycast(relativeTurretPosition + new Vector3(0f, 1f, 0f), directionToTarget, out hit, distanceToTarget, portalMask))
                    {
                        if(hit.collider.tag == nearPortal.tag)
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
