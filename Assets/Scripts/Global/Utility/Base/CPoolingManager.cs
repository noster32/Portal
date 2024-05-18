using System;
using UnityEngine.Pool;

public abstract class CPoolingManager<T> : CComponent where T : CComponent
{
    private T prefab;
    private ObjectPool<T> pool;

    private ObjectPool<T> poolProperty
    {
        get
        {
            if (pool == null)
                throw new InvalidOperationException("You need to call InitPool before using it.");
            return pool;
        }

        set => pool = value;
    }

    protected void InitPool(T initPrefab, int initial = 10, int max = 20, bool collectionChecks = false)
    {
        prefab = initPrefab;
        poolProperty = new ObjectPool<T>(
            ObjectCreate,
            ObjectActive,
            ObjectDeactive,
            ObjectDestroy,
            collectionChecks,
            initial,
            max
            );
    }

    protected virtual T ObjectCreate() => Instantiate(prefab);
    protected virtual void ObjectActive(T obj) => obj.gameObject.SetActive(true);
    protected virtual void ObjectDeactive(T obj) => obj.gameObject.SetActive(false);
    protected virtual void ObjectDestroy(T obj) => Destroy(obj);

    public T Get() => poolProperty.Get();
    public void PoolRelease(T obj) => poolProperty.Release(obj);
}
