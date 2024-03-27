using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CMaterialScroll : CComponent
{
    [SerializeField] private bool scrollX = false;
    [SerializeField] private bool scrollY = true;
    [SerializeField] private float scrollSpeed = -0.2f;
    [SerializeField] private Renderer scrollRenderer;
    private Material material;

    public override void Awake()
    {
        base.Awake();

        material = scrollRenderer.material;
    }

    public override void Update()
    {
        base.Update();
        if(scrollX && scrollY) 
        {
            float offset = Time.time * scrollSpeed;
            material.mainTextureOffset = new Vector2(offset, offset);
        }
        else
        {
            if (scrollX)
            {
                float offsetX = Time.time * scrollSpeed;
                material.mainTextureOffset = new Vector2(offsetX, 0f);
            }
            else if (scrollY)
            {
                float offsetY = Time.time * scrollSpeed;
                material.mainTextureOffset = new Vector2(0f, offsetY);
            }
            else
            {
                Debug.LogWarning("At least one offset value must be set" + this.gameObject.name);
            }
        }

    }
}
