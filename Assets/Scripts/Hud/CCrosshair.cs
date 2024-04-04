using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCrosshair : CComponent
{
    [SerializeField] private Texture2D originalTexture;
    [SerializeField] private Rect cutRect;
    private Image crosshairImage;

    public override void Start()
    {
        base.Start();

        crosshairImage = GetComponent<Image>();

        if (crosshairImage)
        {
            Sprite sprite = Sprite.Create(originalTexture, cutRect, new Vector2(0.5f, 0.5f));

            crosshairImage.sprite = sprite;
        }
    }
}
