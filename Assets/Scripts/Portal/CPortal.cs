using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortal : CComponent
{
    [Header("Setting")]
    public CPortal otherPortal;                                                                     //반대 쪽 포탈
    [SerializeField] private Renderer outlineRenderer;                                              //포탈 테두리 렌더러
    [SerializeField] private Color portalColor;                                                     //포탈 색
    [SerializeField] private LayerMask teleportObjectMask;                                          //텔레포트 가능한 오브젝트 레이어 마스크
    [SerializeField] private CPortalIgnoreCollision portalIgnoreCollision;                          //컴포넌트

    private List<CTeleportObject> teleportableObjects = new List<CTeleportObject>();   //텔레포트 가능한 범위 안에 있는 오브젝트 리스트

    private Material material;
    private Renderer portalRenderer;
    private BoxCollider boxCollider;

    private Coroutine lerpCoroutine;                                                                //포탈 열리거나 닫힐 때 스케일 조정 코루틴
    private bool isPlaced = false;        

    public override void Awake()
    {
        base.Awake();

        boxCollider = GetComponent<BoxCollider>();
        portalRenderer = GetComponent<Renderer>();
        material = portalRenderer.material;
    }

    public override void Start()
    {
        base.Start();

        SetColor(portalColor);
    }

    //teleportObjects가 포탈과의 내적이 -가 되었을 떄 텔레포트하고 리스트에서 제거한다
    public override void Update()
    {
        base.Update();

        if (!isPlaced || !otherPortal.isPlaced)
            return;

        for (int i = 0; i < teleportableObjects.Count; ++i)
        {
            Vector3 offsetFromPortal;
            offsetFromPortal = (teleportableObjects[i].transform.position + teleportableObjects[i].objectCenter) - transform.position;

            int dotValue = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            if (dotValue <= 0f)
            {
                teleportableObjects[i].Teleport();
                teleportableObjects[i].EnterPortalIgnoreCollision(otherPortal.portalIgnoreCollision.GetWallList());
                teleportableObjects[i].ExitPortal();
                teleportableObjects.RemoveAt(i);
                i--;
            }
        }
    }

    //텔레포트 가능한 범위 안에 들어왔을 경우 리스트에 추가, 오브젝트의 클론 active
    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaced || !otherPortal.isPlaced)
            return;

        CTeleportObject tpObject;
        if (other.CompareTag("Turret"))
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();

        if (tpObject != null)
        {
            teleportableObjects.Add(tpObject);
            tpObject.EnterPortal(this);
        }
    }

    //텔레포트 가능한 범위를 벗어났을 경우 리스트에서 제거, 오브젝트의 클론 disable
    private void OnTriggerExit(Collider other)
    {
        if (!isPlaced || !otherPortal.isPlaced)
            return;

        CTeleportObject tpObject;
        if (other.CompareTag("Turret"))
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();

        if (teleportableObjects.Contains(tpObject))
        {
            teleportableObjects.Remove(tpObject);
            tpObject.ExitPortal();
        }
    }

    //포탈을 설치했을 때 텔레포트 가능한 구역 안에 오브젝트가 있는 경우 리스트에 추가
    //주로 오브젝트 밑에 포탈을 설치했을 경우 사용된다
    private void PortalTeleportAreaCheck()
    {
        Collider[] collider = Physics.OverlapBox(transform.TransformPoint(boxCollider.center), boxCollider.size / 2, transform.rotation, teleportObjectMask);

        if (collider.Length > 0)
        {
            foreach (Collider col in collider)
            {
                if (col.CompareTag("Turret"))
                {
                    CTeleportObject obj = col.transform.GetComponentInParent<CTeleportObject>();

                    if (obj != null)
                    {
                        teleportableObjects.Add(obj);
                        obj.EnterPortal(this);
                    }
                }
                else
                {
                    if (col.transform.TryGetComponent(out CTeleportObject obj))
                    {
                        teleportableObjects.Add(obj);
                        obj.EnterPortal(this);
                    }
                }
            }
        }
    }

    //포탈 설치
    //포탈이 설치 되어있는 경우 설치되어 있던 벽면의 리스트 제거
    //포탈이 열리는 중이였을 경우 중단하고 새로 시작
    public void OpenPortal(Vector3 pos, Quaternion rot)
    {
        if (isPlaced)
        {
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalClose, this.transform.position);
            isPlaced = false;
            portalIgnoreCollision.ClearPlacedWallList();
        }

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }

        transform.position = pos;
        transform.rotation = rot;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        portalIgnoreCollision.StartGetPlacedWallCollider(this.transform);

        PortalTeleportAreaCheck();
        portalIgnoreCollision.PlaceInsidePortalAreaCheck(this.transform);

        lerpCoroutine = StartCoroutine(LerpPortal(0.3f, Vector3.zero, Vector3.one, true));

        //포탈 설치 사운드
        if (CompareTag("PortalB"))
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalOpen1, this.transform.position);
        else if (CompareTag("PortalO"))
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalOpen3, this.transform.position);
    }

    //포탈 닫기
    public void ClosePortal()
    {
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalClose, this.transform.position);

        isPlaced = false;
        lerpCoroutine = StartCoroutine(LerpPortal(0.1f, transform.localScale, Vector3.zero, false));
    }

    //placed : 포탈을 설치하려는 경우 true 그렇지 않은 경우 false
    private IEnumerator LerpPortal(float duration, Vector3 start, Vector3 end, bool place)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transform.localScale = Vector3.Lerp(start, end, t);
            timeElapsed += Time.fixedDeltaTime;

            yield return null;
        }

        transform.localScale = end;

        if (place)
        {
            isPlaced = true;
            portalIgnoreCollision.ObjectsIgnoreCollision();
            otherPortal.portalIgnoreCollision.ObjectsIgnoreCollision();
        }
        else
            gameObject.SetActive(false);

        lerpCoroutine = null;
    }

    public void SetTexture(Texture texture)
    {
        material.mainTexture = texture;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void SetColor(Color color)
    {
        outlineRenderer.material.SetColor("_OutlineColor", color);
    }

    //포탈이 카메라에 보이는지 안보이는지 체크
    public bool isVisibleFromMainCamera(Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        return GeometryUtility.TestPlanesAABB(planes, portalRenderer.bounds);
    }


    //건너편 포탈에서의 현재 위치
    public Vector3 GetOtherPortalRelativePoint(Vector3 origin)
    {
        Vector3 relativePos = transform.InverseTransformPoint(origin);
        relativePos = Quaternion.Euler(0f, 180f, 0f) * relativePos;
        Vector3 result = otherPortal.transform.TransformPoint(relativePos);

        return result;
    }

    //건너편 포탈에서의 현재 로테이션
    public Vector3 GetOtherPortalRelativeDirection(Vector3 origin)
    {
        Vector3 relativeDir = transform.InverseTransformDirection(origin);
        relativeDir = Quaternion.Euler(0f, 180f, 0f) * relativeDir;
        Vector3 result = otherPortal.transform.TransformDirection(relativeDir);

        return result;
    }

    //건너편 포탈에서의 현재 방향
    public Quaternion GetOtherPortalRelativeRotation(Quaternion origin)
    {
        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * origin;
        relativeRot = Quaternion.Euler(0f, 180f, 0f) * relativeRot;
        return otherPortal.transform.rotation * relativeRot;
    }

    //포지션과 포탈의 내적 return
    public int GetPortalDotValue(Vector3 position)
    {
        var offset = (position - transform.position).normalized;
        int dot = System.Math.Sign(Vector3.Dot(offset, transform.forward));

        return dot;
    }
}
