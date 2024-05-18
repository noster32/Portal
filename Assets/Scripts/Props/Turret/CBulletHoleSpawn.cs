using System;
using System.Collections.Generic;
using UnityEngine;

public class CBulletHoleSpawn : CComponent
{
    [SerializeField] private CBulletHole[] bulletHolePrefabs;
    private Dictionary<ObjectMaterial, CBulletHole> bulletHoleDictionary = new Dictionary<ObjectMaterial, CBulletHole>();

    public override void Awake()
    {
        base.Awake();

        Array enumValues = Enum.GetValues(typeof(ObjectMaterial));
        int enumCount = enumValues.Length;

        for (int i = 0; i < enumCount; ++i)
        {
            ObjectMaterial mat = (ObjectMaterial)enumValues.GetValue(i);
            bulletHoleDictionary.Add(mat, null);
        }
        
        for (int i = 0; i < bulletHolePrefabs.Length; ++i)
        {
            var surfaceMat = bulletHolePrefabs[i].surfaceMaterial;
            var bulletHolePrefab = bulletHolePrefabs[i];

            if (bulletHoleDictionary[surfaceMat] == null)
                bulletHoleDictionary[surfaceMat] = bulletHolePrefab;
            else
                Debug.LogWarning("already have surface bullet hole : " + surfaceMat);
        }
    }

    public void CreateBulletHole(Vector3 pos, Vector3 direction, ObjectMaterial mat, Transform parent)
    {
        CBulletHole holePrefab = bulletHoleDictionary[mat];
        CBulletHole spawnBulletHole;

        spawnBulletHole = Instantiate(holePrefab, pos + (direction * 0.01f),
                                      Quaternion.FromToRotation(Vector3.up, direction));
        spawnBulletHole.transform.parent = parent;
    }
}
