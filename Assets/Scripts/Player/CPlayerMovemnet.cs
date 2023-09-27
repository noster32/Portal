using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerMovemnet : CComponent
{

    #region public
    [Header("MovementSpeed")]
    private float playerSpeed;

    public float moveSpeed;
    public float crouchSpeed;

    [Header("Movement")]
    public float groundDrag;
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    public bool readyToJump;

    private float startHeight;
    public float crouchHeight;

    [Header("Player Transform")]
    public Transform orientation;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    bool grounded;
    #endregion

    [Header("KeyBinding")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    CapsuleCollider capsuleCollider;

    CPlayerCamera playerCamera;

    Rigidbody rb;

    Transform characterTransform;

    Transform playerModel;
    Animator playerAnimator;

    public MoveState moveState;
    public enum MoveState
    {
        walk,
        air,
        crouch
    }

    public override void Awake()
    {
        base.Awake();

        characterTransform = transform.transform;
        capsuleCollider = GetComponent<CapsuleCollider>();

        playerModel = transform.GetChild(0);
        playerAnimator = playerModel.GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
    }


    public override void Start()
    {
        base.Start();

        rb.freezeRotation = true;
        readyToJump = true;
        startHeight = capsuleCollider.height;
    }

    public override void Update()
    {
        base.Awake();

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, ground);

        playerInput();
        SpeedControl();
        PlayerStateHandler();
        playerCharacterAnim();

        if (grounded)
        {
            rb.drag = groundDrag;
            playerAnimator.SetBool("isGrounded", true);
        }
        else
            rb.drag = 0;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        movePlayer();
    }

    private void playerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            playerAnimator.SetTrigger("aJump");
            playerAnimator.SetBool("isGrounded", false);
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        //Crouch
        if(Input.GetKey(crouchKey))
        {
            capsuleCollider.height = crouchHeight;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
         
        //Stop Crouch
        if(Input.GetKeyUp(crouchKey))
        {
            capsuleCollider.height = startHeight;

        }
    }

    private void playerCharacterAnim()
    {
        characterTransform.rotation = orientation.rotation;
        playerAnimator.SetFloat("DirectionN", verticalInput, 0.5f, Time.deltaTime);
        playerAnimator.SetFloat("DirectionE", horizontalInput, 0.5f, Time.deltaTime);

        float moveCheck = moveDirection.sqrMagnitude;
        playerAnimator.SetFloat("aSpeed", moveCheck);

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {

        }

    }


    private void movePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    } 

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    
    private void PlayerStateHandler()
    {
        if (grounded)
        {
            moveState = MoveState.walk;
            playerSpeed = moveSpeed;
        }
        else if(grounded && Input.GetKey(crouchKey)) 
        {
            moveState = MoveState.crouch;
            playerSpeed = crouchSpeed;
        }
        else
        {
            moveState = MoveState.air;
        }

    }
}
