using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerAnimation_Controller : CComponent
{
    CPlayerMovementController tPlayerMoveCon;
    Animator chellAnimator;

    public override void Start()
    {
        base.Start();

        tPlayerMoveCon = GetComponent<CPlayerMovementController>();
        chellAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        chellAnimator.SetFloat("DirectionN", tPlayerMoveCon.moveDir.z, 0.5f, Time.deltaTime);
        chellAnimator.SetFloat("DirectionE", tPlayerMoveCon.moveDir.x, 0.5f, Time.deltaTime);


    }

    private void AnimationChange()
    {
        if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.IDLE)
        {
            chellAnimator.CrossFade("IDLE", 0.03f);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.WALK)
        {
            chellAnimator.Play("WalkBT", 0);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.JUMP)
        {
            chellAnimator.Play("Jump", 0);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.CROUCH)
        {
            chellAnimator.CrossFade("Crouch IDLE", 0.03f);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.CROUCHWALK)
        {
            chellAnimator.Play("CrouchBT", 0);
        }
    }
    public void AnimationChange(Animator animator)
    {
        if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.IDLE)
        {
            animator.CrossFade("IDLE", 0.03f);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.WALK)
        {
            animator.Play("WalkBT", 0);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.JUMP)
        {
            animator.Play("Jump", 0);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.CROUCH)
        {
            animator.CrossFade("Crouch IDLE", 0.03f);
        }
        else if (tPlayerMoveCon.pState == CPlayerMovementController.PlayerState.CROUCHWALK)
        {
            animator.Play("CrouchBT", 0);
        }
    }
}