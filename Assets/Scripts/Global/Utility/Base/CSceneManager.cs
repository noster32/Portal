using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneManager : CComponent
{

    #region public

    //시작할 떄 균일한 시작 값을 기준으로 실행학 위해서
    public float m_fPlaneDistance = KDefine.DEFAULT_PLANE_DISTANCE;
    //?
    #endregion

    public static Camera UICamera
    {
        get
        {
            return Function.FindComponent<Camera>(KDefine.NAME_UI_CAMERA);
        }
    }

    public static Camera MainCamera
    {
        get
        {
            return Function.FindComponent<Camera>(KDefine.NAME_MAIN_CAMERA);
        }
    }

    public static GameObject UIRoot
    {
        get
        {
            return GameObject.Find(KDefine.NAME_UI_ROOT);
        }
    }

    public static GameObject ObjectRoot
    {
        get
        {
            return GameObject.Find(KDefine.NAME_OBJECT_ROOT);
        }
    }

    public static GameObject CurrentSceneManager
    {
        get
        {
            return GameObject.Find(KDefine.NAME_SCENE_MANAGER);
        }
    }
    public override void Awake()
    {
        base.Awake();

        this.SetUpUICamera();
        this.SetUpMainCamera();

        //모니터 주사율 0 = 맞추겠다 60으로 설정했을 경우에는 그 미ㅁ억
        QualitySettings.vSyncCount = 0;
        //모니터에 맞추겠다
        Application.targetFrameRate = 60;

        //true로 바꾸면 풀스크린
        Screen.SetResolution(KDefine.SCREEN_WIDTH, KDefine.SCREEN_HEIGHT, false);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        //로딩 씬의 프로그래스 바는 일회성이기 때문에 람다를 이용한다
        //게임은 이벤트로 시작해서 이벤트로 끝나기 때문에 델리게이트를 많이 이용하게 되는데 그것으로 이용한 람다의 사용률이 중요하다
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //CSceneLoader.Instance.LoadScene(0);
            CSceneLoader.Instance.LoadSceneAsync(0, (a_oAsyncOperation) =>
            {
                Debug.LogFormat("Percent : {0}", a_oAsyncOperation.progress);
            });
        }
    }

    protected void SetUpUICamera()
    {
        if(CSceneManager.UICamera != null)
        {
            CSceneManager.UICamera.orthographic = true;
            CSceneManager.UICamera.orthographicSize = (KDefine.SCREEN_HEIGHT / 2.0f) * KDefine.UNIT_SCALE;
        }
    }

    protected void SetUpMainCamera()
    {
        if (CSceneManager.MainCamera != null)
        {
            float fPlaneHeight = (KDefine.SCREEN_HEIGHT / 2.0f) * KDefine.UNIT_SCALE;
            // 사선이기 때문에 tan가 들어감
            float fFieldOfView = Mathf.Atan(fPlaneHeight / m_fPlaneDistance);

            CSceneManager.MainCamera.orthographic = false;
                                                                           //Atan을 쓰면 Rad2Deg를 통해 다시 받아야된다
            CSceneManager.MainCamera.fieldOfView = (fFieldOfView * 2.0f) * Mathf.Rad2Deg;
        }
    }
}
