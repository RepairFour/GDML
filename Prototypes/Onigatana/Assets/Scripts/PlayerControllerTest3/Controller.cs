using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Controller : MonoBehaviour
{
    public float forwardSpeed;

    public float dashSpeed;
    public float dashTime;
    
    public float strafeSpeed;
    public float airSpeed;

    public float jumpSpeed;
    public float gravity;
    public Transform cameraTransform;
    public LayerMask layerMask;
    public bool eightWayDash;
    
    float rotationX;
    float rotationY;

    [SerializeField] bool queueJump;

    [SerializeField]Vector3 moveDirection;
    [SerializeField] Vector3 inputDirection;

    [SerializeField]Vector3 currentVelocity;
    [SerializeField] float speed;
    [SerializeField] bool grounded;
    [SerializeField] bool dashing;
    [SerializeField] float dashingTimer;

    PlayerMap input;

    CharacterController cc;

    private ButtonControl jump;

    private void Start()
    {
        input = new PlayerMap();
        input.Enable();
        cc = GetComponent<CharacterController>();
        jump = (ButtonControl)input.Player.Jump.controls[0];
    }

    void GetMoveDirection()
    {
        inputDirection = input.Player.Move.ReadValue<Vector2>();
        moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        moveDirection = transform.TransformDirection(moveDirection);
    }

    void GroundMove()
    {
        GetMoveDirection();
        currentVelocity = moveDirection * forwardSpeed;

        if (eightWayDash)
        {
            if (dashing)
            {
                currentVelocity = dashSpeed * moveDirection;
            }
        }
        else
        {
            if (dashing && Mathf.Abs(inputDirection.y) > 0)
            {
                currentVelocity = dashSpeed * transform.forward;
            }
            if (dashing && inputDirection.y < 0)
            {
                currentVelocity = dashSpeed * (transform.forward * -1);

            }
        }

        //currentVelocity.y -= gravity * Time.deltaTime;
        if (queueJump)
        {
            currentVelocity.y = jumpSpeed;
            queueJump = false;
            grounded = false;
        }
    }

    private void Update()
    {
        var horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        speed = horizontalVelocity.magnitude;
        var mouseVector = input.Player.Mouse.ReadValue<Vector2>();

        rotationX -= mouseVector.y;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().y;
        rotationY += mouseVector.x;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().x;

        //Debug.Log(rotationX + " " + rotationY);
        
        if (rotationX < -90)
        {
            rotationX = -90;
        }
        else if (rotationX > 90)
        {
            rotationX = 90;
        }

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        cameraTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        QueueJump();
        CheckDash();
        DashTimer();
        CheckGrounded();
        if (grounded)
        {
            GroundMove();
        }
        else if (!grounded || queueJump)
        {
            AirMove();
        }
        
        cc.Move(currentVelocity * Time.deltaTime);
        
    }

    private void AirMove()
    {
        float yspeed = currentVelocity.y;
        GetMoveDirection();


        if (eightWayDash)
        {
            if (dashing)
            {
                currentVelocity = dashSpeed * moveDirection;
                return;
            }

        }
        else
        {
            if (dashing && inputDirection.y > 0)
            {
                currentVelocity = dashSpeed * transform.forward;
                return;
            }
            if (dashing && inputDirection.y < 0)
            {
                currentVelocity = dashSpeed * (transform.forward * -1);
                return;
            }
        }

        /* Input is read in on the x and y axis and stored in the variable inputDirection
         * it is converted to x and z and then stored in moveDirection which is then transform
         * into the objects world space axis. Input direction is used here to make sure the 
         * player is strafing in the air. If they are they go faster. This emulates strafe jumping 
         * to a certain extent */
        if (Mathf.Abs(inputDirection.x) > 0 && Mathf.Abs(inputDirection.y) > 0)
        {
            currentVelocity = moveDirection * (forwardSpeed + strafeSpeed);
            currentVelocity.y = yspeed;
            currentVelocity.y -= gravity * Time.deltaTime;
            return;
        }

        currentVelocity = moveDirection * forwardSpeed;
        currentVelocity.y = yspeed;
        currentVelocity.y -= gravity * Time.deltaTime;
    }

    private void QueueJump()
    {
        if (input.Player.Jump.triggered /*jump.wasPressedThisFrame*/ &&
            !queueJump)
        {
            queueJump = true;
        }

        if (jump.wasReleasedThisFrame)
        {
            queueJump = false;

        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.up * -1);
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);

        if (Physics.Raycast(ray,out hit, 1.1f, layerMask)){
            grounded = true;
            Debug.Log(hit.collider.name);
        }
    }

    void CheckDash()
    {
        if (dashing)
        {
            return;
        }
        if (input.Player.Dash.triggered)
        {
            dashing = true;
        }
    }

    void DashTimer()
    {
        if (!dashing)
        {
            return;
        }
        dashingTimer += Time.deltaTime;
        if(dashingTimer >= dashTime)
        {
            dashing = false;
            dashingTimer = 0f;
        }
    }
}
