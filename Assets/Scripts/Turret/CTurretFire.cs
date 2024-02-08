using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTurretFire : CComponent
{
    #region component

    CTurret turret;
    CTurretParticle turretParticle;
    CTurretSound turretSound;
    CBulletSpawn bulletSpawn;

    #endregion

    #region public variable
    #endregion

    #region private variable

    [Header("Fire Setting")]
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float spreadAngle = 3f;
    #endregion

    public override void Start()
    {
        base.Start();

        turret = GetComponent<CTurret>();
        turretParticle = GetComponent<CTurretParticle>();
        turretSound = GetComponent<CTurretSound>();
        bulletSpawn = GetComponent<CBulletSpawn>();

        StartCoroutine(AutoFire(fireRate));
    }

    IEnumerator AutoFire(float rate)
    {
        while (true)
        {
            if (turret.GetState == CTurret.TurretState.ATTACK)
            {
                bulletSpawn.FireBullet();
                turretParticle.PlayMuzzleEffect();
                //turretSound.GunSoundTest(turretGunR);
            }
            else if(turret.GetState == CTurret.TurretState.FALLDOWN)
            {
                bulletSpawn.FallDownFireBullet();
                turretParticle.PlayMuzzleEffect();
            }

            yield return new WaitForSeconds(rate);
        }
    }
}
