using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Pool;

public abstract class CPoolingManager<T> : CComponent where T : CComponent 
{
    private T prefab;
    private ObjectPool<T> pool;

    private ObjectPool<T> Pool
    {
        get
        {
            if (pool != null)
                throw new InvalidOperationException("You need to call InitPool before using it.");
            return pool;
        }

        set => pool = value;
    }

    protected void InitPool(T initPrefab, int initial = 10, int max = 20, bool collectionChecks = false)
    {
        prefab = initPrefab;
        pool = new ObjectPool<T>
        {

        }
    
    }

    protected virtual T CreateSetup() => Instantiate(prefab);
    protected virtual void GetSetup(T obj) => obj.gameObject.SetActive(true);
    protected virtual void ReleaseSetup(T obj) => obj.gameObject.SetActive(false);
    protected virtual void DestroySetup(T obj) => Destroy(obj);

    public T Get() => Pool.Get();
    public void Release(T obj) => Pool.Release(obj);

}
