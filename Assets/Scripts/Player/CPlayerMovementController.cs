using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerMovementController : CTeleportObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    private CharacterController characterController;
    private Vector2 inputVec;
    public Vector3 moveDir;
    private bool isJump;
    private bool isJumping;
    private CollisionFlags collisionFlags;
    private bool isPreviousdlyGrounded;
    private bool isCrouch;
    public override void Awake()
    {
        base.Awake();
    }

    public enum PlayerState
    {
        IDLE,
        WALK,
        JUMP,
        CROUCH,
        CROUCHWALK,
        FALL
    }

    public PlayerState pState;

    public override void Start()
    {
        base.Start();

        characterController = GetComponent<CharacterController>();
        mouseLook.Init(transform, cameraTransform);
    }


    public override void Update()
    {
        base.Update();

        PlayerRotation();

        if(characterController.isGrounded)
        {
            MovePlayer(1.0f);
        }
        else
        {
            moveDir.y -= 9.8f * Time.deltaTime;
            MovePlayer(0.01f);
        }

        characterController.Move(moveDir * Time.deltaTime);
        /*
        if (!isJump)
        {
            isJump = Input.GetKeyDown(KeyCode.Space);
        }

        if(!isPreviousdlyGrounded && characterController.isGrounded)
        {
            moveDir.y = 0f;
            isJump = false;
        }
        if(!characterController.isGrounded && isJumping && isPreviousdlyGrounded)
        {
            moveDir.y = 0f;
        }

        isPreviousdlyGrounded = characterController.isGrounded;
        */
        PlayerStateChange();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        /*
        GetInput();

        Vector3 desiredMove = transform.forward * inputVec.y + transform.right * inputVec.x;
        
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo, 
                    characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
        Debug.Log("desiredM : " + desiredMove);
        moveDir.x = desiredMove.x * moveSpeed;
        moveDir.z = desiredMove.z * moveSpeed;
        //Debug.Log("moveDir : " + moveDir);
        if(characterController.isGrounded)
        {
            if(isJump)
            {
                moveDir.y = jumpSpeed;
                isJump = false;
                isJumping = true;
            }
        }
        else
        {
            moveDir += Physics.gravity * Time.fixedDeltaTime;
        }

        collisionFlags = characterController.Move(moveDir * Time.fixedDeltaTime);
        */
    }

    private void MovePlayer(float rate)
    {
        float tempMoveY = moveDir.y;

        moveDir.y = 0f;

        Vector3 inputMove = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Debug.Log(inputMove);
        float moveXZMagnitude = inputMove.sqrMagnitude;
        inputMove = transform.TransformDirection(inputMove);

        if(moveXZMagnitude <= 1)
        {
            inputMove *= moveSpeed;
        }
        else
        {
            inputMove = inputMove.normalized * moveSpeed;
        }

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            moveDir = Vector3.MoveTowards(moveDir, inputMove, rate * moveSpeed);
        }
        else
        {
            moveDir = Vector3.MoveTowards(moveDir, Vector3.zero, (1 - moveXZMagnitude) * rate  * moveSpeed);
        }
        
        moveDir.y = tempMoveY;
    }
    private void GetInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inputVec = new Vector2(horizontal, vertical);
        //Debug.Log("input V : " + inputVec);
        if(inputVec.sqrMagnitude > 1)
        {
            inputVec = inputVec.normalized;
        }
    }

    private void PlayerRotation()
    {
        mouseLook.MouseRotation(transform, cameraTransform);
    }

    private void PlayerStateChange()
    {
        if (moveDir != Vector3.zero && !isJumping && !isCrouch)
        {
            pState = PlayerState.WALK;
        }
        else if (isJumping)
        {
            pState = PlayerState.JUMP;
        }
        else if (isCrouch && !isJump && moveDir == Vector3.zero)
        {
            pState = PlayerState.CROUCH;
        }
        else if (isCrouch && !isJump && moveDir != Vector3.zero)
        {
            pState = PlayerState.CROUCHWALK;
        }
        else
        {
            pState = PlayerState.IDLE;
        }
    }

}
