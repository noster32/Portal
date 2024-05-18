using System.Collections;
using UnityEngine;

public class CPlayerState : CComponent
{
    [SerializeField] private bool isDrawBluePortalGun = false;          //포탈건B 보유
    [SerializeField] private bool isDrawOrangePortalGun = false;        //포탈건O 보유
    [SerializeField] private int playerHp = 100;                
    [SerializeField] private float damageDelay = 0.3f;                  //데미지 딜레이
    private bool isGrab = false;                                        //그랩 여부
    private bool isDie = false;             
    private bool isDamageDelay = false;                                 //딜레이 진행중일 경우 true
    private int endingHp = 2;                                           //엔딩씬 hp
    private bool isEnding = false;                                      //엔딩용

    public CGrabbableObject grabObject = null;                           //플레이어가 잡고있는 오브젝트

    [SerializeField] private CCameraFade cameraFade;
    private CPlayerMouseLook playerAimPunch;
    private CPlayerDie playerDie;
    private Coroutine autoHealCoroutine;
    public enum PlayerState
    {
        IDLE,
        WALK,
        JUMP,
        CROUCH,
        CROUCHWALK,
        DIE
    }

    private PlayerState pState;

    public override void Awake()
    {
        base.Awake();
        
        playerAimPunch = GetComponent<CPlayerMouseLook>();
        playerDie = GetComponent<CPlayerDie>();
    }

    public override void Update()
    {
        base.Update();
        
        if (!isDie && playerHp < 0)
        {
            isDie = true;
            playerDie.PlayerSpawnDeadCamera();
        }

        Debug.Log(pState);
    }

    #region PortalGun
    public bool GetIsDrawBluePortalGun() => isDrawBluePortalGun;

    public bool GetIsDrawOrangePortalGun() => isDrawOrangePortalGun;

    public void SetDrawBluePortalGun() => isDrawBluePortalGun = true;

    public void SetDrawOrangePortlaGun() => isDrawOrangePortalGun = true;

    public bool GetDrawPortalGun() => (isDrawBluePortalGun || isDrawOrangePortalGun);

    public bool GetDrawBothPortalGun() => (isDrawBluePortalGun && isDrawOrangePortalGun);
    #endregion

    #region AnimationState
    public PlayerState GetPlayerState() => pState;

    public void SetPlayerState(PlayerState state) => pState = state;
    #endregion

    public bool GetIsGrab() => isGrab;

    public void SetIsGrab(bool isGrab) => this.isGrab = isGrab;

    public bool GetIsPlayerDie() => isDie;

    public bool SetIsPlayerDIe() => isDie = true;

    public bool GetIsEnding() => isEnding;
    public int GetPlayerHp() => playerHp;
    public int GetEndingHp() => endingHp;

    public void AimPunchPlayer()
    {
        playerAimPunch.AimPunch(-5f);
    }

    public void DealEndingDamageToPlayer()
    {
        endingHp -= 1;
        if(endingHp <= 0)
        {
            isEnding = true;
            
        }
        else
        {
            playerAimPunch.AimPunch(-5f);
            cameraFade.FadeIn(3f, 0.6f);
        }
    }

    public void DealDamageToPlayer(int damage)
    {
        if (!isDamageDelay)
        {
            if (autoHealCoroutine != null)
                StopCoroutine(autoHealCoroutine);

            playerHp -= damage;
            playerAimPunch.AimPunch(-5f);
            cameraFade.FadeIn(3f, 0.6f);

            StartCoroutine(ResetDamageDelay());
        }
    }

    public void DealDamageToPlayer(int damage, Vector3 direction, float knockback)
    {
        if(!isDamageDelay)
        {
            if (autoHealCoroutine != null)
                StopCoroutine(autoHealCoroutine);

            isDamageDelay = true;
            playerAimPunch.AimPunch(-5f);
            m_oRigidBody.AddForce(direction * knockback, ForceMode.Impulse);
            playerHp -= damage;
            cameraFade.FadeIn(3f, 0.6f);
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.bodyMediumImpactHard, this.transform.position);

            StartCoroutine(ResetDamageDelay());
        }
    }

    private void AutoHeal()
    {
        if(playerHp != 100 && playerHp > 0)
        {
            autoHealCoroutine = StartCoroutine(AutoHealCoroutne());
        }
    }

    private IEnumerator AutoHealCoroutne()
    {
        yield return new WaitForSeconds(5f);

        playerHp = 100;

        yield return null;
    }

    private IEnumerator ResetDamageDelay()
    {
        yield return new WaitForSeconds(damageDelay);
        isDamageDelay = false;
    }
}
