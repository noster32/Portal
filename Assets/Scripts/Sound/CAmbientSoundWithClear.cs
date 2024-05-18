using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAmbientSoundWithClear : CComponent
{
    [SerializeField] private EventReference ambientEventReference;
    [SerializeField] string parameterName;

    private EventInstance ambientEventInstance;

    private enum SceneState
    {
        DEFAULT = 0,
        CLEAR = 1
    }

    [SerializeField] private SceneState sceneState;

    public override void Awake()
    {
        base.Awake();

        sceneState = SceneState.DEFAULT;
    }

    public override void Start()
    {
        base.Start();

        ambientEventInstance = CAudioManager.Instance.CreateEventInstance(ambientEventReference);
        ambientEventInstance.setParameterByName(parameterName, (float)sceneState);
    }

    public void PartClear()
    {
        if(sceneState == SceneState.DEFAULT)
        {
            sceneState = SceneState.CLEAR;
            ambientEventInstance.setParameterByName(parameterName, (float)sceneState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ambientEventInstance.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ambientEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
