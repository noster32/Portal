using System;
using UnityEngine;

public class CBulletSpawn : CPoolingManager<CBullet>
{
    CEnemyFieldOfView enemyFOV;
    private CBulletHoleSpawn bulletHoleSpawn;

    #region private
    [SerializeField] private CBullet bulletPrefab;
    [SerializeField] private Transform parent;

    [SerializeField] private Transform turretGunL;
    [SerializeField] private Transform turretGunR;

    [SerializeField] private int spreadAngle = 30;
    [SerializeField] private float maxLifeTime = 5f;
    [SerializeField] private float maxRayLength = 50f;

    #endregion

    Ray rayL;
    Ray rayR;

    public override void Awake()
    {
        base.Awake();

        enemyFOV = GetComponent<CEnemyFieldOfView>();
        bulletHoleSpawn = GetComponent<CBulletHoleSpawn>();
    }
    public override void Start()
    {
        base.Start();

        InitPool(bulletPrefab, 50, 100, false);
    }

    public void CreateBullet(CBullet bullet, Vector3 pos, Vector3 vel, Vector3 rot, Vector3 endPos)
    {
        bullet.transform.parent = parent;
        bullet.transform.position = pos;
        bullet.transform.rotation = Quaternion.Euler(rot);
        bullet.initialPosition = pos;
        bullet.initialVelocity = vel;
        bullet.endPosition = endPos;
        bullet.time = 0f;
        bullet.maxLifeTime = this.maxLifeTime;
        
        bullet.DeleteBullet(KillBullet);
        bullet.BulletHoleFunction(bulletHoleSpawn.CreateBulletHoleConcret, bulletHoleSpawn.CreateBulletHoleMetal, bulletHoleSpawn.CreateBulletHoleGlass);
    }

    public void FIreBulletRay()
    {
        Vector3 bulletRotationLeft;
        Vector3 bulletRotationRight;

        Vector3 destinationLeft;
        Vector3 destinationRight;

        bulletRotationLeft = RandomSpreadCalVector3();
        bulletRotationRight = RandomSpreadCalVector3();

        rayL.origin = turretGunL.position;
        rayL.direction = bulletRotationLeft;
        rayR.origin = turretGunR.position;
        rayR.direction = bulletRotationRight;

        destinationLeft = rayL.origin + rayL.direction * maxRayLength;
        //Debug.DrawLine(rayL.origin, destinationLeft, Color.yellow, 2f);

        destinationRight = rayR.origin + rayR.direction * maxRayLength;
        //Debug.DrawLine(rayR.origin, destinationRight, Color.yellow, 2f);
        
        Func<Vector3, Vector3, float, Vector3> velocityCal = (start, end, speed) => (end - start).normalized * speed;

        var bulletLeft = Get();
        var bulletRight = Get();

        Vector3 velocityL = velocityCal(rayL.origin, destinationLeft, 50f);
        Vector3 velocityR = velocityCal(rayL.origin, destinationRight, 50f);

        CreateBullet(bulletLeft, rayL.origin, velocityL, bulletRotationLeft, destinationLeft);
        CreateBullet(bulletRight, rayR.origin, velocityR, bulletRotationRight, destinationRight);
    }

    public void FallDownFireBulletRay()
    {
        Vector3 bulletRotationLeft;
        Vector3 bulletRotationRight;

        Vector3 destinationLeft;
        Vector3 destinationRight;

        float randomRotationX = UnityEngine.Random.Range(-10f, 10f);
        float randomRotationY = UnityEngine.Random.Range(-10f, 10f);

        Quaternion randomSpread = Quaternion.Euler(randomRotationX, randomRotationY, 0f);

        bulletRotationLeft = randomSpread * transform.forward;
        bulletRotationRight = randomSpread * transform.forward;

        rayL.origin = turretGunL.position;
        rayL.direction = bulletRotationLeft;
        rayR.origin = turretGunR.position;
        rayR.direction = bulletRotationRight;

        destinationLeft = rayL.origin + rayL.direction * maxRayLength;
        destinationRight = rayR.origin + rayR.direction * maxRayLength;

        Func<Vector3, Vector3, float, Vector3> velocityCal = (start, end, speed) => (end - start).normalized * speed;

        var bulletLeft = Get();
        var bulletRight = Get();

        Vector3 velocityL = velocityCal(rayL.origin, destinationLeft, 50f);
        Vector3 velocityR = velocityCal(rayL.origin, destinationRight, 50f);

        CreateBullet(bulletLeft, rayL.origin, velocityL, bulletRotationLeft, destinationLeft);
        CreateBullet(bulletRight, rayR.origin, velocityR, bulletRotationRight, destinationRight);
    }

    public void FireBullet()
    {
        Quaternion bulletRotationLeft = Quaternion.identity;
        Quaternion bulletRotationRight = Quaternion.identity;

        var bulletLeft = Get();
        var bulletRight = Get();

        bulletLeft.distance = 500f;
        bulletRight.distance = 500f;
        
        bulletLeft.transform.parent = parent;
        bulletRight.transform.parent = parent;
        
        bulletLeft.transform.localScale = new Vector3(2f, 2f, 2f);
        bulletRight.transform.localScale = new Vector3(2f, 2f, 2f);

        bulletLeft.transform.position = turretGunL.position;
        bulletRight.transform.position = turretGunR.position;
        
        bulletRotationLeft = RandomSpreadCal();
        bulletRotationRight = RandomSpreadCal();
        bulletLeft.transform.rotation = bulletRotationLeft;
        bulletRight.transform.rotation = bulletRotationRight;
        
        bulletLeft.DeleteBullet(KillBullet);
        bulletRight.DeleteBullet(KillBullet);
    }

    public void FallDownFireBullet()
    {
        var bulletLeft = Get();
        var bulletRight = Get();

        float randomRotationX = UnityEngine.Random.Range(-10f, 10f);
        float randomRotationY = UnityEngine.Random.Range(-10f, 10f);

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

        float randomAngle = UnityEngine.Random.Range(0, Mathf.PI * 2f);

        Vector3 rotateVector = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);

        Quaternion spreadRotate = Quaternion.AngleAxis(UnityEngine.Random.Range(0, spreadAngle), rotateVector);

        enemyDirection = Quaternion.Euler(0f, enemyFOV.angleToTarget + transform.rotation.eulerAngles.y, 0f);

        bulletSpread = enemyDirection * spreadRotate;

        return bulletSpread;
    }

    private Vector3 RandomSpreadCalVector3()
    {
        Vector3 bulletSpread = Vector3.zero;
        Quaternion enemyDirection = Quaternion.identity;
        
        float randomAngle = UnityEngine.Random.Range(0, Mathf.PI * 2f);

        Vector3 rotateVector = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);

        Quaternion spreadRotate = Quaternion.AngleAxis(UnityEngine.Random.Range(0, spreadAngle), rotateVector);

        enemyDirection = Quaternion.Euler(0f, enemyFOV.angleToTarget + transform.eulerAngles.y, 0f);

        bulletSpread = enemyDirection * spreadRotate * Vector3.forward;

        return bulletSpread;
    }

    private void KillBullet(CBullet bullet)
    {
        PoolRelease(bullet);
    }
}
