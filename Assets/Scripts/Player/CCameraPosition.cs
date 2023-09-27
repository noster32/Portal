using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCameraPosition : CComponent
{
    #region public

    public Transform cameraPos;

    #endregion

    public override void Update()
    {
        transform.position = cameraPos.position;
    }
}
