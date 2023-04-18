using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PortalCamera : CComponent
{
    //기본 포탈
    [SerializeField]
    private Portal[] portals = new Portal[2];

    //포탈 카메라
    [SerializeField]
    private Camera portalCamera;

    //포털 표면을 그리기 위한 메테리얼
    [SerializeField]
    private Material portalMaterial;

    //플레이어 카메라
    private Camera mainCamera;

    private RenderTexture tempTexture;

    private const int maskID1 = 1;
    private const int maskID2 = 2;

    public override void Awake()
    {
        base.Awake();

        mainCamera = GetComponent<Camera>();
        tempTexture = new RenderTexture(Screen.width, Screen.height, 24);

        portalCamera.targetTexture = tempTexture;
    }

    public override void Start()
    {
        base.Start();

        portals[0].SetMaskID(maskID1);
        portals[1].SetMaskID(maskID2);
    }

    public override void Update()
    {
        base.Update();
    }

    private void RenderCamera(Portal renderPortal, Portal cameraPortal)
    {
        Transform renderPortlaTransform = renderPortal.transform;
        Transform cameraPortalTransform = cameraPortal.transform;

        // 반대 포탈의 뒤로 카메라 위치 조정
        Vector3 relativePos = renderPortlaTransform.InverseTransformPoint(transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        portalCamera.transform.position = cameraPortalTransform.TransformPoint(relativePos);

        // 바라보는 방향을 반대 포탈을 기준으로 카메라 설정
        Quaternion relativeRot = Quaternion.Inverse(renderPortlaTransform.rotation) * transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        portalCamera.transform.rotation = cameraPortalTransform.rotation * relativeRot;


        Plane p = new Plane(-cameraPortalTransform.forward, cameraPortalTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        portalCamera.Render();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (portals[0].IsRendererVisible())
        {
            RenderCamera(portals[0], portals[1]);
            portalMaterial.SetInt("_MaskID", maskID1);
            Graphics.Blit(tempTexture, source, portalMaterial);
        }

        if (portals[1].IsRendererVisible())
        {
            RenderCamera(portals[1], portals[0]);
            portalMaterial.SetInt("_MaskID", maskID2);
            Graphics.Blit (tempTexture, source, portalMaterial);
        }

        Graphics.Blit(source, destination);
    }
}
