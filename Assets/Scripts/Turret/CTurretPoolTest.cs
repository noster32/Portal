using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CTurretPoolTest : CComponent
{
    [SerializeField] int spawnAmount = 20;
    [SerializeField] CTurretPoolTest2 cbullet;
    [SerializeField] Transform parent;

    private ObjectPool<CTurretPoolTest2> pool;

    public override void Start()
    {
        base.Start();

       
        pool = new ObjectPool<CTurretPoolTest2>(() =>
        {
            return Instantiate(cbullet);
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, false, 50, 100);

        InvokeRepeating(nameof(Spawn), 0.2f, 0.2f);
    }

    private void Spawn()
    {
        for(var i = 0; i < spawnAmount; i++)
        {
            var bullet = pool.Get();
            Vector3 newVelo = Vector3.zero;

            bullet.transform.parent = parent;
            bullet.transform.position = new Vector3(0f, 50f, 0f);
            bullet.Init(KillBullet);
        }
    }

    private void KillBullet(CTurretPoolTest2 bullet)
    {
        pool.Release(bullet);
    }
}
