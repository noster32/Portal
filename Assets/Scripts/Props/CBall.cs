using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class CBall : CTeleportObject
{
    [Header("Setting")]
    [SerializeField] private bool infiniteLiftTime = false;
    public float speed = 5f;
    public float lifeTime = 16f;

    [Header("Particle")]
    [SerializeField] private ParticleSystem sparkParticle;
    [SerializeField] private ParticleSystem explodeParticle;

    private Renderer ballRenderer;
    private StudioEventEmitter emitter;
    private Coroutine runCoroutine;

    public delegate void ballDestroyDelegate(CBall ball);
    public ballDestroyDelegate isSpawnedFalseFunc;
    private Action<CBall> isSpawnedFalseAction;
    //Action => delegate¶û µ¿ÀÏ predefined delegate

    public override void Awake()
    {
        base.Awake();

        m_oRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        ballRenderer = GetComponent<Renderer>();
    }

    public override void Start()
    {
        base.Start();

        emitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEventsEnergy.Instance.energySingLoop4, this.gameObject);
        emitter.Play();

        if(!infiniteLiftTime)
            runCoroutine = StartCoroutine(AlphaCoroutine(lifeTime));
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        m_oRigidBody.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PortalCollider"))
            return;
        if (collision.transform.tag == "Pedestal")
            return;
        //player hit player die

        Vector3 ballNoraml = transform.forward;
        Vector3 collisionNormal = collision.contacts[0].normal;
        Vector3 reflectionDir = Vector3.Reflect(ballNoraml, collisionNormal);
        transform.forward = reflectionDir;

        CParticleManager.Instance.PlayParticle(sparkParticle, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal), 0.2f);
        CAudioManager.Instance.PlayOneShot(CFMODEventsEnergy.Instance.energyBounce1, this.transform.position);
        CAudioManager.Instance.PlayOneShot(CFMODEventsEnergy.Instance.energyBounce2, this.transform.position);
    }

    public void StartAlphaCoroutine()
    {
        runCoroutine = StartCoroutine(AlphaCoroutine(lifeTime));
    }

    public void StopAlphaCoroutine()
    {
        if(runCoroutine != null)
            StopCoroutine(runCoroutine);

        emitter.Stop();
        //isSpawnedFalseFunc.Invoke(this);
        isSpawnedFalseAction(this);

    }

    public void DestroyBall(Action<CBall> action)
    {
        isSpawnedFalseAction = action;
    }

    public IEnumerator AlphaCoroutine(float duration)
    {
        float elapsedTime = 0.0f;
        Color c = ballRenderer.material.color;

        while (elapsedTime < duration)
        {
            c.a = 1.0f - elapsedTime / duration;
            ballRenderer.material.color = c;
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        m_oRigidBody.velocity = Vector3.zero;

        emitter.Stop();
        CParticleManager.Instance.PlayParticle(explodeParticle, this.transform.position, this.transform.rotation);
        CAudioManager.Instance.PlayOneShot(CFMODEventsEnergy.Instance.energySingExplosion2, this.transform.position);
        runCoroutine = null;

        //isSpawnedFalseFunc.Invoke(this);
        isSpawnedFalseAction(this);
    }

}
