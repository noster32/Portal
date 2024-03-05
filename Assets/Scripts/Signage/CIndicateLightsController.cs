using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIndicateLightsController : CComponent
{
    [SerializeField] private CIndicateLights[] indicateLights;

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
