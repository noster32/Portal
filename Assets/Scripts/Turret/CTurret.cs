using System.Collections;
using UnityEngine;



public class CTurret : CGrabableObject
{
    [SerializeField] [Range(0.5f, 1f)] float aimLeftRight;
    [SerializeField] float aimUpDown;
    [SerializeField] private float falldownRotation = 50f;
    [SerializeField] private float normalizeAngle;

    private CTurretSound turretSound;
    private CEnemyFieldOfView turretDetect;
    private Animator turretAnimator;

    float randomNumber;
    private Coroutine animationCoroutine;

    public enum TurretState
    {
        IDLE,                   //기본
        DEPLOY,                 //적 확인
        ATTACK,                 //공격
        SEARCHINGTARGET,        //적이 시야에서 벗어남
        RETRACT,                //서칭 종료
        PICKUP,                    //잡혀있는 경우
        FALLDOWN,
        TURNOFF,
        DIE
    }

    private TurretState state;

    public TurretState GetState
    {
        get { return state; }
    }


    public override void Awake()
    {
        base.Awake();
        turretSound = GetComponent<CTurretSound>();
        turretDetect = GetComponent<CEnemyFieldOfView>();
        turretAnimator = GetComponentInChildren<Animator>();
    }

    public override void Update()
    {
        base.Update();

        if (state == TurretState.DIE || state == TurretState.TURNOFF)
            return;

        ChangeTurretState();
        PlayTurretAnim();
        
        normalizeAngle = (turretDetect.angleToTarget / (turretDetect.angle * 2)) - 0.8f;
        aimLeftRight = Mathf.Abs(normalizeAngle);
    }

    private void ChangeTurretState()
    {
        if (state == TurretState.FALLDOWN || state == TurretState.PICKUP)
            return;

        bool isRotatedEnough = this.transform.rotation.eulerAngles.z >= falldownRotation && this.transform.rotation.eulerAngles.z <= (360f - falldownRotation);

        if (isRotatedEnough && state != TurretState.FALLDOWN)
        {
            Debug.Log("state Fall Down");
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            state = TurretState.FALLDOWN;
            animationCoroutine = StartCoroutine(TurretFallDownCoroutine());
            return;
        }

        if (isGrabbed && state != TurretState.PICKUP)
        {
            Debug.Log("state Grab");
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            state = TurretState.PICKUP;
            animationCoroutine = StartCoroutine(TurretPickUpCoroutine());
            return;
        }

        if (turretDetect.canSeePlayer)
        {
            switch (state)
            {
                case TurretState.IDLE:
                    state = TurretState.DEPLOY;
                    animationCoroutine = StartCoroutine(TurretOnAndOffCoroutine());
                    Debug.Log("state Deploy");
                    break;

                case TurretState.SEARCHINGTARGET:
                    if (animationCoroutine != null)
                        StopCoroutine(animationCoroutine);

                    state = TurretState.ATTACK;
                    turretSound.PlayTurretActiveVoiceSound();
                    Debug.Log("state Attack");
                    break;
            }
        }
        else
        {
            if(state == TurretState.DEPLOY || state == TurretState.ATTACK)
            {
                if (animationCoroutine != null)
                    StopCoroutine(animationCoroutine);

                state = TurretState.SEARCHINGTARGET;
                animationCoroutine = StartCoroutine(TurretSearchingCoroutine());
            }
        }
    }

    private void PlayTurretAnim()
    {
        switch (state)
        {
            case TurretState.IDLE:
                turretAnimator.Play("turretIdle");
                break;

            case TurretState.ATTACK:
                turretAnimator.CrossFade("aimLeftRight", 1f, 0, aimLeftRight);
                turretAnimator.Play("turretFire1", 1);
                break;

            case TurretState.SEARCHINGTARGET:
                turretAnimator.Play("aimLeftRight", 0, aimLeftRight);
                break;

            case TurretState.PICKUP:
                turretAnimator.Play("aimLeftRight", 0, aimLeftRight);
                break;

            case TurretState.FALLDOWN:
                turretAnimator.Play("aimLeftRight", 0, aimLeftRight);
                break;
        }
    }

    private IEnumerator TurretOnAndOffCoroutine()
    {
        switch (state)
        {
            case TurretState.DEPLOY:
                turretAnimator.Play("TurretDeploy");
                turretSound.PlayTurretDeploySound();
                yield return new WaitForSeconds(0.5f);
                turretSound.PlayTurretActiveVoiceSound();
                yield return new WaitForSeconds(0.5f);

                state = TurretState.ATTACK;
                break;

            case TurretState.RETRACT:
                Debug.Log("AimNatural");
                turretAnimator.CrossFade("aimNatural", 2f);
                yield return new WaitForSeconds(1f);

                Debug.Log("Retract");
                turretAnimator.Play("TurretRetract");
                turretSound.PlayTurretSearchingRetireVoiceSound();
                turretSound.PlayTurretRetractSound();
                yield return new WaitForSeconds(2f);

                Debug.Log("Retract Done");
                state = TurretState.IDLE;
                break;

            case TurretState.TURNOFF:
                Debug.Log("Turn Off Aim Natural");
                turretAnimator.CrossFade("aimNatural", 2f);
                yield return new WaitForSeconds(1f);

                Debug.Log("Turn Off");
                turretAnimator.Play("TurretRetract");
                turretSound.PlayTurretDIsableVoiceSound();
                turretSound.PlayTurretDieSound();
                yield return new WaitForSeconds(2f);

                Debug.Log("Turret Die");
                state = TurretState.DIE;
                break;
        }

        Debug.Log("Animation finished");
        animationCoroutine = null;
        yield return null;
    }

    private IEnumerator TurretSearchingCoroutine()
    {
        float startNum;
        float elapsedTime;
        float duration = 1f;
        Debug.Log("Searching Start");
        turretSound.PlayTurretSearchingVoiceSound();

        for (int i = 0; i < 5; i++)
        {
            startNum = aimLeftRight;
            elapsedTime = 0f;

            //랜덤 변수를 통해 왼쪽 오른쪽으로 터렛 에임이 이동한다
            if (aimLeftRight < 0.8f)
            {
                randomNumber = Random.Range(0.85f, 0.95f);
            }
            else
            {
                randomNumber = Random.Range(0.65f, 0.75f);
            }


            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                aimLeftRight = Mathf.Lerp(startNum, randomNumber, t);

                yield return null;
            }

            aimLeftRight = randomNumber;
            turretSound.PlayTurretPingSound();
        }

        animationCoroutine = null;
        state = TurretState.RETRACT;
        animationCoroutine = StartCoroutine(TurretOnAndOffCoroutine());
        
        yield return null;
    }

    private IEnumerator TurretFallDownCoroutine()
    {
        float startNum;
        float elapsedTime;
        float duration = 0.2f;
        float fallDownShootDuration = 3f;
        float startTime = Time.time;

        Debug.Log("Turret Fall Down Fire");
        turretSound.PlayTurretFallDownVoiceSound();

        while (Time.time - startTime < fallDownShootDuration)
        {
            
            startNum = aimLeftRight;
            elapsedTime = 0f;

            //랜덤 변수를 통해 왼쪽 오른쪽으로 터렛 에임이 이동한다
            if (aimLeftRight < 0.8f)
            {
                randomNumber = Random.Range(0.85f, 0.95f);
            }
            else
            {
                randomNumber = Random.Range(0.65f, 0.75f);
            }


            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                aimLeftRight = Mathf.Lerp(startNum, randomNumber, t);

                yield return null;
            }

            aimLeftRight = randomNumber;
        }

        animationCoroutine = null;
        state = TurretState.TURNOFF;
        animationCoroutine = StartCoroutine(TurretOnAndOffCoroutine());

        yield return null;
    }

    private IEnumerator TurretPickUpCoroutine()
    {
        float startNum;
        float elapsedTime;
        float duration = 0.4f;
        Debug.Log("Turret Grab");
        turretSound.PlayTurretPickUpVoiceSound();

        while(isGrabbed)
        {
            startNum = aimLeftRight;
            elapsedTime = 0f;

            turretSound.PlayTurretGunRotationSound();

            //랜덤 변수를 통해 왼쪽 오른쪽으로 터렛 에임이 이동한다
            if (aimLeftRight < 0.8f)
            {
                randomNumber = Random.Range(0.85f, 0.95f);
            }
            else
            {
                randomNumber = Random.Range(0.65f, 0.75f);
            }


            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                aimLeftRight = Mathf.Lerp(startNum, randomNumber, t);

                yield return null;
            }

            aimLeftRight = randomNumber;
        }

        animationCoroutine = null;
        state = TurretState.SEARCHINGTARGET;
        animationCoroutine = StartCoroutine(TurretSearchingCoroutine());

        yield return null;
    }
}
