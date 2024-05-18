using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLabSignage : CComponent
{
    [Header("Setting")]
    [SerializeField] private int firstNum;
    [SerializeField] private int lastNum;

    [Header("Texture")]
    [SerializeField] private Texture turnOffTexture;
    [SerializeField] private Texture turnOnBeforeTexture;
    [SerializeField] private Texture turnOnTexture;

    [Header("Renderer")]
    [SerializeField] private Renderer signageRenderer;

    private Material[] signageMaterials;
    private Color[] signageColors;
    private StudioEventEmitter emitter;

    private bool isOn;

    public override void Awake()
    {
        base.Awake();

        signageMaterials = new Material[signageRenderer.materials.Length];
        signageColors = new Color[signageRenderer.materials.Length];

        signageMaterials = signageRenderer.materials;

        for (int i = 0; i < signageMaterials.Length; i++)
        {
            signageColors[i] = signageMaterials[i].color;
        }
    }

    public override void Start()
    {
        base.Start();

        emitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.fluorescentHum, this.gameObject);

        for (int i = 1; i < signageMaterials.Length; i++)
        {
            signageColors[i].a = 0f;
            signageMaterials[i].color = signageColors[i];
        }
    }

    public void StartSignageTurnOn()
    {
        if(!isOn)
        {
            isOn = true;
            StartCoroutine(SignageTurnOn(firstNum, lastNum));
        }
    }

    private IEnumerator SignageTurnOn(int first, int last)
    {
        yield return new WaitForSeconds(1f);

        emitter.Play();
        emitter.EventInstance.setParameterByName("signage_intensity", 0);
        signageMaterials[0].mainTexture = turnOnBeforeTexture;

        yield return new WaitForSeconds(0.083f);

        for(int i = 1; i < signageMaterials.Length; i++)
        {
            if(i != first && i != last)
            {
                signageColors[i].a = 1f;
                signageMaterials[i].color = signageColors[i];
            }
        }

        yield return new WaitForSeconds(0.253f);

        signageMaterials[0].mainTexture = turnOnTexture;

        yield return new WaitForSeconds(0.2f);

        emitter.Stop();

        yield return new WaitForSeconds(0.083f);

        emitter.Play();
        signageMaterials[0].mainTexture = turnOnTexture;

        yield return new WaitForSeconds(0.216f);

        signageColors[first].a = 1f;
        signageMaterials[first].color = signageColors[first];

        yield return new WaitForSeconds(0.683f);

        signageColors[last].a = 1f;
        signageMaterials[last].color = signageColors[last];
        emitter.EventInstance.setParameterByName("signage_intensity", 1);

        yield return null;
    }
}
