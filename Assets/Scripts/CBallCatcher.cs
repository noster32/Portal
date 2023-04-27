using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CBallCatcher : CComponent
{
    #region public
    public GameObject connectLauncher;
    #endregion
    private new Animation animation;

    public override void Awake()
    {
        base.Awake();

        animation = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string objectTag = other.gameObject.tag;
        if(objectTag == "Ball")
        {
            Destroy(other.gameObject);
            connectLauncher.GetComponent<CBallLauncher>().ballConnect = true;
            animation.Play("close");
            Debug.Log("Destroy!");
        }
    }
}
