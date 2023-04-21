using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMove : CComponent
{
    #region public

    public float runSpeed = 0.0f;
    public float jumpForce = 5.0f;
    public float mouseSensitivity = 2.0f;
    public Vector3 playerMoveVector;

    #endregion

    Transform playerTransform;
    Transform chellModel;
    Transform cameraTransform;
    Animator chellAnimator;

    Vector3 mouseMove;
    Vector3 characterRotation;
    public Quaternion qCharacterRotation { private set; get; }


    private new Rigidbody rigidbody;

    public override void Awake()
    {
        base.Awake();

        playerTransform = transform.transform;

        chellModel = transform.GetChild(0);
        chellAnimator = chellModel.GetComponent<Animator>();

        cameraTransform = Camera.main.transform;

        rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Update()
    {
        base.Update();

        characterRotation += new Vector3(0, Input.GetAxisRaw("Mouse X") * mouseSensitivity, 0);
        qCharacterRotation = Quaternion.Euler(characterRotation);
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, qCharacterRotation, 10.0f * Time.deltaTime);

        Quaternion characterRot = Quaternion.LookRotation(qCharacterRotation.eulerAngles);

        characterRot.x = characterRot.z = 0;

        chellAnimator.SetFloat("aLookRotation", characterRot.eulerAngles.y);

  
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        PlayerMovement();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * mouseSensitivity, 0, 0);
        
        if (mouseMove.x < -5)
        {
            mouseMove.x = -5;
        }
        else if (50 < mouseMove.x)
        {
            mouseMove.x = 50;
        }
        
        cameraTransform.localEulerAngles = mouseMove;
    }

    private void PlayerMovement()
    {
        float mHorizontal = Input.GetAxis("Horizontal");
        float mVertical = Input.GetAxis("Vertical");
        

        Vector3 movement = new Vector3(mHorizontal, 0f, mVertical);
        playerMoveVector = transform.TransformDirection(movement) * runSpeed;
        rigidbody.velocity = new Vector3(playerMoveVector.x, rigidbody.velocity.y, playerMoveVector.z);

        float speed = playerMoveVector.sqrMagnitude;
        chellAnimator.SetFloat("aSpeed", speed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(GroundCheck())
            {
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    public void ResetTargetRotation()
    {
        qCharacterRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }

    bool GroundCheck()
    {
        return Physics.Raycast(playerTransform.position, Vector3.down, 0.5f);
    }
}