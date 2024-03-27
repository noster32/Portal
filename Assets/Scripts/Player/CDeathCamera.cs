using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDeathCamera : CComponent
{
    private CPlayerMouseLook mouseLook;

    public override void Awake()
    {
        base.Awake();

        mouseLook = GetComponent<CPlayerMouseLook>();
    }
    public override void Start()
    {
        base.Start();

        mouseLook.Init(this.transform);
    }

    public override void Update()
    {
        base.Update();

        mouseLook.MouseRotation(this.transform);
    }
}
