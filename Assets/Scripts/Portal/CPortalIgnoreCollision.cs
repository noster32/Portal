using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalIgnoreCollision : CComponent
{
    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private LayerMask teleportObjectMask;                                  //텔레포트 가능한 오브젝트 레이어

    private BoxCollider boxCollider;

    private List<Collider> placedWall = new List<Collider>();                               //포탈이 설치되어 있는 벽 콜라이더
    private List<CTeleportObject> insidePortalAreaObject = new List<CTeleportObject>();     //포탈 에리어 안에 있는 오브젝트

    public override void Awake()
    {
        base.Awake();

        boxCollider = GetComponent<BoxCollider>();
        if(portalPair == null)
            portalPair = transform.parent.parent.GetComponent<CPortalPair>();
    }

    //콜라이더 안에 들어올 경우 리스트에 추가, 설치되어있는 벽의 콜라이더와 플레이어의 콜리전 false
    private void OnTriggerEnter(Collider other)
    {
        if(!portalPair.PlacedBothPortal())
        {
            return;
        }

        CTeleportObject tpObject;
        if (other.tag == "Turret")
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();
        
        if (tpObject != null)
        {
            if (insidePortalAreaObject.Contains(tpObject))
                return;

            insidePortalAreaObject.Add(tpObject);
            tpObject.EnterPortalIgnoreCollision(placedWall);
        }
    }

    //콜라이더에서 벗어날 경우 리스트에서 제거하고 콜리전 true
    private void OnTriggerExit(Collider other)
    {
        if (!portalPair.PlacedBothPortal())
        {
            return;
        }

        CTeleportObject tpObject;

        if (other.tag == "Turret")
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();

        if (tpObject != null)
        {
            if(insidePortalAreaObject.Contains(tpObject))
                insidePortalAreaObject.Remove(tpObject);

            tpObject.ExitPortalIgnoreCollision(placedWall);
        }
    }

    //
    public void PlaceInsidePortalAreaCheck(Transform targetTransform)
    {
        Collider[] collider = Physics.OverlapBox(targetTransform.TransformPoint(boxCollider.center), boxCollider.size / 2, targetTransform.rotation, teleportObjectMask);

        if (collider.Length > 0)
        {
            foreach(Collider col in collider)
            {
                if(col.tag == "Turret")
                {
                    CTeleportObject obj = col.transform.GetComponentInParent<CTeleportObject>();

                    if(obj != null)
                    {
                        insidePortalAreaObject.Add(obj);
                    }
                }
                else
                {
                    if (col.transform.TryGetComponent(out CTeleportObject obj))
                    {
                        Debug.Log("test");
                        insidePortalAreaObject.Add(obj);
                    }
                }

            }
        }
    }


    public void InsidePortalAreaObjectIgnoreCollision()
    {
        if(portalPair.PlacedBothPortal())
        {
            foreach(CTeleportObject obj in insidePortalAreaObject)
            {
                obj.EnterPortalIgnoreCollision(placedWall);
            }
        }
    }

    public void ClearPlacedWallList()
    {
        if(insidePortalAreaObject.Count > 0)
        {
            foreach (var tpObject in insidePortalAreaObject)
            {
                tpObject.ExitPortalIgnoreCollision(placedWall);
            }
        }

        placedWall.Clear();
    }

    public void StartGetPlacedWallCollider(Transform target)
    {
        StartCoroutine(GetPlacedWallColliderCoroutine(target));
    }

    public List<Collider> GetWallList()
    {
        return placedWall;
    }

    private IEnumerator GetPlacedWallColliderCoroutine(Transform targetTransform)
    {
        Collider[] test = Physics.OverlapBox(targetTransform.position, new Vector3(0.75f, 1.3f, 0.05f), targetTransform.rotation, LayerMask.GetMask("PortalPlaceable"));

        List<Collider> placedWallList = new List<Collider>();

        if (test.Length > 0)
        {
            for (int i = 0; i < test.Length; i++)
            {
                if (test[i].transform.forward == targetTransform.forward)
                {
                    placedWallList.Add(test[i]);
                }
            }
        }

        placedWall = placedWallList;

        yield return null;
    }
}

