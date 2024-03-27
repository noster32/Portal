using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMovingPlatformWall : CComponent
{
    [SerializeField] float moveDistance;
    [SerializeField] float rotationAngle;
    [SerializeField] float moveTime;
    [SerializeField] float rotationTime;

    [SerializeField] private Transform moveCylinder;
    [SerializeField] private Transform rotationCylinder;
    [SerializeField] private GameObject portalPlacementWall;


    private bool isMoving;
    private bool isMoved;
    private bool notMoved;

    public override void Awake()
    {
        base.Awake();

        isMoving = false;
        isMoved = false;
        notMoved = true; 
    }

    public override void Start()
    {
        base.Start();

        portalPlacementWall.layer = LayerMask.NameToLayer("NonPortalPlaceable");
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.U))
        {
            StartMove();
        }
    }

    public void StartMove()
    {
        if(notMoved && !isMoved)
        {
            isMoving = true;
            portalPlacementWall.layer = LayerMask.NameToLayer("NonPortalPlaceable");
            StartCoroutine(moveCoroutine(moveTime, rotationTime));
        }
    }

    public void ReturnMove()
    {
        if(isMoved && !isMoving)
        {
            isMoving = true;
            portalPlacementWall.layer = LayerMask.NameToLayer("NonPortalPlaceable");
            StartCoroutine(moveReturnCoroutine(moveTime, rotationTime));
        }
    }

    private IEnumerator moveCoroutine(float moveDuration, float rotationDuration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = moveCylinder.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, moveDistance, 0f);

        Debug.Log(this.transform.position);
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.apcStart, this.transform.position);

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            moveCylinder.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        moveCylinder.localPosition = endPos;
        StartCoroutine(roationCoroutine(rotationDuration));
        yield return null;
    }

    private IEnumerator roationCoroutine(float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRot = rotationCylinder.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, rotationAngle);

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.apcShutdown, this.transform.position);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rotationCylinder.localRotation = Quaternion.Slerp(startRot, endRot, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rotationCylinder.localRotation = endRot;
        portalPlacementWall.layer = LayerMask.NameToLayer("PortalPlaceable");
        isMoving = false;
        notMoved = false;
        isMoved = true;
        yield return null;
    }

    private IEnumerator moveReturnCoroutine(float moveDuration, float rotationDuration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = moveCylinder.localPosition;
        Vector3 endPos = new Vector3(0f, 0f, 0f);

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            moveCylinder.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        moveCylinder.localPosition = endPos;
        StartCoroutine(roationReturnCoroutine(rotationDuration));
        yield return null;
    }

    private IEnumerator roationReturnCoroutine(float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRot = rotationCylinder.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, 0f, 0f);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rotationCylinder.localRotation = Quaternion.Slerp(startRot, endRot, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rotationCylinder.localRotation = endRot;
        portalPlacementWall.layer = LayerMask.NameToLayer("PortalPlaceable");

        isMoving = false;
        isMoved = false;
        notMoved = true;
        yield return null;
    }
}
