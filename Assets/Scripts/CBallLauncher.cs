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
    [SerializeField] private CBall comBall;
    [SerializeField] CBallCatcher comBallCatcher;

    Animation aLauncher;
    AnimationEvent launcherOpenEvent;
    AnimationEvent launcherCloseEvent;

    MeshCollider launcherCollider;

    private GameObject replicateBall;
    private Rigidbody replicateBallRigidbody;

    public override void Awake()
    {
        base.Awake();
        ballConnect = false;

        aLauncher = GetComponent<Animation>();

        launcherOpenEvent = new AnimationEvent();
        launcherOpenEvent.functionName = "BallLaunch";
        launcherOpenEvent.time = aLauncher.GetClip("open").length;
        aLauncher.GetClip("open").AddEvent(launcherOpenEvent);

        //launcherCloseEvent = new AnimationEvent();
        //launcherCloseEvent.functionName = "BallClose";
        //launcherCloseEvent.time = aLauncher.GetClip("close").length;
        //aLauncher.GetClip("close").AddEvent(launcherCloseEvent);

        launcherCollider = GetComponent<MeshCollider>();
    }
    public override void Update()
    {
        base.Update();

        if (!comBallCatcher.GetcomBallConnected() && !comBall.isActiveAndEnabled)
        {
            aLauncher.Play("open");
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            aLauncher.Play("open");
        }
    }

    private void BallLaunch()
    {
        comBall.transform.position = transform.position + transform.forward * 0.7f;
        comBall.SetForward(transform.forward);
        comBall.gameObject.SetActive(true);
        comBall.SetIsSpawnedTrue();
        comBall.StartSound();

        float BallSpeed = 500.0f;

        Vector3 BallLaunch = comBall.transform.TransformDirection(Vector3.forward);

        comBall.ballRigidbody.AddForce(BallSpeed * BallLaunch);
        
        Physics.IgnoreCollision(comBall.colider, launcherCollider, true);

        aLauncher.Play("close");

        comBall.StartAlphaCoroutine(3f);
    }

    void BallClose()
    {
        aLauncher.Play("idle_closed");
    }
}
