using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CClaw : CComponent
{
    [Header("Setting")]
    [SerializeField] private float bodyMoveDistance;
    [SerializeField] private float pincerRotateValue;
    [SerializeField] private float speed = 4f;
    [SerializeField] private bool isAlreadyGrabbed;
    [SerializeField] private bool isDisable;
    [SerializeField] EventReference retractSound;

    [Header("Object")]
    [SerializeField] private GameObject moveObject;

    private bool isActive;
    private Transform clawPincer1;
    private Transform clawPincer2;
    private Transform grabedObjectParent;

    public override void Awake()
    {
        base.Awake();

        clawPincer1 = transform.GetChild(1).GetChild(0);
        clawPincer2 = transform.GetChild(1).GetChild(1);
        grabedObjectParent = transform.GetChild(2);

        if (isAlreadyGrabbed)
        {
            clawPincer1.localRotation = Quaternion.Euler(pincerRotateValue, 0f, 0f);
            clawPincer2.localRotation = Quaternion.Euler(-pincerRotateValue, 0f, 0f);
        }
        else
        {
            clawPincer1.localRotation = Quaternion.identity;
            clawPincer2.localRotation = Quaternion.identity;
        }

        if (isDisable)
            this.gameObject.SetActive(false);
    }


    public void ActiveClaw()
    {
        if (!isActive)
        {
            isActive = true;
            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);

            StartCoroutine(ClawCoroutine());
        }
    }

    private IEnumerator ClawCoroutine()
    {
        Vector3 originalPosition = transform.position;
        Vector3 targerPositionDown = transform.position + new Vector3(0f, -bodyMoveDistance, 0f);

        while (true)
        {
            if (Vector3.Distance(transform.position, targerPositionDown) < 0.02f)
            {
                transform.position = targerPositionDown;
                break;
            }

            transform.position = Vector3.MoveTowards(transform.position, targerPositionDown, speed * Time.deltaTime);

            yield return null;
        }

        float elapsedTime = 0f;

        CAudioManager.Instance.PlayOneShot(retractSound, this.transform.position);

        if(isAlreadyGrabbed)
        {
            while (elapsedTime < 1f)
            {
                float t = elapsedTime / 1f;
                clawPincer1.localRotation = Quaternion.Slerp(Quaternion.Euler(pincerRotateValue, 0f, 0f), Quaternion.identity, t);
                clawPincer2.localRotation = Quaternion.Slerp(Quaternion.Euler(-pincerRotateValue, 0f, 0f), Quaternion.identity, t);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            moveObject.transform.SetParent(null);
        }
        else
        {
            while (elapsedTime < 2f)
            {
                float t = elapsedTime / 2f;
                clawPincer1.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(pincerRotateValue, 0f, 0f), t);
                clawPincer2.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(-pincerRotateValue, 0f, 0f), t);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            moveObject.transform.SetParent(grabedObjectParent);
        }

        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (Vector3.Distance(transform.position, originalPosition) < 0.02f)
            {
                transform.position = originalPosition;
                break;
            }

            transform.position = Vector3.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);

            yield return null;
        }

        if (!isAlreadyGrabbed)
        {
            moveObject.SetActive(false);
            isAlreadyGrabbed = true;
        }
        else
            isAlreadyGrabbed = false;

        this.gameObject.SetActive(false);

        isActive = false;

        yield return null;
    }
}
