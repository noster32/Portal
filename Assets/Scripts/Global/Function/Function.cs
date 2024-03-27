using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//! ���� �Լ�
public static partial class Function
{
    public static GameObject CreateGameObject(string a_oName, GameObject a_oParent, bool a_bIsStayWorldState = false)
    {
        var oGameObject = new GameObject(a_oName);

        // SetParent() : SetParent�� ���� �Ӽ� ���� �Ű� ������ true�� ������ �Ǹ�
        // ���� �� �������� ���� �Ӽ��� �״�� �����ϱ� ������ Ư�� ��ü�� ������ ������ �� ���
        // �θ� ��ü�� ũ�⿡ ���� �ڽ� ��ü�� ������ ���� ������ �ش�.
        oGameObject?.transform.SetParent(a_oParent?.transform.transform, a_bIsStayWorldState);

        return oGameObject;
    }


    //���� ������Ʈ �߿� ������ ���� �˼��� ����
    public static T CreateGameObject<T>(string a_oName, GameObject a_oParent, bool a_bIsStayWorldState = false) where T : Component
    {
        var oGameObject = Function.CreateGameObject(a_oName, a_oParent, a_bIsStayWorldState);

        return Function.AddComponent<T>(oGameObject);
    }

    //! ������Ʈ Ž��
    public static T FindComponent<T>(string a_oName) where T : Component
    {
        var oGameObject = GameObject.Find(a_oName);

        return oGameObject?.GetComponentInChildren<T>();
    }

    public static T AddComponent<T>(GameObject a_oGameObject) where T : Component
    {
        var oComponent = a_oGameObject.AddComponent<T>();

        if (oComponent != null)
        {
            oComponent = a_oGameObject.GetComponent<T>();
        }

        return oComponent;
    }

    //!�񵿱� �۾��� �����Ѵ�

    public static IEnumerator WaitAsyncOperation(AsyncOperation a_oAsyncOperation, System.Action<AsyncOperation> a_oCallBack)
    {
        while (!a_oAsyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
            a_oCallBack?.Invoke(a_oAsyncOperation);
        }
    }
}