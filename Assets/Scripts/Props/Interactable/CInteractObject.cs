public class CInteractObject : CComponent
{
    CInteractEvent interact = new CInteractEvent();

    public CInteractEvent GetInteractEvent
    {
        get
        {
            if(interact == null)
                interact = new CInteractEvent();

            return interact;
        }
    }

    public void CallInteract()
    {
        interact.CallInteractEvent();
    }
}

public class CInteractEvent
{
    public delegate void InteractHandler();

    public event InteractHandler HasInteracted;

    public void CallInteractEvent() => HasInteracted?.Invoke();
}
