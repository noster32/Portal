using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class CAmbientSound : CComponent    
{
    [SerializeField] private EventReference ambientEventReference;
 
    private EventInstance ambientEventInstance;

    public override void Start()
    {
        base.Start();

        ambientEventInstance = CAudioManager.Instance.CreateEventInstance(ambientEventReference);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ambientEventInstance.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            ambientEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void ambientPlay() => ambientEventInstance.start();
    public void ambientStop() => ambientEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
}
