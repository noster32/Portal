using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CBallLauncher : CComponent
{
    #region public
    public bool ballConnect;
    #endregion

    [SerializeField] private GameObject BallObj;

    Animation aLauncher;
    AnimationEvent launcherOpenEvent;
    AnimationEvent launcherCloseEvent;

    MeshCollider launcherCollider;

    public override void Awake()
    {
        base.Awake();
        ballConnect = false;

        aLauncher = GetComponent<Animation>();

        launcherOpenEvent = new AnimationEvent();
        launcherOpenEvent.functionName = "BallLaunch";
        launcherOpenEvent.time = aLauncher.GetClip("open").length;
        aLauncher.GetClip("open").AddEvent(launcherOpenEvent);

        launcherCloseEvent = new AnimationEvent();
        launcherCloseEvent.functionName = "BallClose";
        launcherCloseEvent.time = aLauncher.GetClip("close").length;
        aLauncher.GetClip("close").AddEvent(launcherCloseEvent);

        launcherCollider = GetComponent<MeshCollider>();
    }
    public override void Update()
    {
        base.Update();

        if (!ballConnect)
        {
            GameObject obj = GameObject.Find("Ball(Clone)");

            if (obj == null)
            {
                launcherCollider.enabled = false;
                aLauncher.Play("open");
            }
        }

    }

    void BallLaunch()
    {
        GameObject Ball = Instantiate(BallObj, transform.position + transform.forward * 0.7f, transform.rotation);
        
        float BallSpeed = 500.0f;
        
        Vector3 BallLaunch = Ball.transform.TransformDirection(Vector3.forward);
        
        Ball.GetComponent<Rigidbody>().AddForce(BallSpeed * BallLaunch);

        aLauncher.Play("close");
        
        Destroy(Ball, 15f);
    }

    void BallClose()
    {
        launcherCollider.enabled = true;

        aLauncher.Play("idle_closed");
    }
}
