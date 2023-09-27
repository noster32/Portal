using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CBullet : CComponent
{
    
    #region public

    public float speed = 100.0f;
    public float distance = 500.0f;

    #endregion

    #region private
    [SerializeField]
    private TrailRenderer bulletTrail;
    #endregion


    public override void Start()
    {
        base.Start();

    }


    public override void Update()
    {
        base.Update();

        float moveDelta = this.speed * Time.deltaTime;

        this.transform.Translate(0, 0, moveDelta);

        this.distance -= moveDelta;

        if(this.distance < 0 )
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.CompareTag("Concret"))
            Destroy(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    string otherTag = other.tag;
    //
    //
    //    if(otherTag != "Turret")
    //    {
    //        Debug.Log("Collision!");
    //        Destroy(this.gameObject);
    //    }
    //}


}
