using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CResumeButton : CComponent, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        CGameManager.Instance.Resume();
    }

}
