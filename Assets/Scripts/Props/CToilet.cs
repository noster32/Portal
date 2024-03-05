using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CToilet : CComponent
{
    [SerializeField] private AudioClip useToiletFlushClip;
    [SerializeField] private AudioClip useToiletThanksClip;

    [SerializeField] private CInteractObject useToiletInteraction;

    private AudioSource audioSource;
    private Coroutine playCoroutine;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if(useToiletInteraction)
            useToiletInteraction.GetInteractEvent.HasInteracted += PlayToiletSound;
    }

    private void OnDisable()
    {
        if(useToiletInteraction)
            useToiletInteraction.GetInteractEvent.HasInteracted -= PlayToiletSound;
    }

    private void PlayToiletSound()
    {
        if (playCoroutine == null)
            playCoroutine = StartCoroutine(PlayClips());
    }

    private IEnumerator PlayClips()
    {
        audioSource.PlayOneShot(useToiletFlushClip, CSoundLoader.Instance.GetEffectVolume(0.6f));

        yield return new WaitForSeconds(4f);

        audioSource.PlayOneShot(useToiletThanksClip, CSoundLoader.Instance.GetEffectVolume(0.6f));

        yield return new WaitForSeconds(useToiletThanksClip.length);

        playCoroutine = null;
    }


}
