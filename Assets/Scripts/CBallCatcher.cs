using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CBallCatcher : CComponent
{
    public GameObject connectLauncher;

    Animation animation;

    public override void Awake()
    {
        base.Awake();

        connectLauncher = GetComponent<CBallLauncher>().gameObject;
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
