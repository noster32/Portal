using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneManager : CComponent
{

    #region public

    //������ �� ������ ���� ���� �������� ������ ���ؼ�
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

        //����� �ֻ��� 0 = ���߰ڴ� 60���� �������� ��쿡�� �� �̤���
        QualitySettings.vSyncCount = 0;
        //����Ϳ� ���߰ڴ�
        Application.targetFrameRate = 60;

        //true�� �ٲٸ� Ǯ��ũ��
        Screen.SetResolution(KDefine.SCREEN_WIDTH, KDefine.SCREEN_HEIGHT, false);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        //�ε� ���� ���α׷��� �ٴ� ��ȸ���̱� ������ ���ٸ� �̿��Ѵ�
        //������ �̺�Ʈ�� �����ؼ� �̺�Ʈ�� ������ ������ ��������Ʈ�� ���� �̿��ϰ� �Ǵµ� �װ����� �̿��� ������ ������ �߿��ϴ�
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
            // �缱�̱� ������ tan�� ��
            float fFieldOfView = Mathf.Atan(fPlaneHeight / m_fPlaneDistance);

            CSceneManager.MainCamera.orthographic = false;
                                                                           //Atan�� ���� Rad2Deg�� ���� �ٽ� �޾ƾߵȴ�
            CSceneManager.MainCamera.fieldOfView = (fFieldOfView * 2.0f) * Mathf.Rad2Deg;
        }
    }
}
