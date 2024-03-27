using System.Collections;
using UnityEngine;

public class CPedestalPortalGunBullet : CComponent
{
    [Header("Setting")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxLifeTime = 1f;
    [SerializeField] private Gradient colorGradient;

    private TrailRenderer trail;
    private Coroutine destroyCoroutine;

    public override void Awake()
    {
        base.Awake();

        trail = GetComponent<TrailRenderer>();
    }

    public override void Start()
    {
        base.Start();

        trail.colorGradient = colorGradient;
        destroyCoroutine = StartCoroutine(bulletDestroyCoroutine(maxLifeTime));
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        m_oRigidBody.velocity = transform.forward * speed;
    }

    private IEnumerator bulletDestroyCoroutine(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("PortalPlaceable"))
        {
            StopCoroutine(destroyCoroutine);
            Destroy(this.gameObject);
        }
    }
}