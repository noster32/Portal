using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CSwitchCube : CComponent
{
    [SerializeField] private float distance;
    [SerializeField] private float duration;

    private bool m_isActive = false;
   
    public void MoveUp()
    {
        if(!m_isActive)
        {
            m_isActive = true;
            StartCoroutine(CubeMoveCoroutine(duration, distance));
        }
    }

    public void MoveDown()
    {
        if (!m_isActive)
        {
            m_isActive = true;
            StartCoroutine(CubeMoveCoroutine(duration, -distance));
        }
    }

    private IEnumerator CubeMoveCoroutine(float duration, float distance)
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + new Vector3(0f, distance, 0f);
        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        yield return null;
    }
}
