using System;
using System.Collections.Generic;
using UnityEngine;
public class CPortalPlacement : CComponent  
{
    [SerializeField] private CPortalBullet portalBullet;
    [SerializeField] private Transform muzzlePosition;
    Vector3 aimPoint;
    private CPortalBullet currentPortalBullet;


    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private LayerMask portalGunCheckLayer;             //포탈 건을 발사 했을 때 체크 할 레이어

    private float portalWidth = 1.5f;
    private float portalHeight = 2.6f;
    private float zFightingOffset = 0.005f;

    private Vector3 topLeftPoint;
    private Vector3 topRightPoint;
    private Vector3 bottomLeftPoint;
    private Vector3 bottomRightPoint;

    private Vector3 centerPointForward = new Vector3(0f, 0f, 0.05f);
    private Vector3 centerPointBackward = new Vector3(0f, 0f, -0.03f);

    Dictionary<string, Vector3> portalEdgePoints = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> offsetPoints = new Dictionary<string, Vector3>();
    string[] edgeNameArray = new string[] { "TL", "TR", "BR", "BL" };

    private Vector3 hitPoint;
    
    private Transform cameraTransform;

    public override void Awake()
    {
        base.Awake();

        cameraTransform = Camera.main.transform;

        topLeftPoint     =   new Vector3(-portalWidth / 2,  portalHeight / 2, 0f);
        topRightPoint    =   new Vector3( portalWidth / 2,  portalHeight / 2, 0f);
        bottomLeftPoint  =   new Vector3(-portalWidth / 2, -portalHeight / 2, 0f);
        bottomRightPoint =   new Vector3( portalWidth / 2, -portalHeight / 2, 0f);

        for(int i = 0; i < edgeNameArray.Length; ++i)
        {
            portalEdgePoints.Add(edgeNameArray[i], Vector3.zero);
            offsetPoints.Add(edgeNameArray[i], Vector3.zero);
        }
    }

    public override void Start()
    {
        base.Start();

        if(portalPair == null)
            portalPair = CSceneManager.Instance.portalPair;
    }

    private void OnDrawGizmos()
    {
        if(hitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hitPoint, 0.2f);
        }
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
                hitPoint = hit.point;
                hitPoint += hit.normal * zFightingOffset;
                PortalPlace(hitPoint, hit.normal, portalNum);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PortalCollider"))
            {
                if (portalPair.portals[portalNum].otherPortal.CompareTag(hit.collider.tag))
                {
                    Transform hitPortalTransform = portalPair.portals[portalNum].otherPortal.transform;

                    Vector3 relativePos = hitPortalTransform.InverseTransformPoint(hit.point);
                    if (relativePos.x > 0f)
                        relativePos = new Vector3(portalWidth, relativePos.y, 0f);
                    else
                        relativePos = new Vector3(-portalWidth, relativePos.y, 0f);

                    relativePos += hit.normal * zFightingOffset;
                    Vector3 resultPos = hitPortalTransform.TransformPoint(relativePos);

                    PortalPlace(resultPos, hit.normal, portalNum);
                }
                else
                {
                    Transform hitPortalTransform = portalPair.portals[portalNum].transform;

                    Vector3 relativePos = hitPortalTransform.InverseTransformPoint(hit.point);
                    relativePos = new Vector3(relativePos.x, relativePos.y, 0f);

                    relativePos += hit.normal * zFightingOffset;
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
        Quaternion portalRotation = Quaternion.LookRotation(normal, Vector3.up);

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
                portalPos += portalPair.portals[num].transform.forward * zFightingOffset;
                portalPair.portals[num].OpenPortal(portalPos, portalRotation);
            }
        }
    }

    //건너편 모서리의 인덱스 가져오기
    private int GetOppositeCorner(string cornerName)
    {
        int index = Array.IndexOf(edgeNameArray, cornerName);
        index = (index + 2) % 4;

        return index;
    }

    //라인캐스트
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

    //모서리의 가로 세로 비교
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
    //각 모서리에서 OverlapSphere를 사용해서 벽이 있는지 없는지 확인하고
    //있는경우 collisionPoint.Add 없는경우 nonCollisionPoint.Add
    //충돌된 포인트를 바탕으로 라인캐스트를 실행해서 오프셋을 설정한다
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
            
            if (collisionPoint.Count <= 0)
                return false;
            if (collisionPoint.Count == 3) 
            {
                string temp3 = OverhangLinecast(originPos, rotation, nonCollisionPoint[0], lineCastEndPos);
                offsetObjectOverhang = rotation * new Vector3(offsetPoints[temp3].x, offsetPoints[temp3].y, 0f);
            }
            if (collisionPoint.Count == 2)
            {
                List<string> offsets = new List<string>();

                for (int castNum = 0; castNum < 2; ++castNum)
                {
                    offsets.Add(OverhangLinecast(originPos, rotation, nonCollisionPoint[castNum], lineCastEndPos));
                }

                offsetObjectOverhang = rotation * CompareEdges(nonCollisionPoint, offsets);
            }
            if (collisionPoint.Count == 1)
            {
                int index = GetOppositeCorner(collisionPoint[0]);
                string temp1 = OverhangLinecast(originPos, rotation, edgeNameArray[index], lineCastEndPos);
                offsetObjectOverhang = rotation * new Vector3(offsetPoints[temp1].x, offsetPoints[temp1].y, 0f);
            }
        }

        result = originPos + offsetObjectOverhang;
        return true;
    }

    //포탈 앞쪽에서 물체와 겹치는지 체크
    //포탈의 중심에서 부터 각 모서리로 라인캐스트를 실행                                                                        
    //오브젝트에 맞은 경우 collisionPoint에 추가하고 offsetPoint에 오프셋을 저장                                        
    //충돌한 수에 따라서 오프셋을 설정                                                                                          
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
                                                                                                                                
        RaycastHit hitObject;                                                                                                   
        for (int i = 0; i < 4; ++i)                                                                                             
        {                                                                                                                       
            Vector3 linecastStartPos = originPos + rotation * centerPointForward;                                               
            Vector3 linecastEndPos = originPos + rotation * portalEdgePoints[edgeNameArray[i]];                                 
            int interactLayer = LayerMask.GetMask("Default", "PortalPlaceable", "NonPortalPlaceable", "Portal");                
                                                                                                                                
            if (Physics.Linecast(linecastStartPos, linecastEndPos, out hitObject, interactLayer))                               
            {                                                                                                                   
                if (hitObject.transform.gameObject.layer == LayerMask.NameToLayer("Portal"))                                    
                    if (portalPair.portals[portalNum].CompareTag(hitObject.collider.tag))                                            
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
                    int index = GetOppositeCorner(nonCollisionPoint[0]);                                                        
                    Vector3 temp3 = new Vector3(offsetPoints[edgeNameArray[index]].x, offsetPoints[edgeNameArray[index]].y, 0f);
                    offsetObjectOverlap = rotation * temp3;
                    break;

                case 2:
                    offsetObjectOverlap = rotation * CompareEdges(collisionPoint, collisionPoint);
                    break;

                case 1:
                    Vector3 temp1 = new Vector3(offsetPoints[collisionPoint[0]].x, offsetPoints[collisionPoint[0]].y, 0f);
                    offsetObjectOverlap = rotation * temp1;
                    break;

                default:
                    return false;
            }
        }       

        result = originPos + offsetObjectOverlap;
        return true;
    }
}
