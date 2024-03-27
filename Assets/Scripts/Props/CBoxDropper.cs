using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CBoxDropper : CComponent
{
    [Header("Setting")]
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform boxSpawnTransform;
    [SerializeField] private Collider popupCubeCollider;

    [Header("Animator")]
    [SerializeField] Animator boxDropperAnimator;

    private Coroutine animCoroutine;

    public override void Start()
    {
        base.Start();

        boxDropperAnimator.Play("closing", 0, 1f);
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.I))
        {
            BoxSpawn();
        }
    }

    public void BoxSpawn()
    {
        if (animCoroutine == null)
            animCoroutine = StartCoroutine(BoxSpawnCoroutine());
    }

    private IEnumerator BoxSpawnCoroutine()
    {
        boxDropperAnimator.Play("opening");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door2, this.transform.position);
        yield return new WaitForSeconds(boxDropperAnimator.GetCurrentAnimatorClipInfo(0).Length);


        boxDropperAnimator.Play("closing");
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.door3, this.transform.position);
        yield return new WaitForSeconds(boxDropperAnimator.GetCurrentAnimatorClipInfo(0).Length - 0.1f);


        Instantiate(boxPrefab, boxSpawnTransform.position, Random.rotation);
        popupCubeCollider.enabled = false;
        yield return new WaitForSeconds(0.8f);

        popupCubeCollider.enabled = true;
        animCoroutine = null;
        yield return null;
    }
}
