using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalBullet : CComponent
{
    #region private
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float maxDistance = 500f;

    #endregion

    #region public
    public bool isHit = false;
    #endregion

    public override void Update()
    {
        base.Update();

        float moveDelta = this.bulletSpeed * Time.deltaTime;

        this.transform.Translate(0f, 0f, moveDelta);

        this.maxDistance -= moveDelta;

        if(this.maxDistance < 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.transform.CompareTag("Player"))
        {
            isHit = true;
            Destroy(this.gameObject);
        }
    }

    public void DeleteBullet()
    {
        Destroy(this.gameObject);
    }
}
