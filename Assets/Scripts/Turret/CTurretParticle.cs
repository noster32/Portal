using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class CTurretParticle : CComponent
{
    #region component
    CEnemyFieldOfView enemyFOV;
    #endregion

    #region private value

    [Header("Muzzle Effect")]
    [SerializeField]
    private GameObject muzzleEffectObject;
    [SerializeField]
    private Transform effectPositionLeft;
    [SerializeField]
    private Transform effectPositionRight;

    #endregion
    public override void Start()
    {
        base.Start();

        enemyFOV = GetComponent<CEnemyFieldOfView>();
    }

    public void PlayMuzzleEffect()
    {
        if (muzzleEffectObject == null)
        {
            Debug.Log("Muzzle Effect Object is null");
            return;
        }
        else if (effectPositionLeft == null)
        {
            Debug.Log("Effect Position Left is null");
            return;
        }
        else if (effectPositionRight == null)
        {
            Debug.Log("Effect Position Right is null");
            return;
        }
        
        Vector3 modifyLeftPosition = effectPositionLeft.position + new Vector3(0f, 0f, 0.1f);
        Vector3 modifyRightPosition = effectPositionRight.position + new Vector3(0f, 0f, 0.1f);

        Quaternion modifyRotation = Quaternion.Euler(180f, enemyFOV.AngleToTarget, 0f);

        GameObject muzzleEffectLeft = Instantiate(muzzleEffectObject, modifyLeftPosition, modifyRotation);
        GameObject muzzleEffectRight = Instantiate(muzzleEffectObject, modifyRightPosition, modifyRotation);

        Destroy(muzzleEffectLeft, 0.105f);
        Destroy(muzzleEffectRight, 0.105f);
    }

}
