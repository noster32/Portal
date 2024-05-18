using System.Collections;
using UnityEngine;

public class CBallLauncher : CComponent
{
    [SerializeField] private bool isStart = false;
    [SerializeField] private CBall ballPrefab;
    [SerializeField] private CBallCatcher ballCatcher;
    [SerializeField] private Transform ballLaunchTransform;

    [SerializeField] private float ballLifeTime = 16f;
    [SerializeField] private float ballSpeed = 5f;

    private bool isBallSpawned = false;

    Animator launcherAnimator;
    public override void Awake()
    {
        base.Awake();

        launcherAnimator = GetComponent<Animator>();
    }

    public override void Start()
    {
        base.Start();

        launcherAnimator.Play("close", 0, 1f);
    }

    public override void Update()
    {
        base.Update();

        if(isStart && !isBallSpawned && !ballCatcher.GetBallConnected())
            launcherAnimator.Play("open");
    }

    //애니메이션 이벤트로 실행
    private void BallLaunch()
    {
        isBallSpawned = true;
        CBall combineBall = Instantiate(ballPrefab, ballLaunchTransform.position, ballLaunchTransform.rotation);
        combineBall.speed = ballSpeed;
        combineBall.lifeTime = ballLifeTime;

        combineBall.DestroyBall(BallDestroyed);

        launcherAnimator.Play("close");
    }

    public void BallDestroyed(CBall ball)
    {
        Destroy(ball.gameObject);
        StartCoroutine(delayCoroutine());
    }

    private IEnumerator delayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        isBallSpawned = false;
    }

    public void BallLuncherStart()
    {
        isStart = true;
    }
}
