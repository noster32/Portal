using System.Collections;
using UnityEngine;
using FMODUnity;

public class CPlayerMovement : CTeleportObject
{
    [Header("Player Move Setting")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float stepInterval;
    [SerializeField] private float playerHeight = 1.6f;
    [SerializeField] private float playerCrouchHeight = 1f;

    [HideInInspector] public Vector3 moveVector;

    private Vector3 airMoveVector;
    private Vector2 moveInput;
    private float moveVectorMagnitude;
    private float stepCycle;
    private float heightOffset;
    private bool isOnGround = false;
    private bool isJump = false;
    private bool isCrouch = false;
    private bool groundChecking = true;

    private CPlayerState playerState;
    private CapsuleCollider playerCollider;
    private Coroutine lineToPortalCoroutine;

    private ObjectMaterial floorMat;

    public override void Awake()
    {
        base.Awake();

        cameraTransform = Camera.main.transform;
        playerCollider = GetComponent<CapsuleCollider>();
        mouseLook = GetComponent<CPlayerMouseLook>();
        playerState = GetComponent<CPlayerState>();

        grapicsClone = Instantiate(grapicsObject);
        grapicsClone.transform.parent = grapicsObject.transform;

        grapicsClone.transform.GetChild(0).
            Find("root/spine_base/spine_mid/chest/clavicle_R/bicep_R/elbow_R/wrist_R/" +
            "weapon_bone/weapon_bone_end/w_portalgun_p3/default").gameObject.layer
            = LayerMask.NameToLayer("PlayerClone");

        grapicsClone.transform.GetChild(1).gameObject.layer = LayerMask.NameToLayer("PlayerClone");
        grapicsClone.transform.localScale = new Vector3(1f, 1f, 1f);

        GetAnimator(grapicsObject, grapicsClone);

        grapicsClone.SetActive(false);
    }

    public override void Start()
    {
        base.Start();

        mouseLook.Init(cameraTransform, this.transform);
        SetColliderHeightAndCenter(playerHeight);

    }


    public override void Update()
    {
        base.Update();

        if (CGameManager.Instance.GetIsPaused())
            return;

        PlayerRotation();
        GetInput();

        Jump();
        Crouch();

        PlayerMove();
        StepCycle();

        PlayerStateChange();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerState.GetIsPlayerDie())
            return;

        GroundCheck();
    }

    private void GetInput()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        moveVector.x = moveInput.x;
        moveVector.z = moveInput.y;

        moveVectorMagnitude = moveVector.sqrMagnitude;
    }

    //플레이어 조작
    //플레이어 공중에 있을 때는 좌우로만 조금씩 움직일 수 있도록 설정
    public void PlayerMove()
    {
 
        moveVector = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f) * moveVector;
        airMoveVector = transform.right * moveInput.x;

        if (moveVectorMagnitude <= 1)
        {
            moveVector *= (isCrouch ? crouchSpeed : moveSpeed);
        }
        else
        {
            moveVector = moveVector.normalized * (isCrouch ? crouchSpeed : moveSpeed);
        }
        
        if (airMoveVector.sqrMagnitude <= 1)
        {
            airMoveVector *= (isCrouch ? crouchSpeed : moveSpeed);
        }
        else
        {
            airMoveVector = airMoveVector.normalized * (isCrouch ? crouchSpeed : moveSpeed);
        }
        
        if (isOnGround)
        {
            m_oRigidBody.velocity = new Vector3(moveVector.x, m_oRigidBody.velocity.y, moveVector.z);
        }
        else if (!isOnGround)
            m_oRigidBody.AddForce(airMoveVector * 2f);

    }

    private void PlayerRotation()
    {
        mouseLook.MouseRotation(cameraTransform, this.transform);
        mouseLook.RecoilRotation();
    }

    public void LineToPortal(Vector3 position)
    {
        if(!isOnGround)
        {
            lineToPortalCoroutine = StartCoroutine(LineToPortalCoroutine(position, 0.3f));
        }
    }

    public void StopLineToPortal()
    {
        if (lineToPortalCoroutine != null)
            StopCoroutine(lineToPortalCoroutine);
    }

    //플레이어의 위치 조정
    private IEnumerator LineToPortalCoroutine(Vector3 position, float duration)
    {
        float elapsedTime = 0f;
        Vector3 start = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 end = new Vector3(position.x, 0f, position.z);
        m_oRigidBody.velocity = new Vector3(0f, m_oRigidBody.velocity.y, 0f);

        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 temp = Vector3.Lerp(start, end, t);
            transform.position = new Vector3(temp.x, transform.position.y, temp.z);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = new Vector3(end.x, transform.position.y, end.z);

        lineToPortalCoroutine = null;
        yield return null;
    }


    public void Jump()
    {
        if (isOnGround)
        {
            isJump = false;
        }

        if (!isJump && (isOnGround))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJump = true;
                PlayJumpAudio();
                m_oRigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouch = true;
            SetColliderHeightAndCenter(playerHeight, playerCrouchHeight);
        }
        else
        {
            isCrouch = false;
            SetColliderHeightAndCenter(playerHeight);
        }
    }

    //발소리 사이클
    //사이클을 스피드에 따라서 더해준다
    //사이클이 인터벌에 도달하면 사이클을 초기화하고 발소리를 재생한다
    //움직임을 멈출경우에는 움직이자 마자 발소리를 재생하도록 인터벌 바로직전에 사이클을 설정
    public void StepCycle()
    {
        if (moveVector.sqrMagnitude > 0 && (moveInput.x != 0 || moveInput.y != 0))
        {
            stepCycle += (moveSpeed * (isCrouch ? 0.5f : 1f)) * Time.deltaTime;
        }
        else if(moveVector.sqrMagnitude == 0 && moveInput.x == 0 && moveInput.y == 0)
        {
            stepCycle = stepInterval - 1;
        }

        if (stepCycle >= stepInterval)
        {
            stepCycle = 0f;
            PlayFootstepAudio();
        }
    }


    private void PlayFootstepAudio()
    {
        if (!isOnGround || playerState.GetPlayerState() == CPlayerState.PlayerState.JUMP)
        {
            return;
        }

        CAudioManager.Instance.PlayOneShot(GetMaterailFootstepsSound(), this.transform.position);
    }

    private void PlayJumpAudio()
    {
        CAudioManager.Instance.PlayOneShot(GetMaterailFootstepsSound(), this.transform.position);
    }

    private EventReference GetMaterailFootstepsSound()
    {
        EventReference footstepsSound;

        switch (floorMat)
        {
            case ObjectMaterial.CONCRETE:
                footstepsSound = CFMODEvents.Instance.stepSoundConcrete;
                break;
            case ObjectMaterial.METAL:
                footstepsSound = CFMODEvents.Instance.stepSoundMetal;
                break;
            case ObjectMaterial.METALGRATE:
                footstepsSound = CFMODEvents.Instance.stepSoundMetalGrate;
                break;
            default:
                footstepsSound = CFMODEvents.Instance.stepSoundConcrete;
                break;
        }

        return footstepsSound;
    }

    //그라운드 체크
    public void GroundCheck()
    {
        if (!groundChecking)
            return;

        float height = isCrouch ? playerCrouchHeight : playerHeight;
        Vector3 playerCenter = transform.position + new Vector3(0f, playerCollider.center.y , 0f);

        RaycastHit hit;
        int excludedLayers = ~LayerMask.GetMask("Player", "InvisiblePlayer");
        Vector3 groundCheckBoxSize = new Vector3(0.2f, 0.05f, 0.2f);
        float maxDis = (height / 2) + heightOffset + 0.1f;

        if (Physics.BoxCast(playerCenter, groundCheckBoxSize, Vector3.down, out hit, Quaternion.identity,
                            maxDis, excludedLayers, QueryTriggerInteraction.Ignore))
        {
            FloatingPlayer(hit, (height / 2) + heightOffset, playerCenter, maxDis);

            switch (hit.collider.tag)
            {
                case "Concret":
                    floorMat = ObjectMaterial.CONCRETE;
                    break;
                case "Metal":
                    floorMat = ObjectMaterial.METAL;
                    break;
                case "MetalGrate":
                    floorMat = ObjectMaterial.METALGRATE;
                    break;
                default:
                    floorMat = ObjectMaterial.CONCRETE;
                    break;
            }
        }
        else
            isOnGround = false;
    }

    //플레이어 플로팅
    //플레이어가 기준점보다 밑에 있으면 GroundCheck에서 히트된 distance와 target과의 차이를 이용해
    //플레이어를 공중에 띄운다
    private void FloatingPlayer(RaycastHit hit, float targetDistance ,Vector3 center, float maxDistacne)
    {
        if (PortalOnFloorCheck(center, maxDistacne))
            return;

        float distance = targetDistance - hit.distance;
        float forceMultiplier = 25f;
        if (distance >= 0f)
            isOnGround = true;

        float forceValue = distance * forceMultiplier - m_oRigidBody.velocity.y;
        Vector3 floatingForce = new Vector3(0f, forceValue, 0f);
        m_oRigidBody.AddForce(floatingForce, ForceMode.VelocityChange);
    }

    //포탈이 열려있을 경우에 발밑에 포탈이 있다면 true 반환
    private bool PortalOnFloorCheck(Vector3 center, float maxDistacne)
    {
        RaycastHit hit;
        int portalLayer = LayerMask.GetMask("PortalCollider");

        if (!CSceneManager.Instance.portalPair.PlacedBothPortal())
            return false;

        if(Physics.Raycast(center,Vector3.down, out hit, maxDistacne, portalLayer))
        {
            if (System.Math.Sign(Vector3.Dot(hit.normal, Vector3.down)) < 0f)
            {
                isOnGround = false;
                return true;
            }
        }
        
        return false;
    }

    private void PlayerStateChange()
    {
        if (moveVectorMagnitude != 0 && !isJump && !isCrouch)
        {
            playerState.SetPlayerState(CPlayerState.PlayerState.WALK);
        }
        else if (isJump)
        {
            playerState.SetPlayerState(CPlayerState.PlayerState.JUMP);
        }
        else if (isCrouch && !isJump && moveVectorMagnitude == 0)
        {
            playerState.SetPlayerState(CPlayerState.PlayerState.CROUCH);
        }
        else if (isCrouch && !isJump && moveVectorMagnitude != 0)
        {
            playerState.SetPlayerState(CPlayerState.PlayerState.CROUCHWALK);
        }
        else
        {
            playerState.SetPlayerState(CPlayerState.PlayerState.IDLE);
        }
    }

    //플레이어 콜라이더 센터, 길이 재설정
    //플레이어 콜라이더를 높이의 90%로 설정하고 줄어든 높이만큼 위로 올려준다
    private void SetColliderHeightAndCenter(float height)
    {
        playerCollider.height = height * 0.8f;

        float diffHeight = height - playerCollider.height;
        heightOffset = diffHeight / 2;
        playerCollider.center = new Vector3(0f, (height / 2) + heightOffset, 0f);
    }

    //플레이어 콜라이더 센터, 길이 재설정
    //플레이어가 앉아서 콜라이더가 줄어든 경우에도 줄어든 높이만큼 위로 올려준다
    private void SetColliderHeightAndCenter(float standHeight, float crouchHeight)
    {
        float height = (isCrouch ? crouchHeight : standHeight);
        playerCollider.height = height * 0.9f;

        float diffHeight = height - playerCollider.height;
        float diffHeightStandCrouch = standHeight - crouchHeight;
        heightOffset = (diffHeight / 2) + (diffHeightStandCrouch / 2);
        playerCollider.center = new Vector3(0f, (standHeight / 2) + heightOffset, 0f);
    }

    public override void Teleport()
    {
        base.Teleport();

        if (lineToPortalCoroutine != null)
        {
            StopLineToPortal();
        }

        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.teleport, this.transform.position);
    }

    public void SetIsJump(bool val) => isJump = val;
    public void SetGroundChecking(bool val) => groundChecking = val;
}
