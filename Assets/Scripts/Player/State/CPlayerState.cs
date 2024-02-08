using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class CPlayerState
{
    public bool isGrab;


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
}
