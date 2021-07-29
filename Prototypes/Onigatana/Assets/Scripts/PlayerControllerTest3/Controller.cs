using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Controller : MonoBehaviour
{
    public float maxSpeed;
    public float maxJumpStrafeSpeed;
    public float maxStrafeSpeed;
    public float accelleration;
    public float deccelleration;
    public float strafeAcceleration;
    public float strafeJumpDecelleration;

    public float dashSpeed;
    public float dashTime;

    public float jumpSpeed;
    public float gravity;
    public float groundedTime;

    public Transform cameraTransform;
    public LayerMask layerMask;

    float rotationX;
    float rotationY;

    float groundedTimer;

    [Header("Debugging Variables")]
    [SerializeField] bool queueJump;

    [SerializeField] Vector3 currentMoveDirection;
    [SerializeField] Vector3 lastMoveDirection;
    [SerializeField] Vector3 inputDirection;

    [SerializeField] Vector3 currentVelocity;
    [SerializeField] float currentSpeed;
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
        currentMoveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        currentMoveDirection = transform.TransformDirection(currentMoveDirection);
    }

    void GroundMove()
    {
        GetMoveDirection();

        if (Mathf.Abs(inputDirection.x) > 0.01 || Math.Abs(inputDirection.y) > 0.01)
        {
            if (Mathf.Abs(inputDirection.x) > 0 && Mathf.Abs(inputDirection.y) > 0)
            {
                StrafeAccelerate();
            }
            else
            {
                Accelerate();
            }
            currentVelocity = currentMoveDirection * currentSpeed;
            
        }


        else 
        {
            lastMoveDirection = currentVelocity;
            lastMoveDirection.Normalize();
            Decellerate();
            currentVelocity = lastMoveDirection * currentSpeed;
            
        }

        if (dashing)
        {
            currentVelocity = dashSpeed * currentMoveDirection;
        }

        if (queueJump)
        {
            currentVelocity.y = jumpSpeed;
            queueJump = false;
            grounded = false;
            groundedTimer = 0;
        }
    }



    private void Update()
    {
        if (grounded)
        {
            groundedTimer += Time.deltaTime;
        }

        var horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
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
        if (!grounded || queueJump)
        {
            AirMove();
        }

        cc.Move(currentVelocity * Time.deltaTime);

    }

    private void AirMove()
    {
        float yspeed = currentVelocity.y;
        GetMoveDirection();

        if (dashing)
        {
            currentVelocity = dashSpeed * currentMoveDirection;
            return;
        }

        /* Input is read in on the x and y axis and stored in the variable inputDirection
         * it is converted to x and z and then stored in moveDirection which is then transform
         * into the objects world space axis. Input direction is used here to make sure the 
         * player is strafing in the air. If they are they go faster. This emulates strafe jumping 
         * to a certain extent */
        if (Mathf.Abs(inputDirection.x) > 0 && Mathf.Abs(inputDirection.y) > 0)
        {

            JumpStrafeAccelerate();
            currentVelocity = currentMoveDirection * currentSpeed;
            currentVelocity.y = yspeed;
            currentVelocity.y -= gravity * Time.deltaTime;
            return;
        }

        if (Mathf.Abs(inputDirection.y) > 0 || Mathf.Abs(inputDirection.x) > 0)
        {
           
            Accelerate();
            currentVelocity = currentMoveDirection * currentSpeed;
            
        }

        else
        {
            
            lastMoveDirection = currentVelocity;
            lastMoveDirection.y = 0;
            lastMoveDirection.Normalize();
            Decellerate();
            currentVelocity = lastMoveDirection * currentSpeed;
            
        }
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

        if (Physics.Raycast(ray, out hit, 1.1f, layerMask))
        {
            grounded = true;
            Debug.Log(hit.collider.name);
        }
        else
        {
            grounded = false;
            groundedTimer = 0;
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
        if (dashingTimer >= dashTime)
        {
            dashing = false;
            dashingTimer = 0f;
        }
    }

    void Accelerate()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += accelleration * Time.deltaTime;
        }
       

        if (currentSpeed >= maxSpeed && groundedTimer >= groundedTime)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, strafeJumpDecelleration * Time.deltaTime);
            if(Mathf.Abs(maxSpeed - currentSpeed) < 0.01)
            {
                currentSpeed = maxSpeed;
            }
        }


    }
    void JumpStrafeAccelerate()
    {
        if(currentSpeed < maxJumpStrafeSpeed)
        {
            currentSpeed += strafeAcceleration * Time.deltaTime;
            if (currentSpeed >= maxJumpStrafeSpeed)
            {
                currentSpeed = maxJumpStrafeSpeed;
            }
        }
       
    }
    void StrafeAccelerate()
    {
        if (currentSpeed < maxStrafeSpeed)
        {
            currentSpeed += accelleration * Time.deltaTime;
            
        }
        if (currentSpeed >= maxStrafeSpeed && groundedTimer >= groundedTime)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxStrafeSpeed, strafeJumpDecelleration * Time.deltaTime);
            if (Mathf.Abs(maxStrafeSpeed - currentSpeed) < 0.01)
            {
                currentSpeed = maxStrafeSpeed;
            }
        }
    }

    private void Decellerate()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= deccelleration * Time.deltaTime;
            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
            }
        }
        
    }
}
