using System;
using UnityEngine;

public class CBullet : CComponent
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
    private Action<Vector3, Vector3, ObjectMaterial, Transform> bulletHoleSpawn;

    public override void Awake()
    {
        base.Awake();

        trailRenderer = GetComponent<TrailRenderer>();
    }
    
    private void OnEnable()
    {
        trailRenderer.enabled = true;
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

    public void DeleteBullet(Action<CBullet> deleteAction) => bulletDeleteAction = deleteAction;

    public void SetBulletHoleSpawn(Action<Vector3, Vector3, ObjectMaterial, Transform> setBulletHoleSpawn) => 
        bulletHoleSpawn = setBulletHoleSpawn;

    public Vector3 GetPosition()
    {
        //p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        return initialPosition + (initialVelocity * time) + (0.5f * gravity * time * time);
    }

    //총알 위치 업데이트
    //GetPosition사이에 시간을 경과하게 하여 현재 포지션과 이후의 포지션 사이를 레이캐스팅한다
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
                bulletDeleteAction(this);
            else
                Destroy(this.gameObject);
        }
    }

    //포탈의 현재 위치와 다음 위치에서 레이캐스팅을 통해 충돌을 감지
    //플레이어에 충돌할 경우 데미지를 주고 플레이어 이외에 충돌할 경우에는
    //OverlapSphere를 통해서 포탈여부를 체크하고 아닐 경우 이펙트 출력 포탈일 경우에는 반대편 포탈로 텔레포트한다
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

                playerState.DealDamageToPlayer(0, initialVelocity, 0);
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
                    CPortal enterPortal = portalPair.CheckPortalTag(portalColliders[0].tag);
                    BulletTeleport(enterPortal, hit.point);
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
            transform.position = end; 
    }

    //총알이 오브젝트에 충돌했을 떄의 이펙트
    //벽면이 아닌 오브젝트에 충돌 했을 경우 오브젝트를 parent로 설정해 bulletHole과 오브젝트와 같이 움직인다
    private void PlayHitEffect(RaycastHit hitInfo)
    {
        switch(hitInfo.collider.tag)
        {
            case "Concret":
                CParticleManager.Instance.PlayParticleEmit(concretHitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                bulletHoleSpawn(hitInfo.point, hitInfo.normal, ObjectMaterial.CONCRETE, hitInfo.transform);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.concreteImpactBullet, hitInfo.point);
                break;
            case "Metal":
                CParticleManager.Instance.PlayParticleEmit(metalHitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                bulletHoleSpawn(hitInfo.point, hitInfo.normal, ObjectMaterial.METAL, hitInfo.transform);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.metalImpactBullet, hitInfo.point);
                break;
            case "Glass":
                bulletHoleSpawn(hitInfo.point, hitInfo.normal, ObjectMaterial.GLASS, hitInfo.transform);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.glassImpactBullet, hitInfo.point);
                break;
            default:
                CParticleManager.Instance.PlayParticleEmit(metalHitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                bulletHoleSpawn(hitInfo.point, hitInfo.normal, ObjectMaterial.METAL, hitInfo.transform);
                CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.metalImpactBullet, hitInfo.point);
                break;
        }
    }

    //총알 텔레포트
    //총알이 발사되고 경과한 시간을 총 라이프타임에서 제외하고 time을 0으로 해서 시작 포인트를 hitPoint로 재설정한다
    private void BulletTeleport(CPortal portal, Vector3 hitPoint)
    {
        Vector3 telePortHitPos = portal.GetOtherPortalRelativePoint(hitPoint);
        Vector3 velocity = portal.GetOtherPortalRelativeDirection(initialVelocity);

        initialPosition = telePortHitPos;
        initialVelocity = velocity;

        maxLifeTime -= time;
        time = 0f;
    }


}
