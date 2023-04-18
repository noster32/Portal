using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//! �̱��� ���
//�޼��� ������ 10�� ���� ����
public class CSceneLoader : CSingleton<CSceneLoader>
{
    //! ���� �����Ѵ�
    public void LoadScene(int a_nIndex)
    {
        //�� ����Ʈ�� ���� �ε����� ���� ���� �÷��ǵ鿡 ���� ���ִ� �޼���
        //Project Setting -> �� �߰�����
        string oScenePath = SceneUtility.GetScenePathByBuildIndex(a_nIndex);
        this.LoadScene(oScenePath);
    }
   

    public void LoadScene(string o_nName)
    {
        //LoadSceneMode.Single
        //LoadSceneMode.Additivve
        // Addtive �� UI���� ���
        SceneManager.LoadScene(o_nName, LoadSceneMode.Single);
    }

    //! ���� �ε��Ѵ�
   public void LoadSceneAsync(int a_nIndex, System.Action<AsyncOperation> a_oCallback,
       float a_Delay = 0.0f, LoadSceneMode a_eLoadSceneMode = LoadSceneMode.Single)
    {
        string oScenePath = SceneUtility.GetScenePathByBuildIndex(a_nIndex);
        // �񵿱� ȣ�� -> oCallBack ����ڰ� ȣ��Ǿ� �븮�� ���� ȣ��Ǳ� ������ ����Ѵ�
        this.LoadSceneAsync(oScenePath, a_oCallback, a_Delay, a_eLoadSceneMode);
        
    }

    //! ���� �ε��Ѵ�
    public void LoadSceneAsync(string a_oName, System.Action<AsyncOperation> a_oCallback,
    float a_Delay = 0.0f, LoadSceneMode a_eLoadSceneMode = LoadSceneMode.Single)
    {
        //�ݹ��� ���⼭ ������
        StartCoroutine(this.DoLoadSceneAsync(a_oName, a_oCallback, a_Delay, a_eLoadSceneMode));
    }

    private IEnumerator DoLoadSceneAsync(string a_oName, System.Action<AsyncOperation> a_oCallback,
    float a_Delay, LoadSceneMode a_eLoadSceneMode)
    {
        //������ 
        yield return new WaitForSeconds(a_Delay);
        //��� ����ֱ�
        var oAsyncOperation = SceneManager.LoadSceneAsync(a_oName, a_eLoadSceneMode);
        
        yield return Function.WaitAsyncOperation(oAsyncOperation, a_oCallback);
    }
    //
}
