using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCameraCullingMask : CComponent
{
    LayerMask cullingMask;


    public override void Start()
    {
        base.Start();

        cullingMask = ~(1 << LayerMask.NameToLayer("InvisiblePlayer"));
    }

    public override void Update()
    {
        base.Update();

        Camera.main.cullingMask = cullingMask;
    }
}
