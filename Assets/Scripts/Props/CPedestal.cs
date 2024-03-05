using System.Collections;
using UnityEngine;

public class CPedestal : CComponent
{
    [Header("Auto Portal")]
    [SerializeField] private CAutoPortal[] autoPortals;
    [SerializeField] private CAutoPortal autoOrangePortal;


    [Header("GameObject")]
    [SerializeField] private GameObject portalGunObject;
    [SerializeField] private GameObject portalGunBullet;
    [SerializeField] private Transform pedestalCenterTransform;
    [SerializeField] private Transform portalGunMuzzleTransform;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pedestalRotationSoundClip;
    [SerializeField] private AudioClip pedestalGlassOpenSoundClip;
    [SerializeField] private AudioClip portalGunChargingSoundClip;
    [SerializeField] private AudioClip portalGunShootSoundClip;

    [Header("Animator")]
    [SerializeField] Animator pedestalBaseAnimator;
    [SerializeField] Animator pedestalCenterAnimator;
    private Coroutine pedestalCoroutine;
    private Coroutine rotationCoroutine;

    private bool m_isOpenGlass;

    public override void Awake()
    {
        base.Awake();

        audioSource.loop = true;
        audioSource.clip = pedestalRotationSoundClip;
        
    }

    public override void Start()
    {
        base.Start();

        audioSource.volume = CSoundLoader.Instance.GetEffectVolume(0.5f);
        pedestalCenterAnimator.Play("close", 0, 1f);

        pedestalCoroutine = StartCoroutine(PortalAutoFireCoroutine());
    }

    public override void Update()
    {
        base.Update();

        audioSource.volume = CSoundLoader.Instance.GetEffectVolume(0.5f);
    }

    public void StopPedestal()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }
        if (pedestalCoroutine != null)
        {
            StopCoroutine(pedestalCoroutine);
        }

        if (audioSource.isPlaying)
            audioSource.Stop();

        portalGunObject.SetActive(false);
        autoPortals[0].AutoRemovePortal();

        StartCoroutine(PedestalMoveDownCoroutine(4f));
    }

    private IEnumerator PortalAutoFireCoroutine()
    {
        autoPortals[1].AutoPlacePortal();
        autoOrangePortal.AutoPlacePortal();

        int count = 0;

        while (true)
        {
            rotationCoroutine = StartCoroutine(PedestalRotationCoroutine(3f, count));
            if(!audioSource.isPlaying)
                audioSource.Play();
            
            yield return new WaitForSeconds(3f);

            if(audioSource.isPlaying)
                audioSource.Stop();
            audioSource.PlayOneShot(portalGunChargingSoundClip, CSoundLoader.Instance.GetEffectVolume(0.8f));

            yield return new WaitForSeconds(0.833f);

            // shoot Portal Bullet
            Instantiate(portalGunBullet, portalGunMuzzleTransform.position, pedestalCenterTransform.rotation);
            audioSource.PlayOneShot(portalGunShootSoundClip, CSoundLoader.Instance.GetEffectVolume(0.8f));

            yield return new WaitForSeconds(0.167f);

            //Place Portal
            autoPortals[count].AutoPlacePortal(); 

            yield return new WaitForSeconds(3f);

            if (count == 0)
                count = 3;
            else
                count--;
        }
    }

    private IEnumerator PedestalRotationCoroutine(float duration, int count)
    {
        float elapsedTime = 0f;
        float changeAngle = 90f;
        float speed = changeAngle / duration;

        while (elapsedTime < duration)
        {
            pedestalCenterTransform.rotation = Quaternion.RotateTowards(
            pedestalCenterTransform.rotation,
            Quaternion.Euler(0f, count * changeAngle, 0f),
            speed * Time.deltaTime);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        pedestalCenterTransform.rotation = Quaternion.Euler(0f, count * 90f, 0f);
        rotationCoroutine = null;
    }

    private IEnumerator PedestalMoveDownCoroutine(float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPoint = pedestalCenterTransform.localPosition;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            pedestalCenterTransform.localPosition = Vector3.Lerp(startPoint, new Vector3(0f, -2f, 0f), t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        pedestalCenterTransform.localPosition = new Vector3(0f, -2f, 0f);
        pedestalBaseAnimator.Play("close");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_isOpenGlass && other.tag == "Player")
        {
            m_isOpenGlass = true;
            pedestalCenterAnimator.Play("open");
            audioSource.PlayOneShot(pedestalGlassOpenSoundClip, CSoundLoader.Instance.GetEffectVolume(0.8f));
        }
    }
}
