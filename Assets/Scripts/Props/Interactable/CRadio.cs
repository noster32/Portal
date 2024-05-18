using FMODUnity;
using UnityEngine;

public class CRadio : CGrabbableObject
{
    private StudioEventEmitter emitter;

    public override void Start()
    {
        base.Start();

        emitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.loopingRadioMix, this.gameObject);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
            return;
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.metalSolidImpactHard, this.transform.position);
    }

    public void RadioPlay() => emitter.Play();
    public void RadioStop() => emitter.Stop();
}
