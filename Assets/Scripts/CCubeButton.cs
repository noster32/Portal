using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CCubeButton : CComponent
{
    private Transform buttonTop;


    public override void Awake()
    {
        base.Awake();

        buttonTop = transform.GetChild(0);
    }


    private void OnTriggerEnter(Collider other)
    {
        string objectTag = other.gameObject.tag;
        Vector3 movePosition = buttonTop.localPosition - Vector3.down * -0.8f;
        if (objectTag == "Cube")
        {
            StartCoroutine(moveButton(buttonTop.localPosition, movePosition, 0.3f));
            
        }

        //Bool true 
    }

    private void OnTriggerStay(Collider other)
    {
        //Red light On
    }

    private void OnTriggerExit(Collider other)
    {
        string objectTag = other.gameObject.tag;
        Vector3 movePosition = buttonTop.localPosition - Vector3.down * 0.8f;
        if (objectTag == "Cube")
        {
            StartCoroutine(moveButton(buttonTop.localPosition, movePosition, 0.3f));

        }

        //Bool false
    }

    IEnumerator moveButton(Vector3 startPos, Vector3 targetPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            buttonTop.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null;
        }

        yield return null;
    }
}
