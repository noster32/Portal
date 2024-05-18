using UnityEngine;

public class CPlayerAnimation : CComponent
{
    CPlayerMovement playerMovement;
    CPlayerState playerState;
    Animator chellAnimator;

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

        PlayAnimation();
    }

    //애니메이션 재생
    //포탈건의 보유 여부에 따라서 애니메이션이 달라짐
    //클론의 경우 active됐을 때 애니메이션을 재생한다
    public void PlayAnimation()
    {
        string animationPrefix = (playerState.GetDrawPortalGun()) ? "_PortalGun" : "";

        switch (playerState.GetPlayerState())
        {
            case CPlayerState.PlayerState.IDLE:
                chellAnimator.CrossFade("Idle" + animationPrefix, 0.03f);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.CrossFade("Idle" + animationPrefix, 0.03f);
                break;
            case CPlayerState.PlayerState.WALK:
                chellAnimator.Play("WalkBT" + animationPrefix, 0);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.Play("WalkBT" + animationPrefix, 0);
                break;
            case CPlayerState.PlayerState.JUMP:
                chellAnimator.Play("Jump" + animationPrefix, 0);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.Play("Jump" + animationPrefix, 0);
                break;
            case CPlayerState.PlayerState.CROUCH:
                chellAnimator.CrossFade("Crouch_Idle" + animationPrefix, 0.03f);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.CrossFade("Crouch_Idle" + animationPrefix, 0.03f);
                break;
            case CPlayerState.PlayerState.CROUCHWALK:
                chellAnimator.Play("CrouchBT" + animationPrefix, 0);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.Play("CrouchBT" + animationPrefix, 0);
                break;
            default:
                chellAnimator.CrossFade("Idle" + animationPrefix, 0.03f);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.CrossFade("Idle" + animationPrefix, 0.03f);
                break;
        }
    }

    public void PlayAnimation(string state)
    {
        string animationPrefix = (playerState.GetDrawPortalGun()) ? "_PortalGun" : "";

        switch (state)
        {
            case "idle":
                chellAnimator.CrossFade("Idle" + animationPrefix, 0.03f);
                if(playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.CrossFade("Idle" + animationPrefix, 0.03f);
                break;
            case "Walk":
                chellAnimator.Play("WalkBT" + animationPrefix, 0);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.Play("WalkBT" + animationPrefix, 0);
                break;
            case "Jump":
                chellAnimator.Play("Jump" + animationPrefix, 0);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.Play("Jump" + animationPrefix, 0);
                break;
            case "Crouch":
                chellAnimator.CrossFade("Crouch_Idle" + animationPrefix, 0.03f);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.CrossFade("Crouch_Idle" + animationPrefix, 0.03f);
                break;
            case "CrouchWalk":
                chellAnimator.Play("CrouchBT" + animationPrefix, 0);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.Play("CrouchBT" + animationPrefix, 0);
                break;
            default:
                chellAnimator.CrossFade("Idle" + animationPrefix, 0.03f);
                if (playerMovement.grapicsClone.activeSelf)
                    playerMovement.cloneAnimator?.CrossFade("Idle" + animationPrefix, 0.03f);
                break;
        }
    }

}
