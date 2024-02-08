using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerStateManager : CComponent
{
    private static CPlayerStateManager instance;
    public CPlayerState playerState;

    public override void Awake()
    {
        base.Awake();

        if(!instance)
        {
            instance = this;
            playerState = new CPlayerState();
        }
        else
        {
            Debug.LogError("Already have StateManager!"); 
        }
    }

    public static CPlayerStateManager GetInstance()
    {
        return instance;
    }
}
