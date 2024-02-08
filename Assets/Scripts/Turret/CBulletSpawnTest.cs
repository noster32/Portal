using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletSpawnTest : CPoolingManager<CTurretPoolTest2>
{

    [SerializeField] private Transform turretGunL;
    [SerializeField] private Transform turretGunR;

    #region private
    [SerializeField] private CTurretPoolTest2 bulletSpawnTestPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private int spawnAmount;
    #endregion

    public override void Start()
    {
        base.Start();

        InitPool(bulletSpawnTestPrefab, 50, 100, false);

    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            this.SpawnBullet();
        }
    }

    private void SpawnBullet()
    {
        var bulletLeft = Get();
        var bulletRight = Get();
        Quaternion bulletRotation = Quaternion.identity;

        bulletLeft.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bulletRight.GetComponent<Rigidbody>().velocity = Vector3.zero;

        bulletLeft.transform.parent = parent;
        bulletRight.transform.parent = parent;

        bulletLeft.transform.position = turretGunL.position;
        bulletRight.transform.position = turretGunR.position;

        bulletRotation = RandomSpreadCal();
        bulletLeft.transform.rotation = bulletRotation;
        bulletRight.transform.rotation = bulletRotation;

        bulletLeft.Init(KillBullet);
    }


    private Quaternion RandomSpreadCal()
    {
        Quaternion bulletSpread = Quaternion.identity;
        Quaternion enemyDirection = Quaternion.identity;

        float randomAngle = Random.Range(0, Mathf.PI * 2f);
        Vector3 rotateVector = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);

        Quaternion spreadRotate = Quaternion.AngleAxis(Random.Range(0, 30), rotateVector);

        //enemyDirection = Quaternion.Euler(0f, enemyFOV.AngleToTarget, 0f);

        bulletSpread = spreadRotate;

        return bulletSpread;
    }

    private void KillBullet(CTurretPoolTest2 bullet)
    {
        PoolRelease(bullet);
    }

}
