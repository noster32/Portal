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

    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private CBulletHoleSpawn bulletHoleSpawn;
    [SerializeField] private CBulletSoundSpawn bulletSoundSpawn;
    private Ray ray;
    private Action<CBullet> bulletDeleteAction;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        UpdateBullet();
    }

    public void DeleteBullet(Action<CBullet> deleteAction)
    {
        bulletDeleteAction = deleteAction;
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

        if(Physics.Raycast(ray, out hit, distance, ~LayerMask.GetMask("Turret", "Bullet", "Player")))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Portal"))
            {
                if(!portalPair.PlacedBothPortal())
                {
                    transform.position = hit.point;
                    PlayHitEffect(hit);
                    time = maxLifeTime;
                    return;
                }

                if(hit.collider.tag == portalPair.portals[0].tag)
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

                Func<Vector3, Vector3> relativeCal = (point) =>
                {
                    Vector3 relativePos = portal1.transform.InverseTransformPoint(point);
                    relativePos = reverse * relativePos;
                    return portal2.transform.TransformPoint(relativePos);
                };

                Vector3 telePortHitPos = relativeCal(hit.point);
                Vector3 teleportEndPos = relativeCal(endPosition);

                Vector3 velocity = velocityCal(telePortHitPos, teleportEndPos, 50f);

                Debug.DrawLine(telePortHitPos, teleportEndPos, Color.blue, 2f);

                initialPosition = telePortHitPos;
                initialVelocity = velocity;

                Teleport();
            }
            else
            {
                transform.position = hit.point;
                PlayHitEffect(hit);
                time = maxLifeTime;
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
                SetHitEffect(CBulletParticle.GetInstance().GetConcretParticle());
                bulletHoleSpawn.CreateBulletHoleConcret(hitInfo.point, hitInfo.normal);
                bulletSoundSpawn.PlayBulletHitConcretSound(hitInfo.point);
                break;
            case "Metal":
                SetHitEffect(CBulletParticle.GetInstance().GetMetalParticle());
                bulletHoleSpawn.CreateBulletHoleMetal(hitInfo.point, hitInfo.normal);
                bulletSoundSpawn.PlayBulletHitMetalSound(hitInfo.point);
                
                break;
            case "Glass":
                bulletHoleSpawn.CreateBulletHoleGlass(hitInfo.point, hitInfo.normal);
                bulletSoundSpawn.PlayBulletHitGlassSound(hitInfo.point);
                break;
            default:
                SetHitEffect(CBulletParticle.GetInstance().GetConcretParticle());
                bulletHoleSpawn.CreateBulletHoleConcret(hitInfo.point, hitInfo.normal);
                bulletSoundSpawn.PlayBulletHitConcretSound(hitInfo.point);
                break;
        }
    }

    
}
