using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CToilet : CComponent
{
    [SerializeField] private AudioClip useToiletFlushClip;
    [SerializeField] private AudioClip useToiletThanksClip;

    private AudioSource audioSource;
    private bool isPlaying;

    public override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();

    }

    public void PlayToiletSound()
    {
        if (isPlaying)
            return;

        StartCoroutine(PlayClips());
    }

    private IEnumerator PlayClips()
    {
        isPlaying = true;
        audioSource.PlayOneShot(useToiletFlushClip, 0.1f);

        yield return new WaitForSeconds(4f);

        audioSource.PlayOneShot(useToiletThanksClip, 0.1f);

        yield return new WaitForSeconds(useToiletThanksClip.length);
        isPlaying = false;
    }


}
