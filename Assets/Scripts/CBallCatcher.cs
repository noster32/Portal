using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CBallCatcher : CComponent
{
    private Animation catcherAnimaton;

    [SerializeField] private UnityEvent catchEvent;
    [SerializeField] private AudioClip ballCatchClip;
    private AudioSource audioSource;
    private bool comBallConnected;
    public override void Awake()
    {
        base.Awake();

        catcherAnimaton = GetComponent<Animation>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();

        audioSource.clip = ballCatchClip;
    }

    private void OnTriggerEnter(Collider other)
    {
        string objectTag = other.gameObject.tag;
        if (objectTag == "ComBall")
        {
            CBall comBall = other.GetComponent<CBall>();
            if(comBall)
            {
                comBallConnected = true;
                audioSource.Play();
                comBall.StopAlphaCoroutine();
                catcherAnimaton.Play("close");

                if (catchEvent != null)
                    catchEvent.Invoke();
            }
        }
    }

    public bool GetcomBallConnected()
    {
        return comBallConnected;
    }
}
