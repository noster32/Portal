using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class CPlayerRecoil : CComponent
{
    [SerializeField] private Transform playerCamera;
    private Vector3 recoilRotation;
    private Vector3 currentRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    public override void Update()
    {
        base.Update();

        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, returnSpeed *  Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, recoilRotation, snappiness * Time.fixedDeltaTime);
        playerCamera.localRotation = playerCamera.localRotation * Quaternion.Euler(currentRotation);
    }

    public void FireRecoil()
    {
        recoilRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), 0f);
        Debug.Log(recoilRotation);
    }


}
