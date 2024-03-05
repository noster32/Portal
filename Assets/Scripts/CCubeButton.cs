using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CCubeButton : CComponent
{
    [Header("Sound")]
    [SerializeField] private AudioClip enterSound;
    [SerializeField] private AudioClip exitSound;

    [Header("Event")]
    [SerializeField] private UnityEvent triggerEnterEvent;
    [SerializeField] private UnityEvent triggerExitEvent;

    private Coroutine moveCoroutine;
    private Transform buttonTop;
    private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();

        buttonTop = transform.GetChild(0);
        audioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        Vector3 movePosition = new Vector3(0f, -0.25f, 0f);
        moveCoroutine = StartCoroutine(moveButton(0.3f, buttonTop.localPosition, movePosition));
        audioSource.PlayOneShot(enterSound, CSoundLoader.Instance.GetEffectVolume(0.4f));

        triggerEnterEvent.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        //Red light slowly off
    }

    private void OnTriggerExit(Collider other)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        Vector3 movePosition = Vector3.zero;
        moveCoroutine = StartCoroutine(moveButton(0.3f, buttonTop.localPosition, movePosition));
        audioSource.PlayOneShot(exitSound, CSoundLoader.Instance.GetEffectVolume(0.4f));

        triggerExitEvent.Invoke();
    }

    IEnumerator moveButton(float duration, Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            buttonTop.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        buttonTop.localPosition = endPos;

        moveCoroutine = null;
        yield return null;
    }
}
