using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CBallLauncher : CComponent
{
    public GameObject BallObj;


    Animation aLauncher;

    public override void Awake()
    {
        base.Awake();

        aLauncher = GetComponent<Animation>();
    }
    public override void Update()
    {
        base.Update();

        BallLaunch();
    }

    void BallLaunch()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject Ball = Instantiate(BallObj, transform.position + transform.forward * 0.7f, transform.rotation);

            float BallSpeed = 10f;

            Vector3 BallLaunch = Ball.transform.TransformDirection(Vector3.forward);

            Ball.GetComponent<Rigidbody>().AddForce(BallSpeed * BallLaunch);

            Destroy(BallObj, 15f);
        }
    }

    //IEnumerator BallLaunchAnim()
    //{
    //    
    //    yield return 
    //}
}
