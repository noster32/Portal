using System.Collections;
using UnityEngine;
public class CTurret : CGrabableObject
{
    [SerializeField] [Range(0.5f, 1f)] float aimLeftRight;
    [SerializeField] float aimUpDown;
    [SerializeField] private float falldownRotation = 50f;
    [SerializeField] private float normalizeAngle;
    [SerializeField] private CTurretLaser turretLaser;
    [SerializeField] private CPortalPair portalPair;

    private Material[] originalMaterials;                  //원본 머터리얼
    private Material[] cloneMaterials;                     //복제 머터리얼

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

    //컴포넌트 + 그래픽 클론 할당
    public override void Awake()
    {
        base.Awake();

        turretSound = GetComponent<CTurretSound>();
        turretDetect = GetComponent<CEnemyFieldOfView>();
        turretAnimator = GetComponentInChildren<Animator>();
        objectCollider = GetComponentInChildren<Collider>();

        grapicsClone = Instantiate(grapicsObject);
        Destroy(grapicsClone.transform.GetChild(1).gameObject);
        grapicsClone.transform.parent = grapicsObject.transform;
        grapicsClone.transform.GetChild(2).gameObject.layer = LayerMask.NameToLayer("PlayerClone");

        originalMaterials = GetMaterials(grapicsObject);
        cloneMaterials = GetMaterials(grapicsClone);
        GetAnimator(grapicsObject, grapicsClone);

        grapicsClone.SetActive(false);

    }

    public override void Start()
    {
        base.Start();

        if(portalPair == null)
            portalPair = CSceneManager.Instance.portalPair;
    }

    public override void Update()
    {
        base.Update();

        if (state == TurretState.DIE || state == TurretState.TURNOFF)
            return;

        ChangeTurretState();

        PlayTurretAnim();
        if (cloneAnimator && grapicsClone.activeSelf)
            PlayTurretCloneAnim();

        TurretLaserPoint();
        TurretLRAngle();
    }

    private void ChangeTurretState()
    {
        if (state == TurretState.FALLDOWN || state == TurretState.PICKUP)
            return;

        bool isRotatedEnough = this.transform.rotation.eulerAngles.z >= falldownRotation && this.transform.rotation.eulerAngles.z <= (360f - falldownRotation);

        if (isRotatedEnough && state != TurretState.FALLDOWN)
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            state = TurretState.FALLDOWN;
            animationCoroutine = StartCoroutine(TurretFallDownCoroutine());
            return;
        }

        if (isGrabbed && state != TurretState.PICKUP)
        {
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
                    break;

                case TurretState.SEARCHINGTARGET:
                    if (animationCoroutine != null)
                        StopCoroutine(animationCoroutine);

                    state = TurretState.ATTACK;
                    turretSound.PlayTurretActiveVoiceSound();
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

    private void TurretLRAngle()
    {
        if (turretDetect.canSeePlayer)
        {
            float changeAngleBasis = 0f;
            if (turretDetect.angleToTarget < 0)
                changeAngleBasis = Mathf.Abs(turretDetect.angleToTarget) + 90f;
            else
                changeAngleBasis = 90f - turretDetect.angleToTarget;

            float minAngle = 5f;
            float maxAngle = 175f;

            float centeredAngle = changeAngleBasis - minAngle;
            float range = maxAngle - minAngle;

            //포탈 애니메이션 정중앙이 0.75가아니라 0.8, 그래서 0.5 -> 0.55
            float normalizeValue = (centeredAngle / range) * 0.5f + 0.55f;
            aimLeftRight = Mathf.Abs(normalizeValue);
        }
        else
            aimLeftRight = 0.8f;
    }

    private void TurretLaserPoint()
    {
        RaycastHit hit;

        //170f -> TurretLRAngle : range 
        float inverseNormalize = ((aimLeftRight - 0.55f) / 0.5f) * 170f;
        inverseNormalize += 5f;

        if(inverseNormalize > 90f)
        {
            inverseNormalize -= 90f;
            inverseNormalize *= -1;
        }
        else
        {
            inverseNormalize = 90f - inverseNormalize;
        }

        Vector3 laserDirection = Quaternion.Euler(0f, inverseNormalize, 0f) * turretLaser.transform.forward;
        if (Physics.Raycast(turretLaser.transform.position, laserDirection, out hit, 50f, ~0, QueryTriggerInteraction.Ignore))
        {
            Collider[] portalColliders = Physics.OverlapSphere(hit.point, 0.5f, LayerMask.GetMask("PortalCollider"));
            if (portalColliders.Length > 0)
            {
                if (!portalPair.PlacedBothPortal())
                    return;

                CPortal portal;
                if (portalColliders[0].tag == "PortalB")
                {
                    portal = portalPair.portals[0];
                }
                else
                    portal = portalPair.portals[1];

                turretLaser.EnableLaserThroughPortal();

                Vector3 startPoint = portal.GetOtherPortalRelativePoint(hit.point);
                Vector3 laserDirectionThroughPortal = portal.GetOtherPortalRelativeDirection(laserDirection);

                RaycastHit hit2;
                if (Physics.Raycast(startPoint, laserDirectionThroughPortal, out hit2, 50f, ~LayerMask.GetMask("Portal", "PortalCollider"), QueryTriggerInteraction.Ignore))
                {
                    turretLaser.SetPointPortalLaser(startPoint, hit2.point);
                }
            }
            else
                turretLaser.DisableLaserThroughPortal();

            turretLaser.SetEndPoint(hit.point);
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

    private void PlayTurretCloneAnim()
    {
        switch (state)
        {
            case TurretState.IDLE:
                cloneAnimator.Play("turretIdle");
                break;

            case TurretState.ATTACK:
                cloneAnimator.CrossFade("aimLeftRight", 1f, 0, aimLeftRight);
                cloneAnimator.Play("turretFire1", 1);
                break;

            case TurretState.SEARCHINGTARGET:
                cloneAnimator.Play("aimLeftRight", 0, aimLeftRight);
                break;

            case TurretState.PICKUP:
                cloneAnimator.Play("aimLeftRight", 0, aimLeftRight);
                break;

            case TurretState.FALLDOWN:
                cloneAnimator.Play("aimLeftRight", 0, aimLeftRight);
                break;
        }
    }

    private IEnumerator TurretOnAndOffCoroutine()
    {
        switch (state)
        {
            case TurretState.DEPLOY:
                turretAnimator.Play("TurretDeploy");
                if(cloneAnimator && grapicsClone.activeSelf)
                    cloneAnimator.Play("TurretDeploy");
                turretSound.PlayTurretDeploySound();
                yield return new WaitForSeconds(0.5f);

                turretSound.PlayTurretActiveVoiceSound();
                yield return new WaitForSeconds(0.5f);

                state = TurretState.ATTACK;
                break;

            case TurretState.RETRACT:
                turretAnimator.CrossFade("aimNatural", 2f);
                if (cloneAnimator && grapicsClone.activeSelf)
                    cloneAnimator.CrossFade("aimNatural", 2f);
                yield return new WaitForSeconds(1f);

                turretAnimator.Play("TurretRetract");
                if (cloneAnimator && grapicsClone.activeSelf)
                    cloneAnimator.Play("TurretRetract");
                turretSound.PlayTurretSearchingRetireVoiceSound();
                turretSound.PlayTurretRetractSound();
                yield return new WaitForSeconds(2f);

                state = TurretState.IDLE;
                break;

            case TurretState.TURNOFF:
                turretAnimator.CrossFade("aimNatural", 2f);
                if (cloneAnimator && grapicsClone.activeSelf)
                    cloneAnimator.CrossFade("aimNatural", 2f);
                yield return new WaitForSeconds(1f);

                turretAnimator.Play("TurretRetract");
                if (cloneAnimator && grapicsClone.activeSelf)
                    cloneAnimator.Play("TurretRetract");
                turretSound.PlayTurretDIsableVoiceSound();
                turretSound.PlayTurretDieSound();
                yield return new WaitForSeconds(2f);

                turretLaser.DisableLaser();
                state = TurretState.DIE;
                break;
        }

        animationCoroutine = null;
        yield return null;
    }

    private IEnumerator TurretSearchingCoroutine()
    {
        float startNum;
        float elapsedTime;
        float duration = 1f;
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

        aimLeftRight = 0.8f;
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
