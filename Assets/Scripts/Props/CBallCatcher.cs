using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CBallCatcher : CComponent
{
    private Animator catchAnimator;

    [SerializeField] private UnityEvent catchEvent;
    private bool ballConnected;

    public override void Awake()
    {
        base.Awake();

        catchAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(ballConnected)
        {
            return;
        }

        string objectTag = other.gameObject.tag;
        if (objectTag == "CombineBall")
        {
            CBall ball = other.GetComponent<CBall>();
            if(ball)
            {
                ballConnected = true;
                CAudioManager.Instance.PlayOneShot(CFMODEventsEnergy.Instance.weld2, this.transform.position);
                ball.StopAlphaCoroutine();
                catchAnimator.Play("close");

                if (catchEvent != null)
                    catchEvent.Invoke();
            }
        }
    }

    public bool GetBallConnected()
    {
        return ballConnected;
    }
}
