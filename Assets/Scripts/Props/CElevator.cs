using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CElevator : CComponent
{
    [Header("Move Setting")]
    [SerializeField] private Transform upperFloorPosition;
    [SerializeField] private float floorGap = 0.42f;

    [Header("Collider")]
    [SerializeField] private GameObject elevatorDoorCollider;

    [Header("Sound")]
    [SerializeField] private AudioClip doorOpenCloseClip;
    [SerializeField] private AudioClip doorOpenSound2;
    [SerializeField] private AudioClip elevatorMoveSound;
    [SerializeField] private AudioClip elevatorStopSound;
    [SerializeField] private AudioClip elevatorChime;

    [Header("Camera")]
    [SerializeField] private CPlayerCameraEffect cameraEffect;

    private AudioSource audioSource;
    private Coroutine doorCoroutine;
    private Animator elevatorAnimator;
    private Vector3 floorGapVector;

    private bool isDoorOpen;

    public override void Awake()
    {
        base.Awake();

        elevatorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();

        floorGapVector = new Vector3(0f, floorGap, 0f);
        transform.localPosition = floorGapVector;

        elevatorAnimator.Play("closing", 0, 1f);

        if (elevatorMoveSound)
            audioSource.clip = elevatorMoveSound;
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

    private IEnumerator DoorOpenAnim()
    {
        elevatorDoorCollider.SetActive(false);
        elevatorAnimator.Play("opening");
        audioSource.PlayOneShot(doorOpenCloseClip, CSoundLoader.Instance.GetEffectVolume(0.3f));
        audioSource.PlayOneShot(doorOpenSound2, CSoundLoader.Instance.GetEffectVolume(0.3f));
        yield return new WaitForSeconds(elevatorAnimator.GetCurrentAnimatorClipInfo(0).Length);

        doorCoroutine = null;
        yield return null;
    }

    private IEnumerator DoorCloseAnim()
    {
        elevatorDoorCollider.SetActive(true);
        elevatorAnimator.Play("closing");
        audioSource.PlayOneShot(doorOpenCloseClip, CSoundLoader.Instance.GetEffectVolume(0.3f));
        yield return new WaitForSeconds(elevatorAnimator.GetCurrentAnimatorClipInfo(0).Length);

        doorCoroutine = null;
        yield return null;
    }

    private IEnumerator MoveElevator(float startDelay, float duration, Vector3 start, Vector3 end)
    {
        yield return new WaitForSeconds(startDelay);

        cameraEffect.PlayCameraShake(2f, 0.2f);
        audioSource.PlayOneShot(elevatorMoveSound, CSoundLoader.Instance.GetEffectVolume(0.3f));

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localPosition = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = end;
        audioSource.PlayOneShot(elevatorStopSound, CSoundLoader.Instance.GetEffectVolume(0.3f));

        yield return new WaitForSeconds(1f);

        PlayDoorOpen();
        audioSource.PlayOneShot(elevatorChime, CSoundLoader.Instance.GetEffectVolume(0.3f));
        

        yield return null;
    }
    
}
