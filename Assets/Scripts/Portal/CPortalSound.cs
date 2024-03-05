using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalSound : CComponent
{
    private AudioSource m_audioSource;

    public override void Awake()
    {
        base.Awake();

        m_audioSource = GetComponent<AudioSource>();
    }

    public void PlayPortalSound(AudioClip clip, float volume = 1f)
    {
        StartCoroutine(PortalSoundCoroutine(clip, volume));
    }

    private IEnumerator PortalSoundCoroutine(AudioClip clip, float volume)
    {
        m_audioSource.PlayOneShot(clip, CSoundLoader.Instance.GetEffectVolume(volume));

        yield return new WaitForSeconds(clip.length);

        Destroy(this.gameObject);
    }
}
