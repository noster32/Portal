using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerAnimation : CComponent
{
    #region component
    CPlayerMovement tPlayerMove;
    Animator chellAnimator;
    #endregion

    public override void Start()
    {
        base.Start();

        tPlayerMove = GetComponent<CPlayerMovement>();
        chellAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        chellAnimator.SetFloat("DirectionN", tPlayerMove.moveVector.z, 0.5f, Time.deltaTime);
        chellAnimator.SetFloat("DirectionE", tPlayerMove.moveVector.x, 0.5f, Time.deltaTime);

        AnimationChange();
        if(tPlayerMove.grapicsClone && tPlayerMove.grapicsClone.activeSelf && tPlayerMove.originalAnimator && tPlayerMove.cloneAnimator)
        {
            AnimationChange(tPlayerMove.cloneAnimator);
        }
    }

    private void AnimationChange()
    {

        if (tPlayerMove.pState == CPlayerMovement.PlayerState.IDLE)
        {
            chellAnimator.CrossFade("IDLE", 0.03f);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.WALK)
        {
            chellAnimator.Play("WalkBT", 0);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.JUMP)
        {
            chellAnimator.Play("Jump", 0);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.CROUCH)
        {
            chellAnimator.CrossFade("Crouch IDLE", 0.03f);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.CROUCHWALK)
        {
            chellAnimator.Play("CrouchBT", 0);
        }
    }
    
    public void AnimationChange(Animator animator)
    {
        if (tPlayerMove.pState == CPlayerMovement.PlayerState.IDLE)
        {
            animator.CrossFade("IDLE", 0.03f);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.WALK)
        {
            animator.Play("WalkBT", 0);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.JUMP)
        {
            animator.Play("Jump", 0);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.CROUCH)
        {
            animator.CrossFade("Crouch IDLE", 0.03f);
        }
        else if (tPlayerMove.pState == CPlayerMovement.PlayerState.CROUCHWALK)
        {
            animator.Play("CrouchBT", 0);
        }
    }
}
