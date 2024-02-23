using System.Collections;
using UnityEngine;

public class CDoor1 : CComponent
{
    [SerializeField] private float doorGap = 1.19f;
    [SerializeField] private float openDuration = 0.5f;
    private Coroutine moveCoroutine;

    private Transform doorR;
    private Transform doorL;

    private AudioSource audioSource;
    [SerializeField] private AudioClip doorSound;

    public override void Awake()
    {
        base.Awake();

        doorR = transform.GetChild(0);
        doorL = transform.GetChild(1);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = doorSound;
    }

    public void DoorOpen()
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        moveCoroutine = StartCoroutine(moveDoor(doorGap, openDuration));
        audioSource.PlayOneShot(doorSound);
    }

    public void DoorClose()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        moveCoroutine = StartCoroutine(moveDoor(0f, openDuration));
        audioSource.PlayOneShot(doorSound);
    }

    IEnumerator moveDoor(float gap, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            doorR.localPosition = Vector3.Lerp(doorR.localPosition, new Vector3(gap, 0f, 0f), elapsedTime / duration);
            doorL.localPosition = Vector3.Lerp(doorL.localPosition, new Vector3(-gap, 0f, 0f), elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        doorR.localPosition = new Vector3(gap, 0f, 0f);
        doorL.localPosition = new Vector3(-gap, 0f, 0f);

        moveCoroutine = null;
        yield return null;
    }
}
