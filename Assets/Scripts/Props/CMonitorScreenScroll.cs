using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMonitorScreenScroll : CComponent
{
    [SerializeField] private float scrollSpeed = -0.2f;
    [SerializeField] private Renderer monitorRenderer;

    public override void Update()
    {
        base.Update();

        float offsetY = Time.time * scrollSpeed;
        monitorRenderer.materials[1].mainTextureOffset = new Vector2(0f, offsetY);
    }
}
