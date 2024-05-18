using System.Collections;
using UnityEngine;

public class CPlayerMouseLook : CComponent
{
    private Quaternion m_playerCharacterRot;
    private Quaternion m_cameraRot;
    private Coroutine m_cameraCoroutine;

    //recoil
    private Vector3 recoilRotation;
    private Vector3 currentRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;


    public void Init(Transform camera)
    {
        m_cameraRot = camera.localRotation;
    }

    public void Init(Transform camera, Transform character)
    {
       m_playerCharacterRot = character.rotation;
       m_cameraRot = camera.localRotation;
    }

    public void MouseRotation(Transform camera)
    {
        float yRot = Input.GetAxis("Mouse X");
        float xRot = Input.GetAxis("Mouse Y");

        m_cameraRot *= Quaternion.Euler(-xRot, yRot, 0f);

        camera.localRotation = Quaternion.Euler(m_cameraRot.eulerAngles.x, m_cameraRot.eulerAngles.y, 0f);
    }

    //플레이어 화면 회전
    public void MouseRotation(Transform camera, Transform character)
    {
        if (CGameManager.Instance.GetIsPaused())
            return;

        float yRot = Input.GetAxis("Mouse X");
        float xRot = Input.GetAxis("Mouse Y");

        m_playerCharacterRot *= Quaternion.Euler(0f, yRot, 0f);
        m_cameraRot *= Quaternion.Euler(-xRot, 0f, 0f);
        m_cameraRot = ClampRotationXAxis(m_cameraRot);

        if (m_cameraRot.z != 0f || m_cameraRot.y != 0f)
        {
            if(m_cameraCoroutine == null)
                m_cameraCoroutine = StartCoroutine(CameraRotationOriginal(3f));
        }

        character.rotation = m_playerCharacterRot;
        camera.localRotation = m_cameraRot * Quaternion.Euler(currentRotation);
    }

    public void RecoilRotation()
    {
        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, recoilRotation, snappiness * Time.fixedDeltaTime);
    }
    
    public void FireRecoil()
    {
        recoilRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), 0f);
    }

    public void AimPunch(float punchX)
    {
        recoilRotation = new Vector3(punchX, 0f, 0f);
    }

    public void TeleportCameraRotation(Quaternion rot)
    {
        m_playerCharacterRot = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);
        m_cameraRot = Quaternion.Euler(rot.eulerAngles.x, 0f, rot.eulerAngles.z);
    }

    public void SetCameraRotation(Quaternion rot)
    {
        m_playerCharacterRot = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);
        m_cameraRot = Quaternion.Euler(rot.eulerAngles.x, 0f, 0f);
    }

    public Quaternion GetPlayerRot()
    {
        return m_playerCharacterRot;
    }

    //카메라 최대 회전 각도 설정
    //쿼터니언의 x값을 각도로 바꾸기 위해 angleX를 구하고
    //Clamp한 뒤에 다시 쿼터니언으로 변환해서 적용시킨다
    private Quaternion ClampRotationXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -90f, 90f);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    //카메라 원상 복귀
    private IEnumerator CameraRotationOriginal(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            m_cameraRot = Quaternion.Slerp(m_cameraRot, Quaternion.Euler(m_cameraRot.eulerAngles.x, 0f, 0f), t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        m_cameraRot = Quaternion.Euler(m_cameraRot.eulerAngles.x, 0f, 0f);
        m_cameraCoroutine = null;
        yield return null;
    }
}
