using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CBulletSpawn : CPoolingManager<CBullet>
{
    #region component

    CTurret turret;
    CEnemyFieldOfView enemyFOV;

    #endregion

    #region private
    [SerializeField] private CBullet bulletPrefab;
    [SerializeField] private Transform parent;

    [SerializeField] private Transform turretGunL;
    [SerializeField] private Transform turretGunR;

    [SerializeField] private int spreadAngle = 30;
    #endregion

    public override void Start()
    {
        base.Start();

        turret = GetComponent<CTurret>();
        enemyFOV = GetComponent<CEnemyFieldOfView>();

        InitPool(bulletPrefab, 50, 100, false);
    }

    public void FireBullet()
    {
        var bulletLeft = Get();
        var bulletRight = Get();
        Quaternion bulletRotation = Quaternion.identity;
        bulletLeft.distance = 500f;
        bulletRight.distance = 500f;
        
        bulletLeft.transform.parent = parent;
        bulletRight.transform.parent = parent;
        
        bulletLeft.transform.position = turretGunL.position;
        bulletRight.transform.position = turretGunR.position;
        
        bulletRotation = RandomSpreadCal();
        bulletLeft.transform.rotation = bulletRotation;
        bulletRight.transform.rotation = bulletRotation;
        
        bulletLeft.DeleteBullet(KillBullet);
        bulletRight.DeleteBullet(KillBullet);
    }

    public void FallDownFireBullet()
    {
        var bulletLeft = Get();
        var bulletRight = Get();

        float randomRotationX = Random.Range(-10f, 10f);
        float randomRotationY = Random.Range(-10f, 10f);

        Quaternion randomSpread = Quaternion.Euler(randomRotationX, randomRotationY, 0f);

        bulletLeft.distance = 500f;
        bulletRight.distance = 500f;

        bulletLeft.transform.parent = parent;
        bulletRight.transform.parent = parent;

        bulletLeft.transform.position = turretGunL.position;
        bulletRight.transform.position = turretGunR.position;

        bulletLeft.transform.rotation = randomSpread;
        bulletRight.transform.rotation = randomSpread;

        bulletLeft.DeleteBullet(KillBullet);
        bulletRight.DeleteBullet(KillBullet);
    }

    private Quaternion RandomSpreadCal()
    {
        Quaternion bulletSpread = Quaternion.identity;
        Quaternion enemyDirection = Quaternion.identity;

        float randomAngle = Random.Range(0, Mathf.PI * 2f);

        Vector3 rotateVector = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);

        Quaternion spreadRotate = Quaternion.AngleAxis(Random.Range(0, spreadAngle), rotateVector);

        enemyDirection = Quaternion.Euler(0f, enemyFOV.AngleToTarget, 0f);

        bulletSpread = enemyDirection * spreadRotate;

        return bulletSpread;
    }
    private void KillBullet(CBullet bullet)
    {
        PoolRelease(bullet);
    }
}
