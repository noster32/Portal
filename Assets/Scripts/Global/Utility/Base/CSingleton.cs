using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSingleton<T> : CComponent where T : CComponent
{
    private static T m_oInstance = null;

    public static T Instance
    {
        get
        {
            if(m_oInstance == null)
            {
                var oGameObject = new GameObject(typeof(T).ToString());
                m_oInstance = Function.AddComponent<T>(oGameObject);

                //씬이 전환될 때 남기고 싶은 애들을 남긴다
                DontDestroyOnLoad(oGameObject);
            }

            return m_oInstance;
        }
    }
    public static T Create()
    {
        return CSingleton<T>.Instance;
    }

}

