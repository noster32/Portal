using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;



public class CTurret : CComponent
{
    #region private

    [SerializeField] [Range(0.5f, 1f)]
    float aimLeftRight;

    [SerializeField]
    float aimUpDown;

    [SerializeField]
    private float falldownRotation = 50f;

    #endregion

    public enum TurretState
    {
        IDLE,                   //기본
        DEPLOY,                 //적 확인
        ATTACK,                 //공격
        SEARCHINGTARGET,        //적이 시야에서 벗어남
        RETRACT,                //서칭 종료
        RETURNTOIDLE,           //IDLE로 되돌아가는 모션
        AIR,                    //공중에 떠있는 경우
        FALLDOWN,
        TURNOFF,
        TURNOFFRETRACT,
        DIE
    }

    TurretState state;

    public TurretState GetState
    {
        get { return state; }
    }

    CEnemyFieldOfView turretDetect;

    Animator turretAnimator;

    [SerializeField]
    float normalizeAngle;

    float randomNumber;

    public override void Awake()
    {
        base.Awake();

        turretDetect = GetComponent<CEnemyFieldOfView>();
        turretAnimator = GetComponent<Animator>();
        
    }

    public override void Update()
    {
        base.Update();

        if (state == TurretState.DIE)
        {
            return;
        }

        TurretStateChange();
        TurretAnim();
        
        normalizeAngle = (turretDetect.AngleToTarget / (turretDetect.angle * 2)) - 0.8f;
        aimLeftRight = Mathf.Abs(normalizeAngle);
    }

    private void TurretStateChange()
    {
        bool isRotatedEnough = this.transform.rotation.eulerAngles.z >= falldownRotation && this.transform.rotation.eulerAngles.z <= (360f - falldownRotation);
        bool turretFallDown = state != TurretState.FALLDOWN && state != TurretState.TURNOFFRETRACT && state != TurretState.TURNOFF && state != TurretState.DIE && state != TurretState.AIR;

        if (isRotatedEnough && turretFallDown)
        {
            Debug.Log("state Fall Down");
            state = TurretState.FALLDOWN;
            StartCoroutine(TurretFallDownCoroutine());
        }
        else if (turretDetect.canSeePlayer && state == TurretState.IDLE && state != TurretState.SEARCHINGTARGET )
        {
            state = TurretState.DEPLOY;
            Debug.Log("state Deploy");
            StartCoroutine(TurretAnimEnumerator());

            Debug.Log("state Attack");
        }
        else if(turretDetect.canSeePlayer && state == TurretState.SEARCHINGTARGET)
        {
            state = TurretState.ATTACK;
            Debug.Log("state Attack");
            StartCoroutine(TurretAnimEnumerator());
        }
        else if(!turretDetect.canSeePlayer && (state == TurretState.DEPLOY || state == TurretState.ATTACK))
        {
            TurretSearching();
        }
    }

    private void TurretAnim()
    {
        if (state == TurretState.ATTACK)
        {
            turretAnimator.CrossFade("aimLeftRight", 1f, 0, aimLeftRight);
            turretAnimator.Play("turretFire1", 1);
        }
        else if (state == TurretState.RETRACT)
        {
            state = TurretState.RETURNTOIDLE;
            StartCoroutine(TurretAnimEnumerator());
        }
        else if (state == TurretState.SEARCHINGTARGET)
        {
            turretAnimator.Play("aimLeftRight", 0, aimLeftRight);
        }
        else if (state == TurretState.FALLDOWN)
        {
            turretAnimator.Play("aimLeftRight", 0, aimLeftRight);
        }
        else if (state == TurretState.TURNOFF)
        {
            state = TurretState.TURNOFFRETRACT;
            StartCoroutine(TurretAnimEnumerator());
        }
    }

    private void TurretSearching()
    {
        Debug.Log("TurretSearching");
        state = TurretState.SEARCHINGTARGET;
        StartCoroutine(TurretSearchingCoroutine());
    }


    private IEnumerator TurretAnimEnumerator()
    {
        if(state == TurretState.DEPLOY)
        {
            turretAnimator.Play("TurretDeploy");
            yield return new WaitForSeconds(1f);

            state = TurretState.ATTACK;
        }
        else if(state == TurretState.RETURNTOIDLE)
        {
            Debug.Log("AimNatural");
            turretAnimator.CrossFade("aimNatural", 2f);
            yield return new WaitForSeconds(1f);

            Debug.Log("Retract");
            turretAnimator.Play("TurretRetract");
            yield return new WaitForSeconds(2f);

            Debug.Log("Retract Done");
            state = TurretState.IDLE;
        }
        else if(state == TurretState.TURNOFFRETRACT)
        {
            Debug.Log("Turn Off Aim Natural");
            turretAnimator.CrossFade("aimNatural", 2f);
            yield return new WaitForSeconds(1f);

            Debug.Log("Turn Off");
            turretAnimator.Play("TurretRetract");
            yield return new WaitForSeconds(2f);

            Debug.Log("Turret Die");
            state = TurretState.DIE;
        }

        Debug.Log("Animation finished");

        yield return null;
    }

    private IEnumerator TurretSearchingCoroutine()
    {
        float startNum;
        float elapsedTime;
        float duration = 1f;
        Debug.Log("Searching Start");

        for (int i = 0; i < 5; i++)
        {
            if(turretDetect.canSeePlayer || state == TurretState.FALLDOWN)
            {
                break;
            }

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

        if(!turretDetect.canSeePlayer)
        {
            state = TurretState.RETRACT;
        }
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

        state = TurretState.TURNOFF;

        yield return null;
    }
}
