using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CPedestal : CComponent
{
    [Header("Setting")]
    [SerializeField][Range(0, 3)] private int startAutoPortalNum;
    [SerializeField] UnityEvent pedestalOpenAreaEvent;

    [Header("Auto Portal")]
    [SerializeField] private CAutoPortal[] autoPortals;
    [SerializeField] private CAutoPortal autoPortalOther;


    [Header("GameObject")]
    [SerializeField] private GameObject portalGunObject;
    [SerializeField] private GameObject portalGunBullet;
    [SerializeField] private GameObject audioObject;
    [SerializeField] private Transform pedestalCenterTransform;
    [SerializeField] private Transform portalGunMuzzleTransform;

    [Header("Animator")]
    [SerializeField] Animator pedestalBaseAnimator;
    [SerializeField] Animator pedestalCenterAnimator;
    private Coroutine pedestalCoroutine;
    private Coroutine rotationCoroutine;

    private bool m_isOpenGlass;
    private StudioEventEmitter emitter;


    public override void Start()
    {
        base.Start();

        emitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.portalgunRotateLoop, audioObject);
        pedestalCenterAnimator.Play("close", 0, 1f);
        pedestalCenterTransform.rotation = Quaternion.Euler(0f, 90f * (startAutoPortalNum + 1), 0f);
        pedestalCoroutine = StartCoroutine(PortalAutoFireCoroutine(startAutoPortalNum));
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

        if (emitter.IsPlaying())
            emitter.Stop();

        portalGunObject.SetActive(false);
        autoPortals[0].AutoRemovePortal();

        StartCoroutine(PedestalMoveDownCoroutine(4f));
    }

    private IEnumerator PortalAutoFireCoroutine(int startNum)
    {
        if(autoPortals[startNum + 1])
            autoPortals[startNum + 1].AutoPlacePortal();
        if(autoPortalOther)
            autoPortalOther.AutoPlacePortal();

        int count = startNum;
        bool placedAutoPortal = true;
        while (true)
        {
            if (!autoPortals[count])
            {
                placedAutoPortal = false;
            }
            else if (autoPortals[count])
                placedAutoPortal = true;

            rotationCoroutine = StartCoroutine(PedestalRotationCoroutine(3f, count));
            if (!emitter.IsPlaying())
                emitter.Play();
            
            yield return new WaitForSeconds(3f);

            if (emitter.IsPlaying())
                emitter.Stop();

            // shoot Portal Bullet
            if(placedAutoPortal)
            {
                CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalCharging, audioObject.transform.position);

                yield return new WaitForSeconds(0.833f);

                Instantiate(portalGunBullet, portalGunMuzzleTransform.position, pedestalCenterTransform.rotation);
                CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.portalgunShootBlue, audioObject.transform.position);


                yield return new WaitForSeconds(0.167f);

                autoPortals[count].AutoPlacePortal();


                yield return new WaitForSeconds(3f);
            }
            else
                yield return new WaitForSeconds(1f);

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
        Quaternion start = pedestalCenterTransform.rotation;
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
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.lever2, audioObject.transform.position);
            if (pedestalOpenAreaEvent != null)
                pedestalOpenAreaEvent.Invoke();
        }
    }
}
