using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletSoundSpawn : CPoolingManager<CBulletSound>
{
    [SerializeField] private CBulletSound bulletSoundPrefab;
    [SerializeField] private Transform parent;

    public override void Start()
    {
        base.Start();

        InitPool(bulletSoundPrefab, 50, 100, false);
    }

    public void PlayBulletHitConcretSound(Vector3 pos)
    {
        var bulletSound = Get();
        bulletSound.transform.position = pos;
        if(parent != null)
            bulletSound.transform.parent = parent;
        bulletSound.DestroyBulletSound(KillBulletSound);

        StartCoroutine(bulletSound.PlayHitConcretSound()); 
    }

    public void PlayBulletHitMetalSound(Vector3 pos)
    {
        var bulletSound = Get();
        bulletSound.transform.position = pos;
        if (parent != null)
            bulletSound.transform.parent = parent;
        bulletSound.DestroyBulletSound(KillBulletSound);

        StartCoroutine(bulletSound.PlayHitMetalSound());
    }

    public void PlayBulletHitGlassSound(Vector3 pos)
    {
        var bulletSound = Get();
        bulletSound.transform.position = pos;
        if (parent != null)
            bulletSound.transform.parent = parent;
        bulletSound.DestroyBulletSound(KillBulletSound);

        StartCoroutine(bulletSound.PlayHitGlassSound());
    }

    private void KillBulletSound(CBulletSound bulletSound)
    {
        PoolRelease(bulletSound);
    }
}
