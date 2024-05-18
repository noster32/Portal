using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHazardGoo : CComponent
{
    [SerializeField] private float gravityScale = 1.0f;
    [SerializeField] private float globalGravity = -9.81f;
    [SerializeField] private GameObject gooFx;

    [SerializeField] Transform playerTransform;
    private Rigidbody playerRigidbody;
    private CPlayerState state;

    private bool isInWater = false;
    private Coroutine damageCoroutine;

    public override void Awake()
    {
        base.Awake();

    }

    public override void Start()
    {
        base.Start();

        if (playerTransform == null)
            playerTransform = CSceneManager.Instance.player.transform;

        playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        state = playerTransform.GetComponent<CPlayerState>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(isInWater)
        {
            if (!state.GetIsPlayerDie())
            {
                Vector3 gravity = globalGravity * gravityScale * playerTransform.up;
                playerRigidbody.AddForce(gravity);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerRigidbody.useGravity = false;
            isInWater = true;
            gooFx.SetActive(true);
            RenderSettings.fog = true;

            if(damageCoroutine == null)
                damageCoroutine = StartCoroutine(InsideGooDamageCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerRigidbody.useGravity = true;
            isInWater = false;
            gooFx.SetActive(false);
            RenderSettings.fog = false;

            StopCoroutine(damageCoroutine);
        }
    }

    private IEnumerator InsideGooDamageCoroutine()
    {
        while(!state.GetIsPlayerDie())
        {
            state.DealDamageToPlayer(20); 

            yield return new WaitForSeconds(0.3f);
        }

        damageCoroutine = null;
    }
}
