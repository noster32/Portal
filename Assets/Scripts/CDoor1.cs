using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CDoor1 : CComponent
{
    public bool interaction;

    private Transform doorR;
    private Transform doorL;

    public override void Awake()
    {
        base.Awake();

        interaction = false;

        doorR = transform.GetChild(0);
        doorL = transform.GetChild(1);
    }

    public override void Update()
    {
        base.Update();

        if(interaction)
        {
            DoorOpen();
        }
        else
        {
            DoorClose();
        }

    }

    void DoorOpen()
    {
        doorR.localPosition = Vector3.Lerp(doorR.localPosition, new Vector3(4.5f, 0f, 0f), Time.deltaTime * 4.0f);
        doorL.localPosition = Vector3.Lerp(doorL.localPosition, new Vector3(-4.5f, 0f, 0f), Time.deltaTime * 4.0f);
    }

    void DoorClose()
    {
        doorR.localPosition = Vector3.Lerp(doorR.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime * 3.0f);
        doorL.localPosition = Vector3.Lerp(doorL.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime * 3.0f);
    }

    IEnumerator moveDoor(Vector3 startPos, Vector3 targetPos, float duration)
    {
        float elapsedTimeR = 0f;
        float elapsedTimeL = 0f;
        while (elapsedTimeR < duration)
        {
            elapsedTimeR += Time.deltaTime;
            elapsedTimeL += Time.deltaTime;
            doorR.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTimeR / duration);
            doorL.localPosition = Vector3.Lerp(startPos, new Vector3(-targetPos.x, targetPos.y, targetPos.z), elapsedTimeL / duration);
            yield return null;
        }

        yield return null;
    }
}
