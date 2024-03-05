using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CLab02 : CComponent
{
    [Header("Event")]
    [SerializeField] UnityEvent entryDoorEvent;

    [Header("Aperture Sound")]
    [SerializeField] private float voiceVolume = 1f;
    [SerializeField] private AudioClip blipSound;
    [SerializeField] private AudioClip[] part1EntryAISoundClip;
    [SerializeField] private AudioClip[] part1GetPortalGunAISoundClip;
    [SerializeField] private AudioClip part2EntryAISoundClip;
    [SerializeField] private AudioClip part2SuccessAISoundClip;

    [Header("Ambient Sound")]
    [SerializeField] private AudioClip Lab02Part1Music;
    [SerializeField] private AudioClip changeAmbient;

    private bool isPart1Entry = false;
    private bool isPart1GetPortalGun = false;
    private bool isPart2Entry = false;
    private bool isPart2Success = false;

    public void PlayPart1Entry()
    {
        if(!isPart1Entry)
        {
            isPart1Entry = true;
            StartCoroutine(Part1EntryCoroutine());
        }
    }
    
    public void PlayPart1GetPortalGun()
    {
        if(!isPart1GetPortalGun)
        {
            isPart1GetPortalGun = true;
            StartCoroutine(Part1GetPortalGunCoroutine());
        }
    }

    public void PlayPart2Entry()
    {
        if(!isPart2Entry) 
        {
            isPart2Entry = true;
            StartCoroutine(Part2EntryCoroutine());
        }
    }

    public void PlayPart2Success()
    {
        if(!isPart2Success)
        {
            isPart2Success = true;
            StartCoroutine(Part2SuccessCoroutine());
        }
    }
    public void Part2EntryMusic()
    {
        CSoundLoader.Instance.PlaySoundOneShot(Lab02Part1Music, 0.05f);
    }

    private IEnumerator Part1EntryCoroutine()
    {
        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);

        yield return new WaitForSeconds(blipSound.length);

        foreach (AudioClip aiSound in part1EntryAISoundClip)
        {
            CSoundLoader.Instance.PlaySoundOneShot(aiSound, voiceVolume);

            yield return new WaitForSeconds(aiSound.length);
        }

        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);

        yield return new WaitForSeconds(blipSound.length);

        entryDoorEvent.Invoke();

        yield return null;
    }

    private IEnumerator Part1GetPortalGunCoroutine()
    {
        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);

        yield return new WaitForSeconds(blipSound.length);

        foreach (AudioClip aiSound in part1GetPortalGunAISoundClip)
        {
            CSoundLoader.Instance.PlaySoundOneShot(aiSound, voiceVolume);

            yield return new WaitForSeconds(aiSound.length);
        }

        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);
    }

    private IEnumerator Part2EntryCoroutine()
    {
        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);

        yield return new WaitForSeconds(blipSound.length);

        CSoundLoader.Instance.PlaySoundOneShot(part2EntryAISoundClip, voiceVolume);

        yield return new WaitForSeconds(part2EntryAISoundClip.length);

        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);

        yield return null; 
    }

    private IEnumerator Part2SuccessCoroutine()
    {
        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);

        yield return new WaitForSeconds(blipSound.length);

        CSoundLoader.Instance.PlaySoundOneShot(part2SuccessAISoundClip, voiceVolume);

        yield return new WaitForSeconds(part2SuccessAISoundClip.length);

        CSoundLoader.Instance.PlaySoundOneShot(blipSound, voiceVolume);

        yield return null;
    }

}
