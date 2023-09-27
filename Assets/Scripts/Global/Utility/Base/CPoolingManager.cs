using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class CPoolingManager<T> : CComponent where T : CComponent 
{
    private T prefeb;
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
}
