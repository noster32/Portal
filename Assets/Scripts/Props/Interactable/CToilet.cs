using FMOD.Studio;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CToilet : CComponent
{
    [SerializeField] private CInteractObject useToiletInteraction;

    private Coroutine playCoroutine;

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
        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.useToiletFlush, this.transform.position);

        yield return new WaitForSeconds(4f);

        CAudioManager.Instance.PlayOneShot(CFMODEventLab00.Instance.useToiletThanks, this.transform.position);

        yield return new WaitForSeconds(3f);

        playCoroutine = null;
    }


}
