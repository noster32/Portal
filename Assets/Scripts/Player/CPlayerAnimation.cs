using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerAnimation : CComponent
{
    #region component
    CPlayerMovement playerMovement;
    CPlayerData playerData;
    Animator chellAnimator;
    #endregion

    public override void Start()
    {
        base.Start();

        playerData = CPlayerData.GetInstance();
        playerMovement = GetComponent<CPlayerMovement>();
        chellAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        chellAnimator.SetFloat("DirectionN", playerMovement.moveVector.z, 0.5f, Time.deltaTime);
        chellAnimator.SetFloat("DirectionE", playerMovement.moveVector.x, 0.5f, Time.deltaTime);

        if(playerData.GetDrawPortalGun())
            AnimationChangeDrawPortalGun();
        else
            AnimationChange();

        if(playerMovement.grapicsClone && playerMovement.grapicsClone.activeSelf && playerMovement.originalAnimator && playerMovement.cloneAnimator)
        {
            if (playerData.GetDrawPortalGun())
                AnimationChangeDrawPortalGun(playerMovement.cloneAnimator);
            else
                AnimationChange(playerMovement.cloneAnimator);
        }
    }

    private void AnimationChange()
    {
        switch (playerData.GetPlayerState())
        {
            case CPlayerData.PlayerState.IDLE:
                chellAnimator.CrossFade("IDLE", 0.03f);
                break;
            case CPlayerData.PlayerState.WALK:
                chellAnimator.Play("WalkBT", 0);
                break;
            case CPlayerData.PlayerState.JUMP:
                chellAnimator.Play("Jump", 0);
                break;
            case CPlayerData.PlayerState.CROUCH:
                chellAnimator.CrossFade("Crouch IDLE", 0.03f);
                break;
            case CPlayerData.PlayerState.CROUCHWALK:
                chellAnimator.Play("CrouchBT", 0);
                break;
            default:
                chellAnimator.CrossFade("IDLE", 0.03f);
                break;
        }
    }
    
    public void AnimationChange(Animator animator)
    {
        switch (playerData.GetPlayerState())
        {
            case CPlayerData.PlayerState.IDLE:
                animator.CrossFade("IDLE", 0.03f);
                break;
            case CPlayerData.PlayerState.WALK:
                animator.Play("WalkBT", 0);
                break;
            case CPlayerData.PlayerState.JUMP:
                animator.Play("Jump", 0);
                break;
            case CPlayerData.PlayerState.CROUCH:
                animator.CrossFade("Crouch IDLE", 0.03f);
                break;
            case CPlayerData.PlayerState.CROUCHWALK:
                animator.Play("CrouchBT", 0);
                break;
            default:
                animator.CrossFade("IDLE", 0.03f);
                break;
        }
    }

    private void AnimationChangeDrawPortalGun()
    {
        switch (playerData.GetPlayerState())
        {
            case CPlayerData.PlayerState.IDLE:
                chellAnimator.CrossFade("Idle_PortalGun", 0.03f);
                break;
            case CPlayerData.PlayerState.WALK:
                chellAnimator.Play("WalkBT_PortalGun", 0);
                break;
            case CPlayerData.PlayerState.JUMP:
                chellAnimator.Play("Jump_PortalGun", 0);
                break;
            case CPlayerData.PlayerState.CROUCH:
                chellAnimator.CrossFade("Crouch_Idle_PortalGun", 0.03f);
                break;
            case CPlayerData.PlayerState.CROUCHWALK:
                chellAnimator.Play("CrouchBT_PortalGun", 0);
                break;
            default:
                chellAnimator.CrossFade("Idle_PortalGun", 0.03f);
                break;
        }
    }

    private void AnimationChangeDrawPortalGun(Animator animator)
    {
        switch (playerData.GetPlayerState())
        {
            case CPlayerData.PlayerState.IDLE:
                animator.CrossFade("Idle_PortalGun", 0.03f);
                break;
            case CPlayerData.PlayerState.WALK:
                animator.Play("WalkBT_PortalGun", 0);
                break;
            case CPlayerData.PlayerState.JUMP:
                animator.Play("Jump_PortalGun", 0);
                break;
            case CPlayerData.PlayerState.CROUCH:
                animator.CrossFade("Crouch_Idle_PortalGun", 0.03f);
                break;
            case CPlayerData.PlayerState.CROUCHWALK:
                animator.Play("CrouchBT_PortalGun", 0);
                break;
            default:
                animator.CrossFade("Idle_PortalGun", 0.03f);
                break;
        }
    }
}
