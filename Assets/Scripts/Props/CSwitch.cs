using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CSwitch : CComponent
{
    [SerializeField] private CInteractObject interaction;
    [SerializeField] private UnityEvent switchEventPress;
    [SerializeField] private UnityEvent switchEventReturn;
    [SerializeField] private bool useTimer = true;
    [SerializeField] private int waitTime = 3;
    
    private bool isActive;
    private Animator animator;

    public override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
       
    }

    private void OnEnable()
    {
        if (interaction)
        {
            interaction.GetInteractEvent.HasInteracted += StartPressSwitch;
        }
    }

    private void OnDisable()
    {
        if (interaction)
        {
            interaction.GetInteractEvent.HasInteracted -= StartPressSwitch;
        }
    }

    private void StartPressSwitch()
    {
        if (!isActive)
        {
            isActive = true;

            if (useTimer)
                StartCoroutine(PressSwitchTimerCoroutine());
            else
                StartCoroutine(PressSwitchCoroutine());

            StartCoroutine(SwitchAnimationCoroutine());
        }
    }

    private IEnumerator PressSwitchTimerCoroutine()
    {
        switchEventPress.Invoke();
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.button3, this.transform.position);

        int duration = waitTime;


        for (int i = 0; i < duration; ++i)
        {
            yield return new WaitForSeconds(1f);
        
            CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.ticktock1, this.transform.position);
        }
        


        //ticktockSoundLength = 0.7f
        yield return new WaitForSeconds(0.7f);

        switchEventReturn.Invoke();
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.button10, this.transform.position);

        isActive = false;
        yield return null;
    }

    private IEnumerator PressSwitchCoroutine()
    {
        switchEventPress.Invoke();
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.button3, this.transform.position);

        isActive = false;
        yield return null;
    }

    private IEnumerator SwitchAnimationCoroutine()
    {
        animator.Play("down");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);

        animator.Play("up");

        yield return null;
    }
}
