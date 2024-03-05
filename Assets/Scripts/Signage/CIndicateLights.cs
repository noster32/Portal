using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIndicateLights : CComponent 
{
    private Renderer indicateRenderer;
    private Material material;

    [SerializeField] private Texture onTexture;
    [SerializeField] private Texture offTexture;

    public override void Awake()
    {
        base.Awake();

        indicateRenderer = GetComponent<Renderer>();
        material = indicateRenderer.material;
    }

    public void ButtonOn()
    {
        material.mainTexture = onTexture;
    }

    public void ButtonOff()
    {
        material.mainTexture = offTexture;
    }

}
