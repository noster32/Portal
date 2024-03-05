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

    [Header("Sound")]
    [SerializeField] private AudioClip fluorescentHum1Clip;
    [SerializeField] private AudioClip fluorescentHum2Clip;

    private Material[] signageMaterials;
    private Color[] signageColors;
    private AudioSource audioSource;

    private bool isOn;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();

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

        audioSource.clip = fluorescentHum1Clip;
        audioSource.loop = true;
        audioSource.volume = CSoundLoader.Instance.GetEffectVolume(0.03f);

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

        audioSource.Play();
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
        signageMaterials[0].EnableKeyword("_EMISSION");
        signageMaterials[0].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        yield return new WaitForSeconds(0.2f);

        audioSource.Stop();
        signageMaterials[0].mainTexture = turnOnBeforeTexture;
        signageMaterials[0].DisableKeyword("_EMISSION");

        yield return new WaitForSeconds(0.083f);

        audioSource.clip = fluorescentHum2Clip;
        audioSource.Play(); 
        signageMaterials[0].mainTexture = turnOnTexture;
        signageMaterials[0].EnableKeyword("_EMISSION");
        signageMaterials[0].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        yield return new WaitForSeconds(0.216f);

        signageColors[first].a = 1f;
        signageMaterials[first].color = signageColors[first];

        yield return new WaitForSeconds(0.683f);

        signageColors[last].a = 1f;
        signageMaterials[last].color = signageColors[last];
        audioSource.clip = fluorescentHum1Clip;
        audioSource.volume = CSoundLoader.Instance.GetEffectVolume(0.045f);
        audioSource.Play();

        yield return null;
    }
}
