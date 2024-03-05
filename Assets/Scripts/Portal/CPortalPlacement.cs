using System.Collections.Generic;
using UnityEngine;

public class CPortalPlacement : CComponent  
{
    #region private
    [SerializeField] private CPortalBullet portalBullet;
    [SerializeField] private Transform muzzlePosition;
    [SerializeField] private LayerMask placementMask;
    Vector3 aimPoint;

    private CPortalBullet currentPortalBullet;
    #endregion

    [SerializeField] private CPortalPair portalPair;

    private Transform cameraTransform;

    Quaternion portalRotation;

    private class PointData
    {
        public Vector3 point { get; set; }
        public int pointNum { get; set; }
    }

    private class DirectionData
    {
        public Vector3 dirX { get; set; }
        public Vector3 dirY { get; set; }
    }

    public override void Awake()
    {
        base.Awake();

        cameraTransform = Camera.main.transform;
    }

    public void FirePortal(int portalNum)
    {
        if(currentPortalBullet != null)
        {
            currentPortalBullet.DeleteBullet();
        }

        RaycastHit hit;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 250.0f, LayerMask.GetMask("PortalPlaceable", "Portal")))
        {
            aimPoint = hit.point;
        }

        Debug.Log("hit Object Tag : " + hit.collider.tag);
        Vector3 aimDir = (aimPoint - muzzlePosition.position).normalized;

        currentPortalBullet = Instantiate(portalBullet, muzzlePosition.position, Quaternion.LookRotation(aimDir, Vector3.forward));

        if(hit.collider)
        {
            if(hit.collider.tag == "Concret")
            {
                PortalPlace(hit, portalNum);
            }
            else if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Portal"))
            {
                if (portalPair.portals[portalNum].tag != hit.collider.tag)
                {
                    int hitPortalNum = portalNum == 0 ? 1 : 0;
                    Transform firePortalTransform = portalPair.portals[portalNum].transform;
                    Transform hitPortalTransform = portalPair.portals[hitPortalNum].transform;

                    Vector3 relativePos = hitPortalTransform.InverseTransformPoint(hit.point);
                    if (relativePos.x < 0f)
                    {
                        relativePos = new Vector3(-1.5f, relativePos.y, 0f);
                    }
                    else
                    {
                        relativePos = new Vector3(1.5f, relativePos.y, 0f);
                    }
                    Vector3 resultPos = portalPair.portals[hitPortalNum].transform.TransformPoint(relativePos);

                    PortalPlace(resultPos, hit.transform.rotation, hit.collider, portalNum); 
                }
                else
                {
                    PortalPlace(hit, portalNum, new Vector3(0f, 0f, 0.051f));
                }
            }
        }
    }

    private void PortalPlace(RaycastHit h, int num, Vector3 offsetPos = default(Vector3))
    {
        float yRotation;
        Quaternion wallRotation = h.transform.rotation;
        portalRotation = wallRotation;

        if (wallRotation.eulerAngles.x != 0 && Mathf.Abs(wallRotation.eulerAngles.x) % 90 == 0f)
        {
            //플레이어 로테이션 가져오기
            Quaternion cameraRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            yRotation = cameraRotation.eulerAngles.y;

            portalRotation = Quaternion.Euler(0f, yRotation, 0f) * portalRotation;
        }

        Vector3 portalPos = FixedPosition(h.point + offsetPos, portalRotation);

        portalPair.portals[num].PlacePortal(h.collider, portalPos, portalRotation);
    }

    private void PortalPlace(Vector3 pos, Quaternion rot, Collider collider, int num)
    {
        float yRotation;
        Quaternion wallRotation = rot;
        portalRotation = wallRotation;

        if (wallRotation.eulerAngles.x != 0 && Mathf.Abs(wallRotation.eulerAngles.x) % 90 == 0f)
        {
            //플레이어 로테이션 가져오기
            Quaternion cameraRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            yRotation = cameraRotation.eulerAngles.y;

            portalRotation = Quaternion.Euler(0f, yRotation, 0f) * portalRotation;
        }

        Vector3 portalPos = FixedPosition(pos, portalRotation);

        portalPair.portals[num].PlacePortal(collider, portalPos, portalRotation);
    }

    private Vector3 FixedPosition(Vector3 originPos, Quaternion rotation)
    {
        var portalEdgePoint = new List<Vector3>
        {
            new Vector3(-0.75f,  1.3f, -0.03f),
            new Vector3( 0.75f,  1.3f, -0.03f),
            new Vector3( 0.75f, -1.3f, -0.03f),
            new Vector3(-0.75f, -1.3f, -0.03f)
        };

        var pointsDirections = new List<DirectionData>
        { 
            new DirectionData { dirX =  rotation *  Vector3.right, dirY = rotation * -Vector3.up },
            new DirectionData { dirX =  rotation * -Vector3.right, dirY = rotation * -Vector3.up },
            new DirectionData { dirX =  rotation * -Vector3.right, dirY = rotation *  Vector3.up },
            new DirectionData { dirX =  rotation *  Vector3.right, dirY = rotation *  Vector3.up },
        };

        var nonCollisionPoint = new List<PointData>();

        for (int i = 0; i < 4; ++i)
        {
            Vector3 raycastPos = originPos + rotation * portalEdgePoint[i];
            //positionTestCube[i].transform.position = raycastPos;
            //positionTestCube[i].transform.rotation = rotation;

            Collider[] wallColliders = Physics.OverlapSphere(raycastPos, 0.05f, placementMask);


            wallColliders[i].transform.forward = 
            if(wallColliders.Length == 0)
            {
                Debug.Log(i + "번째 충돌 실패");
                var point = new PointData { point = raycastPos, pointNum = i };
                nonCollisionPoint.Add(point);
            }

            //if (!Physics.CheckSphere(raycastPos, 0.05f, placementMask))
            //{
            //    Debug.Log(i + "번째 충돌 실패");
            //    var point = new PointData { point = raycastPos, pointNum = i };
            //    nonCollisionPoint.Add(point);
            //}
        }

        return PointCorrection(originPos, nonCollisionPoint, pointsDirections);
    }

    private Vector3 PointCorrection(Vector3 pos, List<PointData> points, List<DirectionData> dirs)
    {
        RaycastHit hit;

        if (points.Count == 0)
        {
            return pos;
        }
        else if(points.Count == 1)
        {
            Vector3 offsetX = Vector3.zero;
            Vector3 offsetY = Vector3.zero;

            if (Physics.Raycast(points[0].point, dirs[points[0].pointNum].dirX, out hit, 1.1f, placementMask))
            {
                Debug.Log("offset X detect");
                offsetX = hit.point - points[0].point;
                Debug.Log("offsetX : " + offsetX);
            }

            if (Physics.Raycast(points[0].point, dirs[points[0].pointNum].dirY, out hit, 1.1f, placementMask))
            {
                Debug.Log("offset Y detect");
                offsetY = hit.point - points[0].point;
                Debug.Log("offsetY : " + offsetY);
            }

            Vector3 updatePos = pos + (offsetX.magnitude < offsetY.magnitude ? offsetX : offsetY);
            //for(int i = 0; i < 4; ++i)
            //{
            //    positionTestCube[i].transform.position = positionTestCube[i].transform.position + (offsetX.magnitude < offsetY.magnitude ? offsetX : offsetY);
            //}
           
            return updatePos;
        }
        else if(points.Count == 2 && points[1].pointNum - points[0].pointNum != 2)
        {
            var offset = new List<Vector3>(2);

            for (int i = 0; i < 2; ++i)
            {
                Debug.Log("Loop iteration: " + i);

                if (Physics.Raycast(points[i].point, dirs[points[i].pointNum].dirX, out hit, 1.1f, placementMask))
                {
                    Debug.Log("offset X detect");
                    offset.Add(hit.point - points[i].point);
                }
                else if (Physics.Raycast(points[i].point, dirs[points[i].pointNum].dirY, out hit, 1.1f, placementMask))
                {
                    Debug.Log("offset Y detect");
                    offset.Add(hit.point - points[i].point);
                }
            }

            Vector3 updatePos = pos + (offset[0].magnitude > offset[1].magnitude ? offset[0] : offset[1]);

            //for (int i = 0; i < 4; ++i)
            //{
            //    positionTestCube[i].transform.position = positionTestCube[i].transform.position + (offset[0].magnitude < offset[1].magnitude ? offset[0] : offset[1]);
            //}
            return updatePos;
        }
        else
            return pos;
    }




}
