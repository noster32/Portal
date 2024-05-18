using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CCubeButton : CComponent
{
    [Header("Event")]
    [SerializeField] private UnityEvent triggerEnterEvent;
    [SerializeField] private UnityEvent triggerExitEvent;

    private Coroutine moveCoroutine;
    private Transform buttonTop;
    private bool isActive = false;

    public override void Awake()
    {
        base.Awake();

        buttonTop = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive || !other.CompareTag("Cube") || !other.CompareTag("Player"))
            return;

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        Vector3 movePosition = new Vector3(0f, -0.25f, 0f);
        moveCoroutine = StartCoroutine(moveButton(0.3f, buttonTop.localPosition, movePosition));
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.button3, this.transform.position);

        triggerEnterEvent?.Invoke();
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

        isActive = false;
        Vector3 movePosition = Vector3.zero;
        moveCoroutine = StartCoroutine(moveButton(0.3f, buttonTop.localPosition, movePosition));
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.button10, this.transform.position);

        triggerExitEvent?.Invoke();
    }

    IEnumerator moveButton(float duration, Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            buttonTop.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        buttonTop.localPosition = endPos;

        moveCoroutine = null;
        yield return null;
    }
}
