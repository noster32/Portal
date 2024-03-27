using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCameraEffect : CComponent
{
    private Coroutine m_cameraShakeCoroutine;

    public void PlayCameraShake(float duration, float magnitude)
    {
        if (m_cameraShakeCoroutine == null)
            m_cameraShakeCoroutine = StartCoroutine(CameraShake(duration, magnitude));
    }

    public void PlayCameraShakeUpdown(float duration, float magnitude)
    {
        if (m_cameraShakeCoroutine == null)
            m_cameraShakeCoroutine = StartCoroutine(CameraShakeUpDown(duration, magnitude));
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsedTime = 0f;
        float originalMagnitude = magnitude;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            magnitude = Mathf.Lerp(originalMagnitude, 0f, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
        m_cameraShakeCoroutine = null;
    }

    private IEnumerator CameraShakeUpDown(float duration, float magnitude)
    {
        Quaternion originalRotation = transform.localRotation;

        float elapsedTime = 0f;
        float originalMagnitude = magnitude;

        while (elapsedTime < duration)
        {
            float y = Random.Range(0, 1f) * magnitude;

            transform.localRotation = Quaternion.Euler(originalRotation.eulerAngles.x, 
                                                        originalRotation.eulerAngles.y + y, 
                                                        originalRotation.eulerAngles.z);

            magnitude = Mathf.Lerp(originalMagnitude, 0f, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localRotation = originalRotation;
        m_cameraShakeCoroutine = null;
    }
}
