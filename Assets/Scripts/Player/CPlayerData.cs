using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerData : CComponent
{
    public bool isDrawPortalGun = false;
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

    public PlayerState pState;

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

    public static CPlayerData GetInstance()
    {
        return instance;
    }
}
