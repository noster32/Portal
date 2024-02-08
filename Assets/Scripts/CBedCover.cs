using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBedCover : CComponent
{
    Animator animator;
    private enum CoverState
    {
        IDLE,
        OPEN,
        OPENING,
        CLOSING,
    }

    [SerializeField] private CoverState coverState;

    public override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        transform.position = new Vector3(0.624f, 0.918f, 8.279f);
    }

    public override void Update()
    {
        base.Update();

        if (coverState == CoverState.IDLE)
        {
            transform.position = new Vector3(0.72f, 3.25f, 8.279f);
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            transform.position = new Vector3(0.624f, 0.918f, 8.279f);
            transform.rotation = Quaternion.identity;
        }

        if (coverState == CoverState.IDLE)
            animator.Play("idle");
        else if (coverState == CoverState.OPEN)
            animator.Play("open");
        else if (coverState == CoverState.OPENING)
        {
            animator.Play("opening");
            transform.GetChild(1).rotation = Quaternion.identity;
        }
        else if (coverState == CoverState.CLOSING)
            animator.Play("closing");
    }

    
}
