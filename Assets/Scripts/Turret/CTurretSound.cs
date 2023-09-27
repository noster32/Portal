using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTurretSound : CComponent
{
    #region sound Path
    #endregion
    #region private
    [SerializeField]
    private AudioClip turretGunSoundClip;
    private AudioSource turretGunSoundSource;

    [SerializeField]
    private Transform targetObj;
    #endregion


    public override void Start()
    {
        base.Awake();

        turretGunSoundSource = GetComponent<AudioSource>();
        CSoundLoader.Instance.LoadSound(turretGunSoundSource, turretGunSoundClip, 1f, false, 1f, 10f);
    }

    public override void Update()
    {
        base.Update(); 
    }

    public void GunSoundTest(Transform turretPos)
    {
        CSoundLoader.Instance.PlaySoundEffect3D(turretGunSoundSource, turretPos.position, turretPos.position, 0.2f);
    }

    public void ActiveSoundTest(Transform turretPos)
    {
        CSoundLoader.Instance.PlaySoundEffect3D(turretGunSoundSource, turretPos.position, turretPos.position, 0.2f);
    }


}
