using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAmbienceSound : CComponent
{
    [SerializeField] private Collider area;
    [SerializeField] private GameObject target;
    [SerializeField] private float volume = 0.1f;
    private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    public override void Update()
    {
        base.Update();

        audioSource.volume = CSoundLoader.Instance.GetEffectVolume(volume);

        Vector3 closestPoint = area.ClosestPoint(target.transform.position + (target.transform.forward * 0.1f));

        if(Vector3.Distance(target.transform.position, closestPoint) <= (audioSource.maxDistance + 0.5f))
        {
            transform.position = closestPoint;
        }
    }

    public void ChangeAmbient(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
