using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTurretPoolTest2 : CComponent
{
    private Action<CTurretPoolTest2> _killAction;
    
    public void Init(Action<CTurretPoolTest2> killAction)
    {
        _killAction = killAction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Concret"))
            _killAction(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(_killAction != null && this.transform.position.y < -50f)
            _killAction(this);
    }
}
