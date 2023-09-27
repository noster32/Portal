using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CTurretFireTest : CComponent
{
    #region public variable


    public float spreadAngle = 3f;
    #endregion

    #region private variable
    private CTurret turret;
    #endregion

    #region serializedField

    [SerializeField]
    private GameObject bulletObj;
    [SerializeField]
    private Transform turretGunL;
    [SerializeField]
    private Transform turretGunR;
    [SerializeField]
    private Transform turretLR;

    #endregion

    public override void Start()
    {
        turret = GetComponent<CTurret>();
        
    }

    public override void Update()
    {
        if(turret.GetState == CTurret.TurretState.ATTACK)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                this.Fire();
            }
        }
        
    }

    private void Fire()
    {
        if(this.bulletObj == null)
        {
            Debug.Log("No Bullet");
            return;
        }
        else if(this.turretGunL == null)
        {
            Debug.Log("Cant Find turretGunL");
            return;
        }
        else if(this.turretGunR == null)
        {
            Debug.Log("Cant Find turretGunR");
            return;
        }

        Quaternion rotationFire = Quaternion.identity;

        float randomAngle = Random.Range(0, Mathf.PI * 2f);

        Vector3 rotVector = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);

        Vector3 shootDirection = this.transform.forward;

        Quaternion conRotate = Quaternion.AngleAxis(Random.Range(0, spreadAngle), rotVector);

        shootDirection = conRotate * shootDirection;

        rotationFire = Quaternion.LookRotation(shootDirection) * turretLR.transform.rotation;

        Instantiate(this.bulletObj, turretGunL.transform.position, rotationFire);

    }
}
