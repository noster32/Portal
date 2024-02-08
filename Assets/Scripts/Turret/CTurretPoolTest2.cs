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
        {
            if (_killAction != null)
                _killAction(this);
            else
                Destroy(this.gameObject);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(this.transform.position.y < -50f)
        {
            if (_killAction != null)
                _killAction(this);
            else
                Destroy(this.gameObject);
        }
    }

}
