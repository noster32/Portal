using System;
using UnityEngine;

public class CBullet : CTeleportObject
{
    #region DefaultSetting
    [HideInInspector] public float speed = 100.0f;
    [HideInInspector] public float distance = 500.0f;

    [HideInInspector] public float time;
    [HideInInspector] public float maxLifeTime;
    [HideInInspector] public Vector3 initialPosition;
    [HideInInspector] public Vector3 initialVelocity;
    [HideInInspector] public Vector3 endPosition;
    [HideInInspector] public float bulletDrop;
    #endregion

    [Header("Setting")]
    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private CPlayerState playerState;

    [Header("Particle")]
    [SerializeField] private ParticleSystem concretHitParticle;
    [SerializeField] private ParticleSystem metalHitParticle;
    
    private Ray ray;
    private TrailRenderer trailRenderer;
    private Action<CBullet> bulletDeleteAction;
    private Action<Vector3, Vector3> concretHoleSpawn;
    private Action<Vector3, Vector3> metalHoleSpawn;
    private Action<Vector3, Vector3> glassHoleSpawn;

    public override void Awake()
    {
        base.Awake();

        trailRenderer = GetComponent<TrailRenderer>();
    }

    public override void Start()
    {
        base.Start();

        if(portalPair == null)
            portalPair = CSceneManager.Instance.portalPair;
        if(playerState == null)
            playerState = CSceneManager.Instance.player.transform.GetComponent<CPlayerState>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        UpdateBullet();
    }

    public void DeleteBullet(Action<CBullet> deleteAction)
    {
        bulletDeleteAction = deleteAction;
    }

    public void BulletHoleFunction(Action<Vector3, Vector3> concret, Action<Vector3, Vector3> metal, Action<Vector3, Vector3> glass)
    {
        concretHoleSpawn = concret;
        metalHoleSpawn = metal;
        glassHoleSpawn = glass;
    }

    public Vector3 GetPosition()
    {
        //p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        return initialPosition + (initialVelocity * time) + (0.5f * gravity * time * time);
    }

    private void UpdateBullet()
    {
        Vector3 p0 = GetPosition();
        time += Time.deltaTime;
        Vector3 p1 = GetPosition();
        RaycastSegment(p0, p1);

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if(time >= maxLifeTime)
        {
            if (bulletDeleteAction != null)
            {
                bulletDeleteAction(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void RaycastSegment(Vector3 start, Vector3 end)
    {
        RaycastHit hit;
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        ray.origin = start;
        ray.direction = direction;

        if(Physics.Raycast(ray, out hit, distance, ~0, QueryTriggerInteraction.Ignore))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (!playerState)
                    playerState = hit.transform.GetComponent<CPlayerState>();

                playerState.DealDamageToPlayer(10, initialVelocity, 10f);
                //피 데칼을 뿌리고 싶은 경우에는 hit.point부터 initialVecocity방향으로 레이를 쏴서 거기에 맞는 오브젝트에
                //데칼을 칠하면 되지않을까
                time = maxLifeTime;

            }
            else
            {
                if (!portalPair.PlacedBothPortal())
                {
                    transform.position = hit.point;
                    PlayHitEffect(hit);
                    time = maxLifeTime;
                    return;
                }

                Collider[] portalColliders = Physics.OverlapSphere(hit.point, 0.5f, LayerMask.GetMask("PortalCollider"));
                if(portalColliders.Length > 0)
                {
                    Debug.Log(portalColliders[0].tag);
                    if (portalColliders[0].tag == portalPair.portals[0].tag)
                    {
                        this.portal1 = portalPair.portals[0];
                        this.portal2 = portalPair.portals[1];
                    }
                    else
                    {
                        this.portal1 = portalPair.portals[1];
                        this.portal2 = portalPair.portals[0];
                    }

                    Func<Vector3, Vector3, float, Vector3> velocityCal = (start, end, speed) => (end - start).normalized * speed;

                    Vector3 telePortHitPos = portal1.GetOtherPortalRelativePoint(hit.point);
                    Vector3 teleportEndPos = portal1.GetOtherPortalRelativePoint(endPosition);

                    Vector3 velocity = velocityCal(telePortHitPos, teleportEndPos, 50f);
                    initialPosition = telePortHitPos;
                    initialVelocity = velocity;

                    //트레일 렌더러는 false를 하더라도 트레일이 계속 보임
                    //텔레포트는 제대로 되지만 건너편 포탈의 일정 범위 이내에는 플레이어가 총알을 맞지않음
                    trailRenderer.enabled = false;
                    Teleport();
                }
                else
                {
                    transform.position = hit.point;
                    PlayHitEffect(hit);
                    time = maxLifeTime;
                }
            }
        }
        else
        {
            transform.position = end; 
        }
    }

    private void PlayHitEffect(RaycastHit hitInfo)
    {
        Action<ParticleSystem> SetHitEffect = (particle) =>
        {
            particle.transform.position = hitInfo.point;
            particle.transform.forward = hitInfo.normal;
            particle.Emit(1);
        };

        switch(hitInfo.collider.tag)
        {
            case "Concret":
                CParticleManager.Instance.PlayParticleEmit(concretHitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                concretHoleSpawn(hitInfo.point, hitInfo.normal);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.concreteImpactBullet, hitInfo.point);
                break;
            case "Metal":
                CParticleManager.Instance.PlayParticleEmit(metalHitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                metalHoleSpawn(hitInfo.point, hitInfo.normal);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.metalImpactBullet, hitInfo.point);
                break;
            case "Glass":
                glassHoleSpawn(hitInfo.point, hitInfo.normal);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.glassImpactBullet, hitInfo.point);
                break;
            default:
                CParticleManager.Instance.PlayParticleEmit(metalHitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                metalHoleSpawn(hitInfo.point, hitInfo.normal);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.metalImpactBullet, hitInfo.point);
                break;
        }
    }

    public override void Teleport()
    {
        base.Teleport();

        trailRenderer.enabled = true;
    }

    private void OnDisable()
    {
        trailRenderer.enabled = false;
    }

    private void OnEnable()
    {
        trailRenderer.enabled = true;
    }


}
