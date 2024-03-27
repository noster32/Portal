using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIndicateLightsController : CComponent
{
    private CIndicateLights[] indicateLights;


    public override void Start()
    {
        base.Start();

        indicateLights = transform.GetComponentsInChildren<CIndicateLights>();
        Debug.Log(indicateLights.Length);
    }
    public void OnIndicateLights()
    {
        foreach (var indicateLight in indicateLights)
        {
            indicateLight.ButtonOn();
        }
    }

    public void OffIndicateLights()
    {
        foreach (var indicateLight in indicateLights)
        {
            indicateLight.ButtonOff();
        }
    }

}
