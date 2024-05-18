using UnityEngine;

public class CPlayerDie : CComponent
{
    [SerializeField] private GameObject deathCamera;
    [SerializeField] private GameObject endingCamera;
    [SerializeField] private CCameraFade deathFade;

    //플레이어를 disable하고 마우스 회전만 가능한 카메라를 스폰한다
    public void PlayerSpawnDeadCamera()
    {
        var cam = Instantiate(deathCamera, 
                                transform.position + new Vector3(0f, 1.6f, 0f), 
                                Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f));

        cam.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        deathFade.SetAlpha(0.65f);
        cam.gameObject.SetActive(true);
    }

    //엔딩용 카메라 스폰
    public void PlayerSpawnDeadCamera2()
    {
        this.gameObject.SetActive(false);
        deathFade.SetAlpha(0.15f);
        endingCamera.gameObject.SetActive(true);
    }
}
