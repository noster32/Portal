using System.Collections;
using UnityEngine;

public class CEnemyFieldOfView : CComponent
{
    #region public

    public float radius;

    [Range(0, 360)] public float angle;

    public GameObject playerRef;

    public LayerMask playerMask;
    public LayerMask portalMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public bool seeThroughPortal;

    public float angleToTarget;

    [SerializeField] CPortalPair portalPair;
    #endregion


    private Vector3 relativeTurretPosition;
    private Vector3 relativeTurretDirection;

    private CPortal nearPortal;
    public override void Start()
    {
        base.Start();

        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());

    }
    public override void Update()
    {
        base.Update();

        if(canSeePlayer)
        {
            if(seeThroughPortal)
            {
                Vector3 relativePos = nearPortal.otherPortal.transform.InverseTransformPoint(playerRef.transform.position);
                relativePos = Quaternion.Euler(0f, 180f, 0f) * relativePos;
                Vector3 relativePlayerPos = nearPortal.transform.TransformPoint(relativePos);

                Vector3 directionToTarget = (transform.position - relativePlayerPos).normalized;
                Vector3 targetDirection = transform.InverseTransformDirection(relativePlayerPos - transform.position);

                //Debug.DrawLine(transform.position, relativePlayerPos, Color.blue, 2f);

                angleToTarget = Vector3.Angle(-transform.forward, directionToTarget);
                if (targetDirection.x < 0)
                {
                    angleToTarget = -angleToTarget;
                }
            }
            else
            {
                Vector3 directionToTarget = (transform.position - playerRef.transform.position).normalized;
                Vector3 targetDirection = transform.InverseTransformDirection(playerRef.transform.position - transform.position);

                angleToTarget = Vector3.Angle(-transform.forward, directionToTarget);
                if (targetDirection.x < 0)
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

        if (portalCheck.Length != 0)
        {
            float distance0 = Vector3.Distance(transform.position, portalPair.portals[0].transform.position);
            float distance1 = Vector3.Distance(transform.position, portalPair.portals[1].transform.position);

            nearPortal = distance0 < distance1 ? portalPair.portals[0] : portalPair.portals[1];

            Vector3 relativePos = nearPortal.transform.InverseTransformPoint(transform.position);
            relativePos = Quaternion.Euler(0f, 180f, 0f) * relativePos;
            relativeTurretPosition = nearPortal.otherPortal.transform.TransformPoint(relativePos);

            Collider[] playerCheckOtherPortal = Physics.OverlapSphere(relativeTurretPosition, radius, playerMask);

            if (playerCheckOtherPortal.Length != 0)
            {
                Transform target = playerCheckOtherPortal[0].transform;
                Vector3 directionToTarget = (target.position - relativeTurretPosition).normalized;

                Vector3 relativeDir = nearPortal.transform.InverseTransformDirection(transform.forward);
                relativeDir = Quaternion.Euler(0f, 180f, 0f) * relativeDir;
                relativeTurretDirection = nearPortal.otherPortal.transform.TransformDirection(relativeDir);

                if (Vector3.Angle(relativeTurretDirection, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(relativeTurretPosition, target.position);
                    if (Physics.Raycast(relativeTurretPosition + new Vector3(0f, 1f, 0f), directionToTarget, distanceToTarget, portalMask))
                    {
                        seeThroughPortal = true;
                        canSeePlayer = true;
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
