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
    private bool isOnGround = false;
    private bool isJump = false;
    private bool isCrouch = false;
    private CPlayerState playerState;
    private CapsuleCollider playerCollider;
    private Coroutine lineToPortalCoroutine;

    private enum FloorMaterial
    { 
        CONCRET,
        METAL,
        METALGRATE
    }

    private FloorMaterial floorMat;

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
            Find("root/spine_base/spine_mid/chest/clavicle_R/bicep_R/elbow_R/wrist_R/weapon_bone/weapon_bone_end/w_portalgun_p3/default").gameObject.layer
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
        StepCycle();
        PlayerMove();


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
    }
    private void PlayerMove()
    {
        moveVectorMagnitude = moveVector.sqrMagnitude;
 
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
            m_oRigidBody.AddForce(airMoveVector * 5f);

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
            lineToPortalCoroutine = StartCoroutine(LineToPortalCoroutine(position));
        }
    }

    public void StopLineToPortal()
    {
        if (lineToPortalCoroutine != null)
            StopCoroutine(lineToPortalCoroutine);
    }

    private IEnumerator LineToPortalCoroutine(Vector3 position)
    {
        float elapsedTime = 0f;
        Vector3 start = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 end = new Vector3(position.x, 0f, position.z);
        m_oRigidBody.velocity = new Vector3(0f, m_oRigidBody.velocity.y, 0f);
        while(elapsedTime < 0.5f)
        {
            float t = elapsedTime / 0.3f;
            Vector3 temp = Vector3.Lerp(start, end, t);
            transform.position = new Vector3(temp.x, transform.position.y, temp.z);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = new Vector3(end.x, transform.position.y, end.z);

        lineToPortalCoroutine = null;
        yield return null;
    }


    private void Jump()
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

    private void StepCycle()
    {
        if (moveVector.sqrMagnitude > 0 && (moveInput.x != 0 || moveInput.y != 0))
        {
            stepCycle += (moveVector.magnitude + (moveSpeed * (isCrouch ? 0.5f : 1f))) * Time.deltaTime;
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
        if (!isOnGround || isJump)
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
            case FloorMaterial.CONCRET:
                footstepsSound = CFMODEvents.Instance.stepSoundConcret;
                break;
            case FloorMaterial.METAL:
                footstepsSound = CFMODEvents.Instance.stepSoundMetal;
                break;
            case FloorMaterial.METALGRATE:
                footstepsSound = CFMODEvents.Instance.stepSoundMetalGrate;
                break;
            default:
                footstepsSound = CFMODEvents.Instance.stepSoundConcret;
                break;
        }

        return footstepsSound;
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        float height = isCrouch ? playerCrouchHeight : playerHeight;

        Vector3 playerCenter = transform.position + new Vector3(0f, playerCollider.center.y , 0f);
        Vector3 groundCheckBox = new Vector3(0.2f, 0.05f, 0.2f);
        float maxDis = height / 2 + 0.1f;

        int excludedLayers = ~LayerMask.GetMask("Player", "InvisiblePlayer");

        if (Physics.BoxCast(playerCenter, groundCheckBox, Vector3.down, out hit, Quaternion.identity, maxDis, excludedLayers, QueryTriggerInteraction.Ignore))
        {
            FloatingPlayer(hit, height, playerCenter, maxDis);

            if (hit.collider.tag == "Concret")
            {
                floorMat = FloorMaterial.CONCRET;
            }
            else if (hit.collider.tag == "Metal")
            {
                floorMat = FloorMaterial.METAL;
            }
            else if (hit.collider.tag == "MetalGrate")
            {
                floorMat = FloorMaterial.METALGRATE;
            }
            else
                floorMat = FloorMaterial.CONCRET;
        }
        else
            isOnGround = false;
    }

    private void FloatingPlayer(RaycastHit hit, float height ,Vector3 center, float maxDistacne)
    {
        if (PortalOnFloorCheck(center, maxDistacne))
        {
            return;
        }

        float distance = height / 2 - hit.distance;
        
        if(distance >= 0f)
        {
            isOnGround = true;
        }

        //이거 50f로 바꿨더니 왜 되는거임??
        float forceValue = distance * 50f - m_oRigidBody.velocity.y;
        Vector3 floatingForce = new Vector3(0f, forceValue, 0f);
        m_oRigidBody.AddForce(floatingForce, ForceMode.VelocityChange);
    }

    private bool PortalOnFloorCheck(Vector3 center, float maxDistacne)
    {
        RaycastHit hit;
        int portalLayer = LayerMask.GetMask("PortalCollider");

        if (!CSceneManager.Instance.portalPair.PlacedBothPortal())
        {
            return false;
        }

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

    private void OnDrawGizmos()
    {
        RaycastHit hit2;
        if(playerCollider != null)
        {
            Vector3 playerCenter = transform.position + new Vector3(0f, playerCollider.height / 2, 0f);

            Vector3 groundCheckBox = new Vector3(0.6f, 0.05f, 0.6f);

            int excludedLayers = ~LayerMask.GetMask("Player", "InvisiblePlayer");

            Gizmos.color = Color.red;

            if (Physics.BoxCast(playerCenter, groundCheckBox, Vector3.down, out hit2, Quaternion.identity, 0.81f, excludedLayers))
            {
                Gizmos.DrawRay(playerCenter, Vector3.down * hit2.distance);
                Gizmos.DrawWireCube(playerCenter + Vector3.down * hit2.distance, groundCheckBox);
            }
        }
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

    private void SetColliderHeightAndCenter(float height)
    {
        playerCollider.height = height * 0.8f;

        float diffHeight = height - playerCollider.height;
        playerCollider.center = new Vector3(0f, (height / 2) + (diffHeight / 2), 0f);
    }

    private void SetColliderHeightAndCenter(float standHeight, float crouchHeight)
    {
        float height = (isCrouch ? crouchHeight : standHeight);
        playerCollider.height = height * 0.9f;

        float diffHeight = height - playerCollider.height;
        float diffHeightStandCrouch = standHeight - crouchHeight;

        playerCollider.center = new Vector3(0f, (standHeight / 2) + (diffHeight / 2) + (diffHeightStandCrouch / 2), 0f);
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
    
    public void DiePlayerMove()
    {
        playerCollider.height = 0.6f;
    }
}
