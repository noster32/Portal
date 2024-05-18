using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CTurret;

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

    public override void Update()
    {
        base.Update();
    }

    IEnumerator AutoFire(float rate)
    {
        while (turret.GetState != TurretState.DIE && turret.GetState != TurretState.TURNOFF)
        {
            if (turret.GetState == CTurret.TurretState.ATTACK)
            {
                bulletSpawn.FIreBulletRay();
                turretParticle.PlayMuzzleEffect();
                turretSound.PlayTurretGunSound();
            }
            else if(turret.GetState == CTurret.TurretState.FALLDOWN)
            {
                bulletSpawn.FallDownFireBulletRay();
                turretParticle.PlayMuzzleEffect();
                turretSound.PlayTurretGunSound();
            }

            yield return new WaitForSeconds(rate);
        }
    }
}
