using FMODUnity;
using UnityEngine;

public class CSecurityCamera : CComponent
{

    [Header("Setting")]
    [SerializeField] float volume = 0.01f;
    [SerializeField] float rotationSpeedX = 0.5f;
    [SerializeField] float rotationSpeedY = 0.5f;

    [Header("Transform")]
    [SerializeField] private Transform basisTransform;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Transform cameraLeftRightTransform;
    [SerializeField] private Transform cameraUpDownTransform;


    private Quaternion m_cameraRotationXZ;
    private Quaternion m_cameraRotationZY;
    private Quaternion m_lastCameraRotationXZ;
    private Quaternion m_lastCameraRotationZY;

    private StudioEventEmitter emitter;

    public override void Start()
    {
        base.Start();

        if(playerTransform == null)
            playerTransform = CSceneManager.Instance.player.transform;

        //emitter의 EventInstance.setVolume(value)는 emitter가 play중일때 만 설정 가능
        emitter = CAudioManager.Instance.InitializeEventEmitter(CFMODEvents.Instance.portalgunRotateLoop, basisTransform.gameObject);
    }

    public override void Update()
    {
        base.Update();

        if(playerTransform == null)
            return;

        Vector3 directionToTarget = ((playerTransform.position + new Vector3(0f, 1.6f, 0f)) - basisTransform.position).normalized;
        int dotValue = System.Math.Sign(Vector3.Dot(basisTransform.forward, directionToTarget));
        
        if(dotValue > 0)
        {
            Vector3 directionXZ = new Vector3(directionToTarget.x, 0f ,directionToTarget.z);

            float m_angleXZ = Vector3.Angle(basisTransform.right, directionXZ);
            float m_angleZY = Vector3.Angle(basisTransform.up, directionToTarget);

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
                if (!emitter.IsPlaying())
                {
                    emitter.Play();
                    emitter.EventInstance.setVolume(volume);
                }
            }
            else
            {
                if(emitter.IsPlaying())
                    emitter.Stop();
            }

            m_lastCameraRotationXZ = cameraLeftRightTransform.localRotation;
            m_lastCameraRotationZY = cameraUpDownTransform.localRotation;

        }
        else
        {
            if (emitter.IsPlaying())
                emitter.Stop();
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
