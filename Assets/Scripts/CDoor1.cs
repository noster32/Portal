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
    [SerializeField] private Collider wallForward;
    [SerializeField] private Collider wallBackward;
    private AudioSource audioSource;
    [SerializeField] private AudioClip doorSound;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = doorSound;
    }

    public override void Start()
    {
        base.Start();

        if(startDoorOpen)
        {
            doorR.localPosition = new Vector3(doorGap, 0f, 0f);
            doorL.localPosition = new Vector3(-doorGap, 0f, 0f);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreCollision(other, wallForward, true);
        Physics.IgnoreCollision(other, wallBackward, true);
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreCollision(other, wallForward, false);
        Physics.IgnoreCollision(other, wallBackward, false);
    }
}
