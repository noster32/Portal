using System.Collections;
using UnityEngine;

public class CBall : CComponent
{
    private bool isSpawned;
    public Rigidbody ballRigidbody;
    public Collider colider;
    float speed = 10f;

    private Renderer ballRenderer;

    [SerializeField] private AudioClip ballSound;
    [SerializeField] private AudioClip ballCollisionClip;
    [SerializeField] private AudioClip ballCollisionClip2;
    [SerializeField] private AudioClip ballBreakSound;

    private AudioSource audioSource;

    private Coroutine runCoroutine;

    public override void Awake()
    {
        base.Awake();

        ballRigidbody = GetComponent<Rigidbody>();
        ballRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        colider = GetComponent<Collider>();
        ballRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isSpawned)
            ballRigidbody.velocity = transform.forward * speed;
    }

    public void SetForward(Vector3 direction)
    {
        transform.forward = direction;
    }

    public void StartSound()
    {
        audioSource.clip = ballSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetIsSpawnedTrue()
    {
        isSpawned = true;
    }

    public void StopAlphaCoroutine()
    {
        if(runCoroutine != null)
            StopCoroutine(runCoroutine);

        isSpawned = false;
        gameObject.SetActive(false);
    }

    public void StartAlphaCoroutine(float duration)
    {
        runCoroutine = StartCoroutine(AlphaCoroutine(duration));
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
        
        ballRigidbody.velocity = Vector3.zero;

        audioSource.Stop();
        audioSource.PlayOneShot(ballBreakSound);

        yield return new WaitForSeconds(ballBreakSound.length);

        isSpawned = false;
        gameObject.SetActive(false);

        runCoroutine = null;
        

        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 ballNoraml = transform.forward;
        Vector3 collisionNormal = collision.contacts[0].normal;
        Vector3 reflectionDir = Vector3.Reflect(ballNoraml, collisionNormal);
        transform.forward = reflectionDir;

        audioSource.PlayOneShot(ballCollisionClip2);
        audioSource.PlayOneShot(ballCollisionClip);
    }
}
