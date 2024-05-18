using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletHole : CComponent
{
    private MeshRenderer m_renderer;
    private System.Random m_random;

    public ObjectMaterial surfaceMaterial;
    [SerializeField] private Texture[] bulletTextures;
    public override void Awake()
    {
        base.Awake();

        m_renderer = GetComponent<MeshRenderer>();
        m_random = new System.Random();
    }

    private void OnEnable()
    {
        SetRandomTexture();
        Destroy(gameObject, 30f);
    }

    private void SetRandomTexture()
    {
        int randomNum = m_random.Next(bulletTextures.Length);

        m_renderer.material.mainTexture = bulletTextures[randomNum];
    }
}
