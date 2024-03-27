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

            //���� ��ȯ�� �� ����� ���� �ֵ��� �����
            DontDestroyOnLoad(oGameObject);
        }
        else
            Debug.LogError("already have singleton");

        return CSingleton<T>.Instance;
    }

}

