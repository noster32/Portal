using UnityEngine;

public abstract class CSingleton<T> : CComponent where T : CComponent
{
    protected static T m_oInstance = null;

    public static T Instance
    {
        get
        {
            return m_oInstance;
        }
    }

    public static T Create()
    {
        if (m_oInstance == null)
        {
            var oGameObject = new GameObject(typeof(T).ToString());
            m_oInstance = Function.AddComponent<T>(oGameObject);

            //씬이 전환될 때 남기고 싶은 애들을 남긴다
            DontDestroyOnLoad(oGameObject);
        }
        else
            Debug.LogError("already have singleton");

        return CSingleton<T>.Instance;
    }

}

