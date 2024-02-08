using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CPlayerMouseLook : CComponent
{
    private Quaternion m_playerCharacterRot;
    private Quaternion m_cameraRot;
    private bool m_cursorIsLocked = true;

    public void Init(Transform character, Transform camera)
    {
       m_playerCharacterRot = character.localRotation;
       m_cameraRot = camera.localRotation;
    }

    public void MouseRotation(Transform character, Transform camera)
    {
        float yRot = Input.GetAxis("Mouse X");
        float xRot = Input.GetAxis("Mouse Y");

        m_playerCharacterRot *= Quaternion.Euler(0f, yRot, 0f);
        m_cameraRot *= Quaternion.Euler(-xRot, 0f, 0f);
        m_cameraRot = ClampRotationXAxis(m_cameraRot);

        if (m_cameraRot.z != 0f)
        {
            m_cameraRot = Quaternion.Slerp(m_cameraRot, 
                                           Quaternion.Euler(m_cameraRot.eulerAngles.x, m_cameraRot.eulerAngles.y, 0f),
                                           5.0f * Time.deltaTime);
        }

        character.localRotation = Quaternion.Slerp(character.localRotation, m_playerCharacterRot, 10.0f * Time.deltaTime);
        camera.localRotation = m_cameraRot;
    }

    public void TeleportCameraRotation(Quaternion rot)
    {
        m_playerCharacterRot = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);
        m_cameraRot = Quaternion.Euler(rot.eulerAngles.x, 0f, rot.eulerAngles.z);
    }

    public Quaternion GetPlayerRot()
    {
        return m_playerCharacterRot;
    }

    Quaternion ClampRotationXAxis(Quaternion q)
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
}
