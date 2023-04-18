using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class ChellMove : CComponent
{
    #region public

    public float runSpeed = 0.0f;
    public float mouseSensitivity = 2.0f;
    //public GameObject playerCharactrer = null;

    #endregion

    Transform oTransform;
    Transform chellModel;
    Transform portalGunModel;
    Transform cameraTransform;
    CharacterController chellController;
    Animator chellAnimator;

    Vector3 move;
    Vector3 mouseMove;
    Vector3 characterRotation;
    Vector3 portalGunRotation;
    Quaternion qCharacterRotation;

    public override void Awake()
    {
        base.Awake();

        oTransform = transform;
        chellModel = transform.GetChild(0);
        portalGunModel = transform.GetChild(1);
        cameraTransform = Camera.main.transform;
        chellController = GetComponent<CharacterController>();
        chellAnimator = chellModel.GetComponent<Animator>();
    }


    public override void Update()
    {
        base.Update();
        ChellController_Move();

        chellController.Move(move * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            chellAnimator.SetTrigger("aJump");
        }
    }
    public override void LateUpdate()
    {
        base.LateUpdate();

        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * mouseSensitivity, 0, 0);

        if(mouseMove.x < -5)
        {
            mouseMove.x = -5;
        }
        else if(50 < mouseMove.x)
        {
            mouseMove.x = 50;
        }

        cameraTransform.localEulerAngles = mouseMove;

    }

    private void ChellController_Move()
    {

        float tempMoveY = move.y;

        move.y = 0;

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        direction *= runSpeed;


        characterRotation += new Vector3(0, Input.GetAxisRaw("Mouse X") * mouseSensitivity, 0);
        portalGunModel.localEulerAngles = mouseMove;
        qCharacterRotation = Quaternion.Euler(characterRotation);
        
        oTransform.rotation = Quaternion.Slerp(oTransform.rotation, qCharacterRotation, 10.0f * Time.deltaTime);

        if(move != Vector3.zero)
        {
            Quaternion characterRotation = Quaternion.LookRotation(move);
            
            characterRotation.x = characterRotation.z = 0;
            
            chellAnimator.SetFloat("aLookRotation", characterRotation.eulerAngles.y);
            
            //chellModel.rotation = Quaternion.Slerp(chellModel.rotation, characterRotation, 10.0f * Time.deltaTime);

            
        }

        move = transform.TransformDirection(direction);

        float speed = move.sqrMagnitude;
        chellAnimator.SetFloat("aSpeed", speed);

        move.y = tempMoveY;
    }
}
