using System.Collections;
using UnityEngine;

public class CElevator : CComponent
{
    [Header("Move Setting")]
    [SerializeField] private Transform upperFloorPosition;
    [SerializeField] private float floorGap = 0.42f;

    [Header("Collider")]
    [SerializeField] private GameObject elevatorDoorCollider;

    [Header("Camera")]
    [SerializeField] private CPlayerCameraShake cameraEffect;

    private Coroutine doorCoroutine;
    private Animator elevatorAnimator;
    private Vector3 floorGapVector;

    public override void Awake()
    {
        base.Awake();

        elevatorAnimator = GetComponent<Animator>();
    }

    public override void Start()
    {
        base.Start();

        if (cameraEffect == null)
            Camera.main.transform.GetComponent<CPlayerCameraShake>();

        floorGapVector = new Vector3(0f, floorGap, 0f);
        transform.localPosition = floorGapVector;

        elevatorAnimator.Play("closing", 0, 1f);
    }

    public void ActiveElevator()
    {
        PlayDoorClose();

        StartCoroutine(MoveElevator(5f, 9f, transform.localPosition, upperFloorPosition.localPosition + floorGapVector));
    }

    public void PlayDoorClose()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        doorCoroutine = StartCoroutine(DoorCloseAnim());
    }

    public void PlayDoorOpen()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        doorCoroutine = StartCoroutine(DoorOpenAnim());
    }

    public void PlayArriveDoorOpen()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        doorCoroutine = StartCoroutine(ArriveDoorOpenAnim());
    }

    private IEnumerator ArriveDoorOpenAnim()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.garageStop1, this.transform.position);

        yield return new WaitForSeconds(1f);

        elevatorDoorCollider.SetActive(false);
        elevatorAnimator.Play("opening");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.elevatorDoor, this.transform.position);
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.doorLatch1, this.transform.position);
        yield return new WaitForSeconds(elevatorAnimator.GetCurrentAnimatorClipInfo(0).Length);

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalElevatorChime, this.transform.position);
        doorCoroutine = null;
        yield return null;
    }

    private IEnumerator DoorOpenAnim()
    {
        elevatorDoorCollider.SetActive(false);
        elevatorAnimator.Play("opening");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.elevatorDoor, this.transform.position);
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.doorLatch1, this.transform.position);
        yield return new WaitForSeconds(elevatorAnimator.GetCurrentAnimatorClipInfo(0).Length);

        doorCoroutine = null;
        yield return null;
    }

    private IEnumerator DoorCloseAnim()
    {
        elevatorDoorCollider.SetActive(true);
        elevatorAnimator.Play("closing");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.elevatorDoor, this.transform.position);
        yield return new WaitForSeconds(elevatorAnimator.GetCurrentAnimatorClipInfo(0).Length);

        doorCoroutine = null;
        yield return null;
    }

    private IEnumerator MoveElevator(float startDelay, float duration, Vector3 start, Vector3 end)
    {
        yield return new WaitForSeconds(startDelay);

        cameraEffect.PlayCameraShake(2f, 0.2f);
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.elevatorMove, this.transform.position);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localPosition = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = end;
        PlayArriveDoorOpen();
        yield return null;
    }
}
