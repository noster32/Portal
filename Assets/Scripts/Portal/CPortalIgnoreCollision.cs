using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalIgnoreCollision : CComponent
{
    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private LayerMask teleportObjectMask;                                  //텔레포트 가능한 오브젝트 레이어

    private BoxCollider boxCollider;

    private List<Collider> placedWall = new List<Collider>();                               //포탈이 설치되어 있는 벽 콜라이더
    private List<CTeleportObject> collideIgnoreObjects = new List<CTeleportObject>();      //충돌 무시 구역 안에 있는 오브젝트

    public override void Awake()
    {
        base.Awake();

        boxCollider = GetComponent<BoxCollider>();
        if(portalPair == null)
            portalPair = transform.parent.parent.GetComponent<CPortalPair>();
    }

    //구역 안에 들어올 경우 리스트에 추가, 설치되어있는 벽의 콜라이더와 플레이어의 콜리전 false
    private void OnTriggerEnter(Collider other)
    {
        if(!portalPair.PlacedBothPortal())
            return;

        CTeleportObject tpObject;
        if (other.CompareTag("Turret"))
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();
        
        if (tpObject != null)
        {
            if (collideIgnoreObjects.Contains(tpObject))
                return;

            collideIgnoreObjects.Add(tpObject);
            tpObject.EnterPortalIgnoreCollision(placedWall);
        }
    }

    //구역에서 벗어날 경우 리스트에서 제거하고 콜리전 true
    private void OnTriggerExit(Collider other)
    {
        if (!portalPair.PlacedBothPortal())
            return;

        CTeleportObject tpObject;

        if (other.CompareTag("Turret"))
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject>();

        if (tpObject != null)
        {
            RemoveCollideIgnoreObject(tpObject);
        }
    }

    //충돌 무시 구역 안에 있는 오브젝트를 체크해서 리스트에 추가
    public void PlaceInsidePortalAreaCheck(Transform targetTransform)
    {
        Collider[] collider = Physics.OverlapBox(targetTransform.TransformPoint(boxCollider.center), boxCollider.size / 2, targetTransform.rotation, teleportObjectMask);

        if (collider.Length > 0)
        {
            for(int i = 0; i < collider.Length; ++i)
            {
                if (collider[i].CompareTag("Turret"))
                {
                    CTeleportObject obj = collider[i].transform.GetComponentInParent<CTeleportObject>();

                    if (obj != null)
                    {
                        collideIgnoreObjects.Add(obj);
                    }
                }
                else
                {
                    if (collider[i].transform.TryGetComponent(out CTeleportObject obj))
                    {
                        collideIgnoreObjects.Add(obj);
                    }
                }
            }
        }
    }

    //리스트에 있는 오브젝트들을 벽과에 충돌을 무시
    public void ObjectsIgnoreCollision()
    {
        if(portalPair.PlacedBothPortal())
        {
            for(int i = 0; i < collideIgnoreObjects.Count; ++i)
            {
                collideIgnoreObjects[i].EnterPortalIgnoreCollision(placedWall);
            }
        }
    }

    //리스트의 오브젝트를 제거
    public void RemoveCollideIgnoreObject(CTeleportObject obj)
    {
        if (!collideIgnoreObjects.Contains(obj))
            return;

        collideIgnoreObjects.Remove(obj);
        obj.ExitPortalIgnoreCollision(placedWall);
    }

    //설치된 벽의 리스트 초기화
    public void ClearPlacedWallList()
    {
        if(collideIgnoreObjects.Count > 0)
        {
            for(int i = 0; i < collideIgnoreObjects.Count; ++i)
            {
                collideIgnoreObjects[i].ExitPortalIgnoreCollision(placedWall);
            }
        }

        placedWall.Clear();
    }

    //설치된 벽의 콜라이더를 리스트에 추가
    public void StartGetPlacedWallCollider(Transform target)
    {
        Collider[] test = Physics.OverlapBox(target.position, new Vector3(0.75f, 1.3f, 0.05f), target.rotation, LayerMask.GetMask("PortalPlaceable"));

        List<Collider> placedWallList = new List<Collider>();

        if (test.Length > 0)
        {
            for (int i = 0; i < test.Length; i++)
            {
                if (test[i].transform.forward == target.forward)
                {
                    placedWallList.Add(test[i]);
                }
            }
        }

        placedWall = placedWallList;
    }

    public List<Collider> GetWallList() => placedWall;

}

