using UnityEngine;

public class CPlayerDie : CComponent
{
    [SerializeField] private GameObject deathCamera;
    [SerializeField] private GameObject endingCamera;
    [SerializeField] private GameObject dropPortalGun;
    [SerializeField] private CCameraFade deathFade;

    public void PlayerSpawnDeadCamera()
    {
        var cam = Instantiate(deathCamera, 
                                transform.position + new Vector3(0f, 1.6f, 0f), 
                                Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f));

        //Instantiate(dropPortalGun, 
        //                            (transform.position + new Vector3(0f, 1.6f, 0f)) + transform.forward * 0.1f, 
        //                            Quaternion.identity);
        //포탈건 오브젝트 생성
        cam.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        deathFade.SetAlpha(0.65f);
        cam.gameObject.SetActive(true);
    }

    public void PlayerSpawnDeadCamera2()
    {
        this.gameObject.SetActive(false);
        deathFade.SetAlpha(0.15f);
        endingCamera.gameObject.SetActive(true);
    }
}
