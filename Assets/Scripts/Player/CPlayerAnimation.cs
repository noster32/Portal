using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerAnimation : CComponent
{
    #region component
    CPlayerMovement playerMovement;
    CPlayerState playerState;
    Animator chellAnimator;
    #endregion

    public override void Awake()
    {
        base.Awake();

        playerState = GetComponent<CPlayerState>();
        playerMovement = GetComponent<CPlayerMovement>();
        chellAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        if (playerState.GetIsPlayerDie())
            return;

        chellAnimator.SetFloat("DirectionN", playerMovement.moveVector.z, 0.5f, Time.deltaTime);
        chellAnimator.SetFloat("DirectionE", playerMovement.moveVector.x, 0.5f, Time.deltaTime);

        if(playerState.GetDrawPortalGun())
            AnimationChangeDrawPortalGun();
        else
            AnimationChange();

        if(playerMovement.grapicsClone && playerMovement.grapicsClone.activeSelf && playerMovement.originalAnimator && playerMovement.cloneAnimator)
        {
            if (playerState.GetDrawPortalGun())
                AnimationChangeDrawPortalGun(playerMovement.cloneAnimator);
            else
                AnimationChange(playerMovement.cloneAnimator);
        }
    }

    private void AnimationChange()
    {
        switch (playerState.GetPlayerState())
        {
            case CPlayerState.PlayerState.IDLE:
                chellAnimator.CrossFade("IDLE", 0.03f);
                break;
            case CPlayerState.PlayerState.WALK:
                chellAnimator.Play("WalkBT", 0);
                break;
            case CPlayerState.PlayerState.JUMP:
                chellAnimator.Play("Jump", 0);
                break;
            case CPlayerState.PlayerState.CROUCH:
                chellAnimator.CrossFade("Crouch IDLE", 0.03f);
                break;
            case CPlayerState.PlayerState.CROUCHWALK:
                chellAnimator.Play("CrouchBT", 0);
                break;
            default:
                chellAnimator.CrossFade("IDLE", 0.03f);
                break;
        }
    }
    
    public void AnimationChange(Animator animator)
    {
        switch (playerState.GetPlayerState())
        {
            case CPlayerState.PlayerState.IDLE:
                animator.CrossFade("IDLE", 0.03f);
                break;
            case CPlayerState.PlayerState.WALK:
                animator.Play("WalkBT", 0);
                break;
            case CPlayerState.PlayerState.JUMP:
                animator.Play("Jump", 0);
                break;
            case CPlayerState.PlayerState.CROUCH:
                animator.CrossFade("Crouch IDLE", 0.03f);
                break;
            case CPlayerState.PlayerState.CROUCHWALK:
                animator.Play("CrouchBT", 0);
                break;
            default:
                animator.CrossFade("IDLE", 0.03f);
                break;
        }
    }

    private void AnimationChangeDrawPortalGun()
    {
        switch (playerState.GetPlayerState())
        {
            case CPlayerState.PlayerState.IDLE:
                chellAnimator.CrossFade("Idle_PortalGun", 0.03f);
                break;
            case CPlayerState.PlayerState.WALK:
                chellAnimator.Play("WalkBT_PortalGun", 0);
                break;
            case CPlayerState.PlayerState.JUMP:
                chellAnimator.Play("Jump_PortalGun", 0);
                break;
            case CPlayerState.PlayerState.CROUCH:
                chellAnimator.CrossFade("Crouch_Idle_PortalGun", 0.03f);
                break;
            case CPlayerState.PlayerState.CROUCHWALK:
                chellAnimator.Play("CrouchBT_PortalGun", 0);
                break;
            default:
                chellAnimator.CrossFade("Idle_PortalGun", 0.03f);
                break;
        }
    }

    private void AnimationChangeDrawPortalGun(Animator animator)
    {
        switch (playerState.GetPlayerState())
        {
            case CPlayerState.PlayerState.IDLE:
                animator.CrossFade("Idle_PortalGun", 0.03f);
                break;
            case CPlayerState.PlayerState.WALK:
                animator.Play("WalkBT_PortalGun", 0);
                break;
            case CPlayerState.PlayerState.JUMP:
                animator.Play("Jump_PortalGun", 0);
                break;
            case CPlayerState.PlayerState.CROUCH:
                animator.CrossFade("Crouch_Idle_PortalGun", 0.03f);
                break;
            case CPlayerState.PlayerState.CROUCHWALK:
                animator.Play("CrouchBT_PortalGun", 0);
                break;
            default:
                animator.CrossFade("Idle_PortalGun", 0.03f);
                break;
        }
    }
}
