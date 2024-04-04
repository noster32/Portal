using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDoor2 : CComponent
{
    public int waitTime;
    [SerializeField] private float doorGap = 3f;
    [SerializeField] private float openDuration;
    [SerializeField] private float closeDuraition;

    [SerializeField] private Transform  door;

    private Coroutine moveCoroutine;

    public override void Start()
    {
        base.Start();

        door.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void StartMoveDoor()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(moveDoor(doorGap, openDuration, closeDuraition, waitTime));
    }

    public void StartOpenDoor()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(OpenDoor(doorGap, openDuration));
    }

    public void StartCloseDoor()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(CloseDoor(doorGap, closeDuraition));
    }

    IEnumerator moveDoor(float gap, float openDuration, float closeDuration, int waitTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < openDuration)
        {
            float t = elapsedTime / openDuration;
            door.localPosition = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(0f, gap, 0f), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        door.localPosition = new Vector3(0f, gap, 0f);

        float waitSec = waitTime - openDuration + 0.7f;
        yield return new WaitForSeconds(waitSec);

        elapsedTime = 0f;

        while (elapsedTime < closeDuration)
        {
            float t = elapsedTime / closeDuration;
            door.localPosition = Vector3.Lerp(new Vector3(0f, gap, 0f), new Vector3(0f, 0f, 0f), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.localPosition = new Vector3(0f, 0f, 0f);

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door1, this.transform.position);
        moveCoroutine = null;
        yield return null;
    }

    private IEnumerator OpenDoor(float gap, float openDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < openDuration)
        {
            float t = elapsedTime / openDuration;
            door.localPosition = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(0f, gap, 0f), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        door.localPosition = new Vector3(0f, gap, 0f);
        moveCoroutine = null;
        yield return null;
    }

    private IEnumerator CloseDoor(float gap, float closeDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < closeDuration)
        {
            float t = elapsedTime / closeDuration;
            door.localPosition = Vector3.Lerp(new Vector3(0f, gap, 0f), new Vector3(0f, 0f, 0f), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.localPosition = new Vector3(0f, 0f, 0f);

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door1, this.transform.position);
        moveCoroutine = null;
        yield return null;
    }
}
