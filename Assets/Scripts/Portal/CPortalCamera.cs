using UnityEngine;

public class CPortalCamera : CComponent
{
    #region private
    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private Camera portalCamera;

    [SerializeField] private Color colorBlue;
    [SerializeField] private Color colorOrange;

    private Texture2D defaultTexture1;
    private Texture2D defaultTexture2;
    private RenderTexture tempTexture1;
    private RenderTexture tempTexture2;

    //포탈 카메라 반복 횟수
    private const int iterations = 7;

    private Camera mainCamera;
    private Quaternion reverse = Quaternion.Euler(0f, 180f, 0f);

    #endregion

    public override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;

        tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

        defaultTexture1 = new Texture2D(1, 1);
        defaultTexture1.SetPixel(0, 0, colorBlue);
        defaultTexture1.Apply();

        defaultTexture2 = new Texture2D(1, 1);
        defaultTexture2.SetPixel(0, 0, colorOrange);
        defaultTexture2.Apply();
    }

    public override void Start()
    {
        base.Start();

        if (portalPair == null)
            portalPair = CSceneManager.Instance.portalPair;

        portalPair.portals[0].SetTexture(defaultTexture1);
        portalPair.portals[1].SetTexture(defaultTexture2);
    }

    private void OnPreRender()
    {
        if (!portalPair.PlacedBothPortal())
        {
            portalPair.portals[0].SetTexture(defaultTexture1);
            portalPair.portals[1].SetTexture(defaultTexture2);
            return;
        }
        else
        {
            portalPair.portals[0].SetTexture(tempTexture1);
            portalPair.portals[1].SetTexture(tempTexture2);
        }

        if (portalPair.portals[0].isVisibleFromMainCamera(mainCamera))
        {
            portalCamera.targetTexture = tempTexture1;
            for(int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portalPair.portals[0], portalPair.portals[1], i);
            }
        }

        if (portalPair.portals[1].isVisibleFromMainCamera(mainCamera))
        {
            portalCamera.targetTexture = tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portalPair.portals[1], portalPair.portals[0], i);
            }
        }

    }

    private void RenderCamera(CPortal lookPortal, CPortal otherPortal, Camera pCamera, int iteration)
    {
        Transform lPortalTransform = lookPortal.transform;
        Transform oPortalTransform = otherPortal.transform;

        //포탈 카메라 트랜스폼 가져오기
        Transform cameraTransform = pCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for(int i = 0; i <= iteration; ++i)
        {
            //포탈에 대한 카메라의 로컬 좌표 구하기
            Vector3 relativeCamPos = lPortalTransform.InverseTransformPoint(transform.position);

            //포탈의 뒤로 이동해야 하기 떄문에 로컬 좌표를 180도 회전
            relativeCamPos = reverse * relativeCamPos;

            //로컬 좌표를 반대쪽 포탈에 적용시켜서 포탈 카메라 위치 설정
            cameraTransform.position = oPortalTransform.TransformPoint(relativeCamPos);

            Quaternion relativeRot = Quaternion.Inverse(lPortalTransform.rotation) * transform.rotation;
            relativeRot = reverse * relativeRot;
            cameraTransform.rotation = oPortalTransform.rotation * relativeRot;
        }

        //포탈이 붙어있는 벽면을 클리핑해서 벽 너머로도 카메라가 보일 수 있게 한다
        Plane p = new Plane(oPortalTransform.forward, oPortalTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(pCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        pCamera.projectionMatrix = newMatrix;
        
        pCamera.Render();
    }

    private void RenderCamera(CPortal lookPortal, CPortal otherPortal, int iteration)
    {
        Transform lPortalTransform = lookPortal.transform;
        Transform oPortalTransform = otherPortal.transform;

        //포탈 카메라 트랜스폼 가져오기
        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iteration; ++i)
        {
            //포탈에 대한 카메라의 로컬 좌표 구하기
            Vector3 relativeCamPos = lPortalTransform.InverseTransformPoint(cameraTransform.position);

            //포탈의 뒤로 이동해야 하기 떄문에 로컬 좌표를 180도 회전
            relativeCamPos = reverse * relativeCamPos;

            //로컬 좌표를 반대쪽 포탈에 적용시켜서 포탈 카메라 위치 설정
            cameraTransform.position = oPortalTransform.TransformPoint(relativeCamPos);

            Quaternion relativeRot = Quaternion.Inverse(lPortalTransform.rotation) * cameraTransform.rotation;
            relativeRot = reverse * relativeRot;
            cameraTransform.rotation = oPortalTransform.rotation * relativeRot;
        }

        //포탈이 붙어있는 벽면을 클리핑해서 벽 너머로도 카메라가 보일 수 있게 한다
        Plane p = new Plane(oPortalTransform.forward, oPortalTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        portalCamera.Render();
    }
}
