using System;
using UnityEngine;

public class CBulletSpawn : CPoolingManager<CBullet>
{
    CEnemyDeploy enemyFOV;
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

    public override void Awake()
    {
        base.Awake();

        enemyFOV = GetComponent<CEnemyDeploy>();
        bulletHoleSpawn = GetComponent<CBulletHoleSpawn>();
    }
    public override void Start()
    {
        base.Start();

        InitPool(bulletPrefab, 50, 100, false);
    }

    public void CreateBullet(CBullet bullet, Vector3 pos, Vector3 vel, Vector3 dir)
    {
        bullet.transform.parent = parent;
        bullet.transform.position = pos;
        bullet.transform.rotation = Quaternion.LookRotation(dir);
        bullet.initialPosition = pos;
        bullet.initialVelocity = vel;
        bullet.time = 0f;
        bullet.maxLifeTime = this.maxLifeTime;
        
        bullet.DeleteBullet(KillBullet);
        bullet.SetBulletHoleSpawn(bulletHoleSpawn.CreateBulletHole);
    }

    private Vector3 velocityCal(Vector3 start, Vector3 end, float speed) => (end - start).normalized * speed;

    public void FIreBulletRay()
    {
        Vector3 startPosL = turretGunL.position;
        Vector3 startPosR = turretGunR.position;
        Vector3 bulletDirectionL = RandomSpreadCalVector3();
        Vector3 bulletDirectionR = RandomSpreadCalVector3();

        Vector3 destinationLeft = startPosL + bulletDirectionL * maxRayLength;
        Vector3 destinationRight = startPosR + bulletDirectionR * maxRayLength;
        Vector3 velocityL = velocityCal(startPosL, destinationLeft, 50f);
        Vector3 velocityR = velocityCal(startPosR, destinationRight, 50f);

        var bulletLeft = Get();
        var bulletRight = Get();

        CreateBullet(bulletLeft, startPosL, velocityL, bulletDirectionL);
        CreateBullet(bulletRight, startPosR, velocityR, bulletDirectionR);
    }

    public void FallDownFireBulletRay()
    {
        Vector3 startPosL = turretGunL.position;
        Vector3 startPosR = turretGunR.position;

        float randomRotationX = UnityEngine.Random.Range(-10f, 10f);
        float randomRotationY = UnityEngine.Random.Range(-10f, 10f);
        Quaternion randomSpread = Quaternion.Euler(randomRotationX, randomRotationY, 0f);

        Vector3 bulletDirectionL = randomSpread * transform.forward;
        Vector3 bulletDirectionR = randomSpread * transform.forward;

        Vector3 destinationLeft = startPosL + bulletDirectionL * maxRayLength;
        Vector3 destinationRight = startPosR + bulletDirectionR * maxRayLength;
        Vector3 velocityL = velocityCal(startPosL, destinationLeft, 50f);
        Vector3 velocityR = velocityCal(startPosR, destinationRight, 50f);

        var bulletLeft = Get();                                                          
        var bulletRight = Get();                                                                                                                                                    
                                                                                         
        CreateBullet(bulletLeft, startPosL, velocityL, bulletDirectionL);
        CreateBullet(bulletRight, startPosR, velocityR, bulletDirectionR);
    }                                                                                    

    private Vector3 RandomSpreadCalVector3()
    {
        Vector3 bulletSpread = Vector3.zero;
        Quaternion enemyDirection = Quaternion.identity;
        
        float randomAngle = UnityEngine.Random.Range(0, Mathf.PI * 2f);
        Vector3 rotateVector = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);
        Quaternion spreadRotate = Quaternion.AngleAxis(UnityEngine.Random.Range(0, spreadAngle), rotateVector);

        enemyDirection = Quaternion.Euler(0f, enemyFOV.angleToTargetBackward + transform.eulerAngles.y, 0f);
        bulletSpread = enemyDirection * spreadRotate * Vector3.forward;

        return bulletSpread;
    }

    private void KillBullet(CBullet bullet) => PoolRelease(bullet);
}
