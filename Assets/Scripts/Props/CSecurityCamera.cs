using UnityEditor.Timeline;
using UnityEngine;

public class CSecurityCamera : CComponent
{

    [Header("Setting")]
    [SerializeField] float rotationSpeedX = 0.5f;
    [SerializeField] float rotationSpeedY = 0.5f;

    [Header("Transform")]
    [SerializeField] private Transform basisTransform;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Transform cameraLeftRightTransform;
    [SerializeField] private Transform cameraUpDownTransform;

    [Header("Sound")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSoundClip;

    private Quaternion m_cameraRotationXZ;
    private Quaternion m_cameraRotationZY;
    private Quaternion m_lastCameraRotationXZ;
    private Quaternion m_lastCameraRotationZY;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = moveSoundClip;
    }

    public override void Update()
    {
        base.Update();

        if(playerTransform == null)
        {
            Debug.LogError("Security Camera doesn't set Player");
            return;
        }

        audioSource.volume = CSoundLoader.Instance.GetEffectVolume(0.05f);

        Vector3 directionToTarget = ((playerTransform.position + new Vector3(0f, 1.6f, 0f)) - basisTransform.position).normalized;
        int dotValue = System.Math.Sign(Vector3.Dot(basisTransform.forward, directionToTarget));
        
        if(dotValue > 0)
        {
            Vector3 directionXZ = new Vector3(directionToTarget.x, 0f ,directionToTarget.z);
            Vector2 directionZY = new Vector2(directionToTarget.z, directionToTarget.y);

            float m_angleXZ = Vector3.Angle(basisTransform.right, directionXZ);
            float m_angleZY = Vector2.Angle(basisTransform.up, directionZY);

            m_angleXZ *= dotValue;
            m_angleZY *= dotValue;

            if (m_angleXZ >= 20f && m_angleXZ <= 160f)
            {
                m_cameraRotationXZ = Quaternion.Euler(0f, 270f - m_angleXZ, 0f);

                if (m_angleZY >= 70f && m_angleZY <= 180f)
                {
                    m_cameraRotationZY = Quaternion.Euler(270f - m_angleZY, 0f, 0f);
                }
            }


            CameraRotationChange(m_cameraRotationXZ, m_cameraRotationZY);

            bool rotationChangedXZ = m_lastCameraRotationXZ != cameraLeftRightTransform.localRotation;
            bool rotationChangedZY = m_lastCameraRotationZY != cameraUpDownTransform.localRotation;

            if (rotationChangedXZ || rotationChangedZY)
            {
                if(!audioSource.isPlaying)
                    audioSource.Play();
            }
            else
            {
                if(audioSource.isPlaying)
                    audioSource.Stop();
            }

            m_lastCameraRotationXZ = cameraLeftRightTransform.localRotation;
            m_lastCameraRotationZY = cameraUpDownTransform.localRotation;

        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void CameraRotationChange(Quaternion targetAngleLR, Quaternion targetAngleUD)
    {
        cameraLeftRightTransform.localRotation = Quaternion.RotateTowards(
            cameraLeftRightTransform.localRotation,
            targetAngleLR,
            rotationSpeedX);


        cameraUpDownTransform.localRotation = Quaternion.RotateTowards(
            cameraUpDownTransform.localRotation,
            targetAngleUD,
            rotationSpeedY);
    }
}
