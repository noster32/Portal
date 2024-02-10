using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor.Timeline;
using UnityEngine;

public class CPlayerMovement : CTeleportObject
{
    #region Move
    [Header("Player Move Setting")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float groundDrag;
    [SerializeField] private float stepInterval;

    [HideInInspector] public Vector3 moveVector;

    private Vector3 airMoveVector;
    private Vector2 moveInput;
    private float moveVectorMagnitude;
    private float stepCycle;
    private int stepNum;
    #endregion

    #region State
    private bool isOnGround = false;
    private bool isJump = false;
    private bool isCrouch = false;

    [SerializeField] private float playerHeight = 1.6f;
    [SerializeField] private float playerCrouchHeight = 1f;
    #endregion

    #region Collider
    private CapsuleCollider playerCollider;
    #endregion

    #region Sound
    [Header("Sound")]
    [SerializeField] AudioClip[] footStepSound_concret;
    [SerializeField] AudioClip[] footStepSound_metal;
    [SerializeField] AudioClip jumpSound_concret;
    [SerializeField] AudioClip jumpSound_metal;

    private AudioSource audioSource;
    #endregion

    [SerializeField] private CPortalPair portalPair;

    public enum PlayerState
    {
        IDLE,
        WALK,
        JUMP,
        CROUCH,
        CROUCHWALK,
        FALL,
        DIE
    }

    private enum FloorMaterial
    { 
        CONCRET,
        METAL
    }

    private FloorMaterial floorMat;

    [HideInInspector] public PlayerState pState;

    public override void Awake()
    {
        base.Awake();

    }
    public override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<CapsuleCollider>();

        mouseLook.Init(transform, cameraTransform);
        SetColliderHeightAndCenter(1.8f);
    }

    public override void Update()
    {
        base.Update();

        if (pState == PlayerState.DIE)
            return;

        GetInput();
        PlayerRotation();
        Jump();
        Crouch();
        StepCycle();
        PlayerMove();

        PlayerStateChange();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (pState == PlayerState.DIE)
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

        moveVector = transform.TransformDirection(moveVector);
        airMoveVector = transform.forward * (moveInput.y <= 0 ? moveInput.y : 0f) + transform.right * moveInput.x;

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
            objRigidbody.velocity = new Vector3(moveVector.x, objRigidbody.velocity.y, moveVector.z);
        else if (!isOnGround)
            objRigidbody.AddForce(airMoveVector * 100f);
    }

    private void PlayerRotation()
    {
        mouseLook.MouseRotation(transform, cameraTransform);
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
                objRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
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

    private IEnumerator LerpCameraPostion(Vector3 startPos, Vector3 endPosition, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            timeElapsed += Time.fixedDeltaTime;

            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    private void StepCycle()
    {
        if (moveVector.sqrMagnitude > 0 && (moveInput.x != 0 || moveInput.y != 0))
        {
            stepCycle += (moveVector.magnitude + (moveSpeed * (isCrouch? 0.5f : 1f))) * Time.deltaTime;
        }
        else if(moveVector.sqrMagnitude == 0 && moveInput.x == 0 && moveInput.y == 0)
        {
            stepCycle = stepInterval - 1;
            stepNum = 1;
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

        audioSource.volume = 0.06f;
        audioSource.clip = GetRandomFootStepClip();
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void PlayJumpAudio()
    {
        audioSource.volume = 0.15f;
        audioSource.clip = GetRandomFootStepClip();
        audioSource.PlayOneShot(audioSource.clip);
        stepCycle = stepInterval - 1;
        stepNum = 1;
    }

    private AudioClip GetRandomFootStepClip()
    {
        AudioClip[] footStepClip;

        switch (floorMat)
        {
            case FloorMaterial.CONCRET:
                footStepClip = footStepSound_concret;
                break;
            case FloorMaterial.METAL:
                footStepClip = footStepSound_metal;
                break;
            default:
                footStepClip = footStepSound_concret;
                break;
        }

        if (stepNum != 0)
            stepNum = 0;
        else
        {
            stepNum = Random.Range(1, footStepClip.Length - 1);
        }

        return footStepClip[stepNum];
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        float height = isCrouch ? playerCrouchHeight : playerHeight;

        Vector3 playerCenter = transform.position + new Vector3(0f, playerCollider.center.y , 0f);
        Vector3 groundCheckBox = new Vector3(0.2f, 0.05f, 0.2f);
        float maxDis = height / 2 + 0.1f;

        int excludedLayers = ~LayerMask.GetMask("Player", "InvisiblePlayer");

        if (Physics.BoxCast(playerCenter, groundCheckBox, Vector3.down, out hit, Quaternion.identity, maxDis, excludedLayers))
        {
            if (hit.collider.isTrigger)
                return;

            FloatingPlayer(hit, height, playerCenter, maxDis);

            if (hit.collider.tag == "Concret")
            {
                floorMat = FloorMaterial.CONCRET;
            }
            else if (hit.collider.tag == "Metal")
            {
                floorMat = FloorMaterial.METAL;
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
        float forceValue = distance * 50f - objRigidbody.velocity.y;
        Vector3 floatingForce = new Vector3(0f, forceValue, 0f);
        objRigidbody.AddForce(floatingForce, ForceMode.VelocityChange);
    }

    private bool PortalOnFloorCheck(Vector3 center, float maxDistacne)
    {
        RaycastHit hit;
        int portalLayer = LayerMask.GetMask("Portal");

        if (!portalPair.PlacedBothPortal())
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
            pState = PlayerState.WALK;
        }
        else if (isJump)
        {
            pState = PlayerState.JUMP;
        }
        else if (isCrouch && !isJump && moveVectorMagnitude == 0)
        {
            pState = PlayerState.CROUCH;
        }
        else if (isCrouch && !isJump && moveVectorMagnitude != 0)
        {
            pState = PlayerState.CROUCHWALK;
        }
        else
        {
            pState = PlayerState.IDLE;
        }
    }

    private void SetColliderHeightAndCenter(float height)
    {
        playerCollider.height = height * 0.9f;

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

}
