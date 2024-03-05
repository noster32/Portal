using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CBoxDropper : CComponent
{
    [Header("Setting")]
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform boxSpawnTransform;
    [SerializeField] private Collider popupCubeCollider;

    [Header("Animator")]
    [SerializeField] Animator boxDropperAnimator;

    [Header("Sound")]
    [SerializeField] private AudioClip coverOpenClip;
    [SerializeField] private AudioClip coverCloseClip;

    private AudioSource audioSource;
    private Coroutine animCoroutine;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();

        boxDropperAnimator.Play("closing", 0, 1f);
    }

    public override void Update()
    {
        base.Update();

        //if(Input.GetKeyDown(KeyCode.I))
        //{
        //    BoxSpawn();
        //}
    }

    public void BoxSpawn()
    {
        if (animCoroutine == null)
            animCoroutine = StartCoroutine(BoxSpawnCoroutine());
    }

    private IEnumerator BoxSpawnCoroutine()
    {
        boxDropperAnimator.Play("opening");
        audioSource.PlayOneShot(coverOpenClip, CSoundLoader.Instance.effectSoundVolume);
        yield return new WaitForSeconds(boxDropperAnimator.GetCurrentAnimatorClipInfo(0).Length);


        boxDropperAnimator.Play("closing");
        audioSource.PlayOneShot(coverCloseClip, CSoundLoader.Instance.effectSoundVolume);
        yield return new WaitForSeconds(boxDropperAnimator.GetCurrentAnimatorClipInfo(0).Length - 0.1f);


        Instantiate(boxPrefab, boxSpawnTransform.position, Random.rotation);
        popupCubeCollider.enabled = false;
        yield return new WaitForSeconds(0.8f);

        popupCubeCollider.enabled = true;
        animCoroutine = null;
        yield return null;
    }
}
