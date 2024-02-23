using System;
using UnityEngine;

public class CBulletHoleSpawn : CComponent
{
    [SerializeField] private CBulletHole bulletHoleMetalPrefab;
    [SerializeField] private CBulletHole bulletHoleConcretPrefab;
    [SerializeField] private CBulletHole bulletHoleGlassPrefab;

    [SerializeField] private Transform bulletHoleParent;

    public void CreateBulletHoleMetal(Vector3 pos, Vector3 direction)
    {
        CBulletHole spawnBulletHole;
        if(bulletHoleMetalPrefab != null)
            spawnBulletHole = Instantiate(bulletHoleMetalPrefab, pos + (direction * 0.01f), Quaternion.FromToRotation(Vector3.up, direction), bulletHoleParent);
        else
            spawnBulletHole = Instantiate(bulletHoleMetalPrefab, pos + (direction * 0.01f), Quaternion.FromToRotation(Vector3.up, direction));

        spawnBulletHole.SetRandomTexture();
    }

    public void CreateBulletHoleConcret(Vector3 pos, Vector3 direction)
    {
        CBulletHole spawnBulletHole;
        if (bulletHoleMetalPrefab != null)
            spawnBulletHole = Instantiate(bulletHoleConcretPrefab, pos + (direction * 0.01f), Quaternion.FromToRotation(Vector3.up, direction), bulletHoleParent);
        else
            spawnBulletHole = Instantiate(bulletHoleConcretPrefab, pos + (direction * 0.01f), Quaternion.FromToRotation(Vector3.up, direction));

        spawnBulletHole.SetRandomTexture();
    }

    public void CreateBulletHoleGlass(Vector3 pos, Vector3 direction) 
    {
        CBulletHole spawnBulletHole;
        if (bulletHoleMetalPrefab != null)
            spawnBulletHole = Instantiate(bulletHoleGlassPrefab, pos + (direction * 0.01f), Quaternion.FromToRotation(Vector3.up, direction), bulletHoleParent);
        else
            spawnBulletHole = Instantiate(bulletHoleGlassPrefab, pos + (direction * 0.01f), Quaternion.FromToRotation(Vector3.up, direction));

        spawnBulletHole.SetRandomTexture();
    }   

}
