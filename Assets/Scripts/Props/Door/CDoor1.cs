using System.Collections;
using UnityEngine;

public class CDoor1 : CComponent
{
    [SerializeField] private bool startDoorOpen = false;
    [SerializeField] private float doorGap = 1.19f;
    [SerializeField] private float openDuration = 0.5f;
    private Coroutine moveCoroutine;

    [SerializeField] private Transform doorR;
    [SerializeField] private Transform doorL;

    [SerializeField] private bool isDoorOpen = false;
    [SerializeField] private bool active1 = false;
    [SerializeField] private bool active2 = false;

    public override void Start()
    {
        base.Start();

        if(startDoorOpen)
        {
            doorR.localPosition = new Vector3(doorGap, 0f, 0f);
            doorL.localPosition = new Vector3(-doorGap, 0f, 0f);
            isDoorOpen = true;
        }
    }

    public void DoorOpen()
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        isDoorOpen = true;
        moveCoroutine = StartCoroutine(moveDoor(doorGap, openDuration));
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door1, this.transform.position);
    }

    public void DoorOpenDoubleButton()
    {
        if (!active1 || !active2)
            return;

        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        isDoorOpen = true;
        moveCoroutine = StartCoroutine(moveDoor(doorGap, openDuration));
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door1, this.transform.position);
    }

    public void DoorClose()
    {
        if (!isDoorOpen)
            return;

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        isDoorOpen = false;
        moveCoroutine = StartCoroutine(moveDoor(0f, openDuration));
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door1, this.transform.position);
    }

    public void DoorActive1() => active1 = true;

    public void DoorActive2() => active2 = true;    

    public void DoorDisable1() => active1 = false;

    public void DoorDisable2() => active2 = false;


    IEnumerator moveDoor(float gap, float duration)
    {
        float elapsedTime = 0f;
        Vector3 doorRStartPos = doorR.localPosition;
        Vector3 doorLStartPos = doorL.localPosition;
        while (elapsedTime < duration)
        {
            doorR.localPosition = Vector3.Lerp(doorRStartPos, new Vector3(gap, 0f, 0f), elapsedTime / duration);
            doorL.localPosition = Vector3.Lerp(doorLStartPos, new Vector3(-gap, 0f, 0f), elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        doorR.localPosition = new Vector3(gap, 0f, 0f);
        doorL.localPosition = new Vector3(-gap, 0f, 0f);

        moveCoroutine = null;
        yield return null;
    }
}
