using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CPortalPlacement : CComponent  
{
    #region private
    [SerializeField] private CPortalBullet portalBullet;
    [SerializeField] private Transform muzzlePosition;
    [SerializeField] private LayerMask portalGunCheckLayer;
    Vector3 aimPoint;
    private CPortalBullet currentPortalBullet;
    #endregion

    [SerializeField] private CPortalPair portalPair;

    private Transform cameraTransform;

    Quaternion portalRotation;

    public override void Awake()
    {
        base.Awake();

        cameraTransform = Camera.main.transform;
    }
    public override void Start()
    {
        base.Start();

        if(portalPair == null)
            portalPair = CSceneManager.Instance.portalPair;
    }

    public void FirePortal(int portalNum)
    {
        if(currentPortalBullet != null)
        {
            currentPortalBullet.DeleteBullet();
        }
        int excludeLayer = ~LayerMask.GetMask("Player", "InvisiblePlayer");

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 250.0f, portalGunCheckLayer))
        {
            aimPoint = hit.point;
        }

        Vector3 aimDir = (aimPoint - muzzlePosition.position).normalized;

        //currentPortalBullet = Instantiate(portalBullet, muzzlePosition.position, Quaternion.LookRotation(aimDir, Vector3.forward));

        if(hit.collider)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PortalPlaceable"))
            {
                PortalPlace(hit, portalNum);
            }
            else if(hit.transform.gameObject.layer == LayerMask.NameToLayer("PortalCollider"))
            {
                int otherPortalNum = portalNum == 0 ? 1 : 0;

                if (portalPair.portals[otherPortalNum].tag == hit.collider.tag)
                {
                    Transform hitPortalTransform = portalPair.portals[otherPortalNum].transform;

                    Vector3 relativePos = hitPortalTransform.InverseTransformPoint(hit.point);
                    //Debug.DrawLine(transform.position, hit.point, Color.green, 3f);

                    if (relativePos.x > 0f)
                    {
                        relativePos = new Vector3(1.5f, relativePos.y, 0f);
                    }
                    else
                    {
                        relativePos = new Vector3(-1.5f, relativePos.y, 0f);
                    }

                    Vector3 resultPos = hitPortalTransform.TransformPoint(relativePos);
                    //Debug.DrawLine(portalPair.portals[hitPortalNum].transform.position, resultPos, Color.magenta, 3f);

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

    private void PortalPlace(RaycastHit h, int num, Vector3 offsetPos = default(Vector3))
    {
        float yRotation;
        portalRotation = Quaternion.LookRotation(h.normal, Vector3.up);

        if (h.normal == Vector3.up || h.normal == Vector3.down)
        {
            //플레이어 로테이션 가져오기
            Quaternion cameraRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            yRotation = cameraRotation.eulerAngles.y;

            portalRotation = Quaternion.Euler(0f, yRotation, 0f) * portalRotation;
        }

        Vector3 portalPos;

        if(FixedOverhangs(h.point + offsetPos, portalRotation, h.normal, out portalPos))
        {
            if(FixedOverlap(portalPos, portalRotation, out portalPos, num))
            {
                portalPair.portals[num].PlacePortal(portalPos, portalRotation);
            }
        }
        
        //포탈이 설치되지 않을경우의 사운드
    }

    private void PortalPlace(Vector3 pos, Vector3 normal, int num)
    {
        float yRotation;
        portalRotation = Quaternion.FromToRotation(Vector3.forward, normal);

        if (normal == Vector3.up || normal == Vector3.down)
        {
            //플레이어 로테이션 가져오기
            Quaternion cameraRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            yRotation = cameraRotation.eulerAngles.y;

            portalRotation = Quaternion.Euler(0f, yRotation, 0f) * portalRotation;
        }

        Vector3 portalPos;
        if(FixedOverhangs(pos, portalRotation, normal, out portalPos))
        {
            Debug.Log("Overhangs test");
            if (FixedOverlap(portalPos, portalRotation, out portalPos, num))
            {
                Debug.Log("Overlap test");
                portalPair.portals[num].PlacePortal(portalPos, portalRotation);
            }
        }
    }

    //포탈 뒤쪽에서 벽 밖으로 나가는지 체크
    private bool FixedOverhangs(Vector3 originPos, Quaternion rotation, Vector3 hitNormal, out Vector3 result)
    {
        List<Vector3> portalEdgePoint = new List<Vector3>               //포탈의 꼭짓점
        {
            new Vector3(-0.75f,  1.3f, -0.03f),
            new Vector3( 0.75f,  1.3f, -0.03f),
            new Vector3( 0.75f, -1.3f, -0.03f),
            new Vector3(-0.75f, -1.3f, -0.03f)
        };

        List<Vector3> pointsEndPosition = new List<Vector3>             //각 꼭짓점의 역방향
        {
            new Vector3( 0.75f, -1.3f, -0.03f),
            new Vector3(-0.75f, -1.3f, -0.03f),
            new Vector3(-0.75f,  1.3f, -0.03f),
            new Vector3( 0.75f,  1.3f, -0.03f)
        };

        result = Vector3.zero;

        Vector3 offset = Vector3.zero;
        List<Vector3> offsets = new List<Vector3>();
        List<int> collideNum = new List<int>();

        for(int i = 0; i < 4; ++i)
        {
            Vector3 linecastStartPos = originPos + rotation * portalEdgePoint[i];
            Collider[] colliders = Physics.OverlapSphere(linecastStartPos, 0.1f, LayerMask.GetMask("PortalPlaceable"));
            foreach (Collider c in colliders)
            {
                if (c.transform.forward == hitNormal)
                {
                    collideNum.Add(i);
                }
            }
        }

        if(collideNum.Count > 0 && collideNum.Count < 4)
        {
            RaycastHit hit;
            for(int i = 0; i < collideNum.Count; ++i)
            {
                Vector3 linecastStartPos = originPos + rotation * portalEdgePoint[collideNum[i]];
                Vector3 lineCastEndPos = originPos + rotation * pointsEndPosition[collideNum[i]];
                
                if (Physics.Linecast(lineCastEndPos, linecastStartPos, out hit, LayerMask.GetMask("PortalPlaceable")))
                {
                    var worldOffset = hit.point - lineCastEndPos;
                    worldOffset = Quaternion.Inverse(rotation) * worldOffset;
                    offsets.Add(worldOffset);
                }
            }
        }

        if(offsets.Count > 0)
        {
            if (offsets.Count == 1)
            {
                offset = rotation * offsets[0];
            }
            else
            {
                float beforeOffsetX = 0f;
                float beforeOffsetY = 0f;

                foreach (Vector3 vec in offsets)
                {
                    if (Mathf.Abs(vec.x) > Mathf.Abs(beforeOffsetX))
                        beforeOffsetX = vec.x;
                    if (Mathf.Abs(vec.y) > Mathf.Abs(beforeOffsetY))
                        beforeOffsetY = vec.y;
                }

                if (Mathf.Abs(beforeOffsetX) > Mathf.Abs(beforeOffsetY))
                {
                    offset = new Vector3(beforeOffsetX, 0f, 0f);
                }
                else
                {
                    offset = new Vector3(0f, beforeOffsetY, 0f);
                }

                offset = rotation * offset;
            }
        }

        result = originPos + offset;
        return true;
    }

    //포탈 앞쪽에서 물체와 겹치는지 체크
    private bool FixedOverlap(Vector3 originPos, Quaternion rotation, out Vector3 result, int portalNum)
    {
        List<Vector3> portalEdgePoint = new List<Vector3>               //포탈의 4개의 꼭짓점
        {
            new Vector3(-0.75f,  1.3f, 0.05f),
            new Vector3( 0.75f,  1.3f, 0.05f),
            new Vector3( 0.75f, -1.3f, 0.05f),
            new Vector3(-0.75f, -1.3f, 0.05f)
        };

        Vector3 centerPoint = new Vector3(0f, 0f, 0.05f);               //포탈 중앙
        result = Vector3.zero;

        Vector3 offsetObjectOverlap = Vector3.zero;
        Vector3 offsetWallOverlap = Vector3.zero;
        Vector3 offsetPortalOverlap = Vector3.zero;

        List<Vector3> hitPointsObject = new List<Vector3>();
        List<Vector3> hitPointsWall = new List<Vector3>();
        List<Vector3> hitPointsPortal = new List<Vector3>();

        //포탈의 중심에서 부터 라인캐스트를 실행
        //오브젝트나 벽에 맞은 경우 각각의 리스트에 맞은 위치를 포탈의 로컬 위치로 저장
        RaycastHit hitObject;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 linecastStartPos = originPos + rotation * centerPoint;
            Vector3 linecastEndPos = originPos + rotation * portalEdgePoint[i];
            int excludeLayer = ~LayerMask.GetMask("InteractionObject");

            if (Physics.Linecast(linecastStartPos, linecastEndPos, out hitObject, excludeLayer))
            {
                Debug.DrawLine(linecastStartPos, hitObject.point, Color.blue, 10f);
                Vector3 worldOffset = hitObject.point - linecastEndPos;
                worldOffset = Quaternion.Inverse(rotation) * worldOffset;

                if (hitObject.transform.gameObject.layer == LayerMask.NameToLayer("PortalPlaceable"))
                    hitPointsWall.Add(worldOffset);
                else if (hitObject.transform.gameObject.layer == LayerMask.NameToLayer("PortalCollider"))
                {
                    if(portalPair.portals[portalNum].tag != hitObject.transform.tag)
                        hitPointsPortal.Add(worldOffset);
                }
                else
                    hitPointsObject.Add(worldOffset);
            }
        }

        //오브젝트에 맞은 거리를 통해서 오프셋 계산
        if(hitPointsObject.Count > 0)
        {
            //hitPointsObject가 4개인 경우에는 사방이 막혀있단 뜻이기 때문에 설치 불가능
            if(hitPointsObject.Count == 4)
            {
                return false;
            }

            float beforeOffsetX = 0.75f;
            float beforeOffsetY = 1.3f;

            foreach (Vector3 vec in hitPointsObject)
            {
                if (Mathf.Abs(vec.x) < Mathf.Abs(beforeOffsetX))
                    beforeOffsetX = vec.x;
                if (Mathf.Abs(vec.y) < Mathf.Abs(beforeOffsetY))
                    beforeOffsetY = vec.y;
            }

            if (Mathf.Abs(beforeOffsetX) < Mathf.Abs(beforeOffsetY))
            {
                offsetObjectOverlap = new Vector3(beforeOffsetX, 0f, 0f);
            }
            else
            {
                offsetObjectOverlap = new Vector3(0f, beforeOffsetY, 0f);
            }

            offsetObjectOverlap = rotation * offsetObjectOverlap;
        }

        //벽에 맞은 거리를 통해서 오프셋 계산
        if (hitPointsWall.Count > 0)
        {
            if(hitPointsWall.Count == 4)
            {
                return false;      
            }

            float beforeOffsetX = 0f;
            float beforeOffsetY = 0f;

            foreach (Vector3 vec in hitPointsWall)
            {
                if (Mathf.Abs(vec.x) > Mathf.Abs(beforeOffsetX))
                    beforeOffsetX = vec.x;
                if (Mathf.Abs(vec.y) > Mathf.Abs(beforeOffsetY))
                    beforeOffsetY = vec.y;
            }

            if (Mathf.Abs(beforeOffsetX) < Mathf.Abs(beforeOffsetY))
            {
                offsetWallOverlap = new Vector3(beforeOffsetX, 0f, 0f);
            }
            else
            {
                offsetWallOverlap = new Vector3(0f, beforeOffsetY, 0f);
            }

            offsetWallOverlap = rotation * offsetWallOverlap;
        }

        if (hitPointsPortal.Count > 0)
        {
            float beforeOffsetX = 0f;
            float beforeOffsetY = 0f;

            foreach (Vector3 vec in hitPointsPortal)
            {
                if (Mathf.Abs(vec.x) > Mathf.Abs(beforeOffsetX))
                    beforeOffsetX = vec.x;
                if (Mathf.Abs(vec.y) > Mathf.Abs(beforeOffsetY))
                    beforeOffsetY = vec.y;
            }

            if (Mathf.Abs(beforeOffsetX) > Mathf.Abs(beforeOffsetY))
            {
                offsetPortalOverlap = new Vector3(beforeOffsetX, 0f, 0f);
            }
            else
            {
                offsetPortalOverlap = new Vector3(0f, beforeOffsetY, 0f);
            }

            offsetPortalOverlap = rotation * offsetPortalOverlap;
        }

        //벽과 오브젝트의 오프셋을 합쳐서 결과 대입
        result = originPos + offsetObjectOverlap + offsetWallOverlap + offsetPortalOverlap;
        return true;
    }
}
