using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CTurretFire : CComponent
{
    #region component

    CTurret turret;
    CEnemyFieldOfView enemyFOV;
    CTurretParticle turretParticle;
    CTurretSound turretSound;

    #endregion

    #region public variable
    #endregion

    #region private variable

    [SerializeField] private CBullet bulletPrefab;
    [SerializeField] private Transform parentTransform;

    [Header("Turret Object")]
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private Transform turretGunL;
    [SerializeField] private Transform turretGunR;

    [Header("Fire Setting")]
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float spreadAngle = 3f;

    private ObjectPool<CBullet> bulletPool;
    #endregion

    public override void Start()
    {
        base.Start();

        bulletPool = new ObjectPool<CBullet>(() =>
        {
            return Instantiate()
        })

        turret = GetComponent<CTurret>();
        enemyFOV = GetComponent<CEnemyFieldOfView>();
        turretParticle = GetComponent<CTurretParticle>();
        turretSound = GetComponent<CTurretSound>();

        StartCoroutine(AutoFire(fireRate));
    }

    IEnumerator AutoFire(float rate)
    {
        while (true)
        {
            if (turret.GetState == CTurret.TurretState.ATTACK)
            {
                Fire();
                //turretSound.GunSoundTest(turretGunR);
                turretParticle.PlayMuzzleEffect();
            }
            else if(turret.GetState == CTurret.TurretState.FALLDOWN)
            {
                FallDownFire();
                turretParticle.PlayMuzzleEffect();
            }

            yield return new WaitForSeconds(rate);
        }
    }
    private void Fire()
    {
        if (this.bulletObject == null)
        {
            Debug.Log("No Bullet");
            return;
        }
        else if (this.turretGunL == null)
        {
            Debug.Log("Cant Find turretGunL");
            return;
        }
        else if (this.turretGunR == null)
        {
            Debug.Log("Cant Find turretGunR");
            return;
        }

        Quaternion bulletSpread = Quaternion.identity;
        Quaternion enemyDirection = Quaternion.identity;

        //·£´ý ¶óµð¾È
        float randomAngle = Random.Range(0, Mathf.PI * 2f);

        // ·£´ý º¤ÅÍ
        Vector3 rotateVector = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);

        // spreadAngleÀ» ÅëÇÑ ·£´ýÀ¸·Î Åº È¸ÀüÇÏ¿© Èð»Ñ¸®±â
        Quaternion spreadRotate = Quaternion.AngleAxis(Random.Range(0, spreadAngle), rotateVector);

        // ÀûÀ» ÇâÇØ¼­ ¹æÇâ ¼³Á¤
        enemyDirection = Quaternion.Euler(0, enemyFOV.AngleToTarget, 0);

        bulletSpread = enemyDirection * spreadRotate;

        Instantiate(this.bulletObject, turretGunL.transform.position, bulletSpread);
        Instantiate(this.bulletObject, turretGunR.transform.position, bulletSpread);
    }

    private void FallDownFire()
    {
        if (this.bulletObject == null)
        {
            Debug.Log("No Bullet");
            return;
        }
        else if (this.turretGunL == null)
        {
            Debug.Log("Cant Find turretGunL");
            return;
        }
        else if (this.turretGunR == null)
        {
            Debug.Log("Cant Find turretGunR");
            return;
        }

        float randomRotationX = Random.Range(-10f, 10f);
        float randomRotationY = Random.Range(-10f, 10f);


        Quaternion randomSpread = Quaternion.Euler(randomRotationX, randomRotationY, 0);

        Instantiate(this.bulletObject, turretGunL.transform.position, randomSpread);
        Instantiate(this.bulletObject, turretGunR.transform.position, randomSpread);
    }

}
