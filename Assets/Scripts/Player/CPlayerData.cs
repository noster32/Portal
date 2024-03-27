using UnityEngine;

public class CPlayerData : CComponent
{
    [SerializeField] private bool isDrawBluePortalGun = false;
    [SerializeField] private bool isDrawOrangePortalGun = false;
    public bool isGrab = false;
    public CGrabableObject grabObject = null;

    public enum PlayerState
    {
        IDLE,
        WALK,
        JUMP,
        CROUCH,
        CROUCHWALK,
        FALL,
        DIE
    }

    private PlayerState pState;

    private static CPlayerData instance;

    public override void Awake()
    {
        base.Awake();

        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already have PlayerData");
        }
    }

    public static CPlayerData GetInstance() => instance;

    public bool GetIsDrawBluePortalGun() => isDrawBluePortalGun;

    public bool GetIsDrawOrangePortalGun() => isDrawOrangePortalGun;

    public void SetDrawBluePortalGun() => isDrawBluePortalGun = true;

    public void SetDrawOrangePortlaGun() => isDrawOrangePortalGun = true;

    public bool GetDrawPortalGun() => (isDrawBluePortalGun || isDrawOrangePortalGun);

    public bool GetDrawBothPortalGun() => (isDrawBluePortalGun && isDrawOrangePortalGun);

    public PlayerState GetPlayerState() => pState;

    public void SetPlayerState(PlayerState state) => pState = state;
}
