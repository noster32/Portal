using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//! 싱글톤 상속
//메세지 참조는 10개 이하 권자
public class CSceneLoader : CSingleton<CSceneLoader>
{
    //! 씬을 변경한다
    public void LoadScene(int a_nIndex)
    {
        //씬 리스트의 빌드 인덱스를 가진 씬을 컬렉션들에 담을 수있는 메서드
        //Project Setting -> 씬 추가가능
        string oScenePath = SceneUtility.GetScenePathByBuildIndex(a_nIndex);
        this.LoadScene(oScenePath);
    }
   

    public void LoadScene(string o_nName)
    {
        //LoadSceneMode.Single
        //LoadSceneMode.Additivve
        // Addtive 는 UI에서 사용
        SceneManager.LoadScene(o_nName, LoadSceneMode.Single);
    }

    //! 씬을 로드한다
   public void LoadSceneAsync(int a_nIndex, System.Action<AsyncOperation> a_oCallback,
       float a_Delay = 0.0f, LoadSceneMode a_eLoadSceneMode = LoadSceneMode.Single)
    {
        string oScenePath = SceneUtility.GetScenePathByBuildIndex(a_nIndex);
        // 비동기 호출 -> oCallBack 대기자가 호출되어 대리자 먼저 호출되기 때문에 대기한다
        this.LoadSceneAsync(oScenePath, a_oCallback, a_Delay, a_eLoadSceneMode);
        
    }

    //! 씬을 로드한다
    public void LoadSceneAsync(string a_oName, System.Action<AsyncOperation> a_oCallback,
    float a_Delay = 0.0f, LoadSceneMode a_eLoadSceneMode = LoadSceneMode.Single)
    {
        //콜백이 역기서 찍힌다
        StartCoroutine(this.DoLoadSceneAsync(a_oName, a_oCallback, a_Delay, a_eLoadSceneMode));
    }

    private IEnumerator DoLoadSceneAsync(string a_oName, System.Action<AsyncOperation> a_oCallback,
    float a_Delay, LoadSceneMode a_eLoadSceneMode)
    {
        //딜레이 
        yield return new WaitForSeconds(a_Delay);
        //노드 잡아주기
        var oAsyncOperation = SceneManager.LoadSceneAsync(a_oName, a_eLoadSceneMode);
        
        yield return Function.WaitAsyncOperation(oAsyncOperation, a_oCallback);
    }
    //
}
