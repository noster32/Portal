using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CInteractObject : CComponent
{
    CInteractEvent interact = new CInteractEvent();
    CPlayerInteraction player;

    public CInteractEvent GetInteractEvent
    {
        get
        {
            if(interact == null)
                interact = new CInteractEvent();

            return interact;
        }
    }

    public void CallInteract(CPlayerInteraction interactedPlayer)
    {
        player = interactedPlayer;
        interact.CallInteractEvent();
    }
}

public class CInteractEvent
{
    public delegate void InteractHandler();

    public event InteractHandler HasInteracted;

    public void CallInteractEvent() => HasInteracted?.Invoke();
}
