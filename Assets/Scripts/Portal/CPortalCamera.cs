using UnityEngine;

public class CPortalCamera : CComponent
{
    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private Camera portalCamera;

    [SerializeField] private Color colorBlue;
    [SerializeField] private Color colorOrange;

    private Texture2D defaultTexture1;                      //포탈이 활성화 되지 않았을 때 텍스쳐
    private Texture2D defaultTexture2;
    private RenderTexture portalCameraTexture1;             //포탈이 활성화 되어서 카메라를 보여주는 텍스처
    private RenderTexture portalCameraTexture2;

    private const int iterations = 7;                        //포탈 카메라 반복 횟수

    private Camera mainCamera;

    public override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;

        portalCameraTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        portalCameraTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

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
            portalPair.portals[0].SetTexture(portalCameraTexture1);
            portalPair.portals[1].SetTexture(portalCameraTexture2);
        }

        if (portalPair.portals[0].isVisibleFromMainCamera(mainCamera))
        {
            portalCamera.targetTexture = portalCameraTexture1;

            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portalPair.portals[0], portalPair.portals[1], i);
            }
        }

        if (portalPair.portals[1].isVisibleFromMainCamera(mainCamera))
        {
            portalCamera.targetTexture = portalCameraTexture2;
        
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portalPair.portals[1], portalPair.portals[0], i);
            }
        }

    }


    //렌더링 카메라
    //플레이어의 카메라 현재 위치와 동일하게 다른 포탈쪽으로 포탈 카메라를 이동시킴
    //포탈 카메라는 포탈을 기준으로 벽을 클리핑하여 포탈 뒤에있는 오브젝트나 벽들은 보이지 않게 함
    //iteration인자에서 받은 숫자만큼 반복해서 위치를 설정해
    //반복의 제일 나중의 카메라부터 렌더링을 시켜 포탈이 연속적으로 보이게 한다
    private void RenderCamera(CPortal lookPortal, CPortal otherPortal, int iteration)
    {
        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iteration; ++i)
        {
            cameraTransform.position = lookPortal.GetOtherPortalRelativePoint(cameraTransform.position);
            cameraTransform.rotation = lookPortal.GetOtherPortalRelativeRotation(cameraTransform.rotation);
        }
        
        Plane p = new Plane(otherPortal.transform.forward, otherPortal.transform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        portalCamera.Render();
    }
}
