using FMODUnity;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CRadio : CGrabableObject
{
    private StudioEventEmitter emitter;

    public override void Start()
    {
        base.Start();

        emitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.loopingRadioMix, this.gameObject);
        emitter.Play();
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
            return;
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.metalSolidImpactHard, this.transform.position);
    }

    private void OnDisable()
    {
        emitter.Stop();
    }
    public void RadioPlay() => emitter.Play();
    public void RadioStop() => emitter.Stop();
}
