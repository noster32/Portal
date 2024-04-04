using System;
using System.Collections.Generic;
using UnityEngine;

public class CPortalPlacement : CComponent  
{
    [SerializeField] private CPortalBullet portalBullet;
    [SerializeField] private Transform muzzlePosition;
    [SerializeField] private LayerMask portalGunCheckLayer;
    Vector3 aimPoint;
    private CPortalBullet currentPortalBullet;
    [SerializeField] private CPortalPair portalPair;
    private Transform cameraTransform;

    private Vector3 topLeftPoint     = new Vector3(-0.75f,  1.3f, 0f);
    private Vector3 topRightPoint    = new Vector3( 0.75f,  1.3f, 0f);
    private Vector3 bottomLeftPoint  = new Vector3(-0.75f, -1.3f, 0f);
    private Vector3 bottomRightPoint = new Vector3( 0.75f, -1.3f, 0f);

    private Vector3 centerPointForward = new Vector3(0f, 0f, 0.05f);
    private Vector3 centerPointBackward = new Vector3(0f, 0f, -0.03f);
    Quaternion portalRotation;

    Dictionary<string, Vector3> portalEdgePoints = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> offsetPoints = new Dictionary<string, Vector3>();
    string[] edgeNameArray = new string[] { "TL", "TR", "BR", "BL" };

    public override void Awake()
    {
        base.Awake();

        cameraTransform = Camera.main.transform;

        portalEdgePoints["TL"] = Vector3.zero;
        portalEdgePoints["TR"] = Vector3.zero;
        portalEdgePoints["BL"] = Vector3.zero;
        portalEdgePoints["BR"] = Vector3.zero;
        offsetPoints["TL"] = Vector3.zero;
        offsetPoints["TR"] = Vector3.zero;
        offsetPoints["BL"] = Vector3.zero;
        offsetPoints["BR"] = Vector3.zero;
    }

    public override void Start()
    {
        base.Start();

        if(portalPair == null)
            portalPair = CSceneManager.Instance.portalPair;
    }

    //포탈건 발사
    //레이어가 PortalPlaceable인 경우에만 포탈을 설치
    //포탈에 발사할 경우 다른 색의 포탈이라면
    //마우스 포인터를 기준으로 포탈의 왼쪽이나 오른쪽으로 이동해서 설치
    public void FirePortal(int portalNum)
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 250.0f, portalGunCheckLayer))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PortalPlaceable"))
            {
                Debug.Log("test");
                PortalPlace(hit.point, hit.normal, portalNum);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PortalCollider"))
            {
                if (portalPair.portals[portalNum].otherPortal.tag == hit.collider.tag)
                {
                    Transform hitPortalTransform = portalPair.portals[portalNum].otherPortal.transform;

                    Vector3 relativePos = hitPortalTransform.InverseTransformPoint(hit.point);
                    if (relativePos.x > 0f)
                        relativePos = new Vector3(1.5f, relativePos.y, 0f);
                    else
                        relativePos = new Vector3(-1.5f, relativePos.y, 0f);

                    Vector3 resultPos = hitPortalTransform.TransformPoint(relativePos);

                    PortalPlace(resultPos, hit.normal, portalNum);
                }
                else
                {
                    Transform hitPortalTransform = portalPair.portals[portalNum].transform;

                    Vector3 relativePos = hitPortalTransform.InverseTransformPoint(hit.point);
                    relativePos = new Vector3(relativePos.x, relativePos.y, 0f);
                    Vector3 resultPos = hitPortalTransform.TransformPoint(relativePos);

                    PortalPlace(resultPos, hit.normal, portalNum);
                }
            }
        }
    }

    //포탈 설치
    //천장이나 바닥에 설치한 경우에는 플레이어가 바라보는 방향에 따라서
    //포탈의 로테이션이 바뀌게 함
    //포탈이 밖으로 나가거나 겹치는지 체크하고 모두 통과시 포탈 설치
    private void PortalPlace(Vector3 pos, Vector3 normal, int num)
    {
        float yRotation;
        portalRotation = Quaternion.LookRotation(normal, Vector3.up);
        //portalRotation = Quaternion.FromToRotation(Vector3.forward, normal);

        if (normal == Vector3.up || normal == Vector3.down)
        {
            Quaternion cameraRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            yRotation = cameraRotation.eulerAngles.y;
            portalRotation = Quaternion.Euler(0f, yRotation, 0f) * portalRotation;
        }

        Vector3 portalPos;
        if (FixedOverlap(pos, portalRotation, out portalPos, num))
        {
            if (FixedOverhangs(portalPos, portalRotation, normal, out portalPos))
            {
                portalPair.portals[num].OpenPortal(portalPos, portalRotation);
            }
        }
    }
    private int GetOppositeCorner(string cornerName)
    {
        int index = Array.IndexOf(edgeNameArray, cornerName);
        index = (index + 2) % 4;

        return index;
    }

    private string OverhangLinecast(Vector3 origin, Quaternion rotation, string startPoint, Vector3 endPos)
    {
        RaycastHit hit;
        Vector3 startPos = origin + rotation * portalEdgePoints[startPoint];

        if (Physics.Linecast(startPos, endPos, out hit, LayerMask.GetMask("PortalPlaceable")))
        {
            var worldOffset = hit.point - startPos;
            worldOffset = Quaternion.Inverse(rotation) * worldOffset;

            offsetPoints[startPoint] = worldOffset;
            return startPoint;
        }
        else
            return "TL";
    }

    private Vector3 CompareEdges(List<string> edges, List<string> offsets)
    {
        Vector3 result = Vector3.zero;
        if (portalEdgePoints[edges[0]].x == portalEdgePoints[edges[1]].x)
        {
            float beforeOffsetX = 0f;

            for (int offsetNum = 0; offsetNum < 2; ++offsetNum)
            {
                if (Mathf.Abs(offsetPoints[offsets[offsetNum]].x) > Mathf.Abs(beforeOffsetX))
                    beforeOffsetX = offsetPoints[offsets[offsetNum]].x;
            }

            result = new Vector3(beforeOffsetX, 0f, 0f);
        }
        else if (portalEdgePoints[edges[0]].y == portalEdgePoints[edges[1]].y)
        {
            float beforeOffsetY = 0f;

            for (int offsetNum = 0; offsetNum < 2; ++offsetNum)
            {
                if (Mathf.Abs(offsetPoints[offsets[offsetNum]].y) > Mathf.Abs(beforeOffsetY))
                    beforeOffsetY = offsetPoints[offsets[offsetNum]].y;
            }

            result = new Vector3(0f, beforeOffsetY, 0f);
        }

        return result;
    }

    //포탈 뒤쪽에서 벽 밖으로 나가는지 체크
    private bool FixedOverhangs(Vector3 originPos, Quaternion rotation, Vector3 hitNormal, out Vector3 result)
    {
        portalEdgePoints["TL"] = topLeftPoint + centerPointBackward;
        portalEdgePoints["TR"] = topRightPoint + centerPointBackward;
        portalEdgePoints["BL"] = bottomLeftPoint + centerPointBackward;
        portalEdgePoints["BR"] = bottomRightPoint + centerPointBackward;
        offsetPoints["TL"] = Vector3.zero;
        offsetPoints["TR"] = Vector3.zero;
        offsetPoints["BL"] = Vector3.zero;
        offsetPoints["BR"] = Vector3.zero;

        result = originPos;

        Vector3 offsetObjectOverhang = Vector3.zero;
        List<string> collisionPoint = new List<string>();
        List<string> nonCollisionPoint = new List<string>();

        for(int i = 0; i < 4; ++i)
        {
            Vector3 linecastStartPos = originPos + rotation * portalEdgePoints[edgeNameArray[i]];
            Collider[] colliders = Physics.OverlapSphere(linecastStartPos, 0.1f, LayerMask.GetMask("PortalPlaceable"));

            if(colliders.Length == 0)
            {
                nonCollisionPoint.Add(edgeNameArray[i]);
                continue;
            }

            int count = 0;
            for(int cNum = 0 ; cNum  < colliders.Length; ++cNum)
            {
                if (colliders[cNum].transform.forward == hitNormal)
                {
                    collisionPoint.Add(edgeNameArray[i]);
                    ++count;
                }
            }

            if (count == 0)
                nonCollisionPoint.Add(edgeNameArray[i]);
        }

        if(collisionPoint.Count > 0)
        {
            Vector3 lineCastEndPos = originPos + rotation * centerPointBackward;

            switch (collisionPoint.Count)
            {
                case 4:
                    break;

                case 3:
                    string temp3 = OverhangLinecast(originPos, rotation, nonCollisionPoint[0], lineCastEndPos);
                    offsetObjectOverhang = rotation * new Vector3(offsetPoints[temp3].x, offsetPoints[temp3].y, 0f);
                    break;

                case 2:
                    List<string> offsets = new List<string>();

                    for (int castNum = 0; castNum < 2; ++castNum)
                    {
                        offsets.Add(OverhangLinecast(originPos, rotation, nonCollisionPoint[castNum], lineCastEndPos));
                    }

                    offsetObjectOverhang = rotation * CompareEdges(nonCollisionPoint, offsets);
                    break;

                case 1:
                    int index = GetOppositeCorner(collisionPoint[0]);
                    string temp1 = OverhangLinecast(originPos, rotation, edgeNameArray[index], lineCastEndPos);
                    offsetObjectOverhang = rotation * new Vector3(offsetPoints[temp1].x, offsetPoints[temp1].y, 0f);
                    break;

                case 0:
                    return false;

                default:
                    return false;
            }
        }

        result = originPos + offsetObjectOverhang;
        return true;
    }

    //포탈 앞쪽에서 물체와 겹치는지 체크
    private bool FixedOverlap(Vector3 originPos, Quaternion rotation, out Vector3 result, int portalNum)
    {
        portalEdgePoints["TL"] = topLeftPoint + centerPointForward;
        portalEdgePoints["TR"] = topRightPoint + centerPointForward;
        portalEdgePoints["BL"] = bottomLeftPoint + centerPointForward;
        portalEdgePoints["BR"] = bottomRightPoint + centerPointForward;
        offsetPoints["TL"] = Vector3.zero;
        offsetPoints["TR"] = Vector3.zero;
        offsetPoints["BL"] = Vector3.zero;
        offsetPoints["BR"] = Vector3.zero;

        result = Vector3.zero;
        Vector3 offsetObjectOverlap = Vector3.zero;
        List<string> collisionPoint = new List<string>();
        List<string> nonCollisionPoint = new List<string>();

        //포탈의 중심에서 부터 라인캐스트를 실행
        //오브젝트나 벽에 맞은 경우 각각의 리스트에 맞은 위치를 포탈의 로컬 위치로 저장
        RaycastHit hitObject;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 linecastStartPos = originPos + rotation * centerPointForward;
            Vector3 linecastEndPos = originPos + rotation * portalEdgePoints[edgeNameArray[i]];
            int interactLayer = LayerMask.GetMask("Default", "PortalPlaceable", "NonPortalPlaceable", "Portal");

            if (Physics.Linecast(linecastStartPos, linecastEndPos, out hitObject, interactLayer))
            {
                if (hitObject.transform.gameObject.layer == LayerMask.NameToLayer("Portal"))
                    if (portalPair.portals[portalNum].tag == hitObject.collider.tag)
                    {
                        nonCollisionPoint.Add(edgeNameArray[i]);
                        continue;
                    }

                Vector3 worldOffset = hitObject.point - linecastEndPos;
                worldOffset = Quaternion.Inverse(rotation) * worldOffset;

                offsetPoints[edgeNameArray[i]] = worldOffset;
                collisionPoint.Add(edgeNameArray[i]);
            }
            else
                nonCollisionPoint.Add(edgeNameArray[i]);
        }

        if (collisionPoint.Count > 0)
        {
            switch(collisionPoint.Count)
            {
                case 4:
                    return false;

                case 3:
                    //논콜리전에 저장해 놓은 name반대편에 있는걸 오프셋으로 사용
                    int index = GetOppositeCorner(nonCollisionPoint[0]);
                    Vector3 temp3 = new Vector3(offsetPoints[edgeNameArray[index]].x, offsetPoints[edgeNameArray[index]].y, 0f);
                    offsetObjectOverlap = rotation * temp3;
                    break;

                case 2:
                    offsetObjectOverlap = rotation * CompareEdges(collisionPoint, collisionPoint);
                    break;

                case 1:
                    //offsetPoints 오프셋 사용
                    Vector3 temp1 = new Vector3(offsetPoints[collisionPoint[0]].x, offsetPoints[collisionPoint[0]].y, 0f);
                    offsetObjectOverlap = rotation * temp1;
                    break;

                default:
                    return false;
            }
        }       

        //벽과 오브젝트의 오프셋을 합쳐서 결과 대입
        result = originPos + offsetObjectOverlap;
        return true;
    }
}
