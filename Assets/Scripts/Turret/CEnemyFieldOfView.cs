using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyFieldOfView : CComponent
{
    #region public

    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public float AngleToTarget;
    #endregion

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
            Vector3 directionToTarget = (transform.position - playerRef.transform.position).normalized;
            Vector3 targetDirection = transform.InverseTransformDirection(playerRef.transform.position - transform.position);

            AngleToTarget = Vector3.Angle(-transform.forward, directionToTarget);
            if (targetDirection.x < 0)
            {
                AngleToTarget = -AngleToTarget;
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
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            { 
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;

        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
