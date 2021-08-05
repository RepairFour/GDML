using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Controller : MonoBehaviour
{
    [Header ("Character Movement Values")]
    public float maxSpeed;
    public float maxJumpStrafeSpeed;
    public float maxStrafeSpeed;
    public float accelleration;
    public float deccelleration;
    public float strafeAcceleration;
    public float strafeJumpDecelleration;

    [Header ("Dash Variables")]
    public float dashSpeed;
    public float dashTime;
    public int maxDashAmount;
    public float dashCooldown;

    [Header("Jump Variables")]
    public float jumpSpeed;
    public float gravity;
    public float groundedTime;
    public LayerMask layerMask;

    [Header ("Hookshot Variables")]
    [Tooltip("Range of the hookshot")]
    public float hookShotDistance;
    [Tooltip("How fast you travel when hookshoting")]
    public float hookShotSpeed;
    [Tooltip("How fast the grapple flys out to attach to an object")]
    public float hookShotThrowSpeed;
    [Tooltip("Gives a small boost of speed at the end of a hookshot")]
    public float momentumExtraSpeed = 7f;
    [Tooltip("How quickly momentum drops off once hookshot is done")]
    public float momentumDrag = 3f;
   
    public float hookCooldown;

    [Header ("Transforms")]
    public Transform cameraTransform;
    public Transform hookShotTransform;

    

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
    [SerializeField] float dashCooldownTimer;
    [SerializeField] int numberOfDashesCurrent;

    [SerializeField] bool hookShoting;
    [SerializeField] bool hookShotFiring;
    [SerializeField] bool hookShotMove;
    [SerializeField] Vector3 hookShotDirection;
    [SerializeField] float hookShotSize;
    [SerializeField] Vector3 hookHitPoint;
    [SerializeField] Vector3 momentum;
    [SerializeField] float hookCooldownTimer;
    [SerializeField] bool hookOnCooldown;


    PlayerMap input;

    CharacterController cc;
    

    private ButtonControl jump;
    private ButtonControl hook;

    private void Start()
    {
        input = new PlayerMap();
        input.Enable();
        cc = GetComponent<CharacterController>();
        numberOfDashesCurrent = maxDashAmount;

        jump = (ButtonControl)input.Player.Jump.controls[0];
        hook = (ButtonControl)input.Player.Hook.controls[0];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (hookShotMove)
        {
            HookShotMove();
            return;
        }

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

        if (!hookOnCooldown)
        {
            CheckGrappleHook();
        }

        QueueJump();
        CheckDash();
        HandleCooldownFunctions();
        CheckGrounded();

        if (hookShotFiring)
        {
            FiringHookShot();
        }

        if (grounded)
        {
            GroundMove();
        }
        if (!grounded || queueJump)
        {
            AirMove();
        }
        currentVelocity += momentum;

        cc.Move(currentVelocity * Time.deltaTime);

        if(momentum.magnitude >= 0f)
        {
            
            momentum -= momentum * momentumDrag * Time.deltaTime;
            if (momentum.magnitude < .01f)
            {
                momentum = Vector3.zero;
            }
        }

    }

    private void CheckGrappleHook()
    {
        if (input.Player.Hook.triggered)
        {
            CheckHookShotHit();
        }
    }

    private void FiringHookShot()
    {
        hookShotTransform.gameObject.SetActive(true);
        hookShotTransform.LookAt(hookHitPoint);
        hookShotSize += hookShotThrowSpeed * Time.deltaTime;
        hookShotTransform.localScale = new Vector3(1, 1, hookShotSize);

        if(hookShotSize >= Vector3.Distance(transform.position, hookHitPoint))
        {
            hookShotMove = true;
            hookShotFiring = false;
        }
        if (hook.wasReleasedThisFrame)
        {

            //momentum = hookShotDirection * hookShotSpeed * momentumExtraSpeed;
            //momentum.y = 0;
            //currentVelocity.y = 0;
            CancelHookShot();
        }
    }
    private void CheckHookShotHit()
    {
        if(Physics.Raycast(hookShotTransform.position, cameraTransform.forward, out RaycastHit hit, hookShotDistance))
        {
            hookHitPoint = hit.point;
            Debug.Log(hit.collider.name);
            hookShotFiring = true;
            HookShotDirection();
            hookShotSize = 0f;
            hookShotTransform.LookAt(hookHitPoint);
        }
    }

    private void HookShotDirection()
    {
        hookShotDirection = (hookHitPoint - transform.position).normalized;
    }
    private void CancelHookShot()
    {
        hookShotMove = false;
        hookShotFiring = false;
        hookShotSize = 0;
        hookShotTransform.localScale = new Vector3(1, 1, hookShotSize);
        hookShotTransform.gameObject.SetActive(false);

    }
    private void HookShotMove()
    {
        HookShotDirection();
        currentVelocity = hookShotSpeed * hookShotDirection;
        hookShotTransform.LookAt(hookHitPoint);
        if (Vector3.Distance(hookHitPoint, transform.position) < 2)
        {

            momentum = hookShotDirection * hookShotSpeed * momentumExtraSpeed;
            momentum.y = 0;
            currentVelocity.y = 0;
            CancelHookShot();
            hookOnCooldown = true;
        }
        if (hook.wasReleasedThisFrame)
        {

            momentum = hookShotDirection * hookShotSpeed * momentumExtraSpeed;
            momentum.y = 0;
            currentVelocity.y = 0;
            CancelHookShot();
            hookOnCooldown = true;
        }
           
        
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

        if (hookShotMove)
        {
            HookShotMove();
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
        if (input.Player.Dash.triggered && numberOfDashesCurrent > 0)
        {
            dashing = true;
            numberOfDashesCurrent--;
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

    private void HandleCooldownFunctions()
    {
        DashTimer();
        DashCooldowns();
        HookShotCooldowns();
    }

    private void DashCooldowns()
    {
        if(numberOfDashesCurrent < maxDashAmount)
        {
            dashCooldownTimer += Time.deltaTime;
            if(dashCooldownTimer >= dashCooldown)
            {
                numberOfDashesCurrent++;
                dashCooldownTimer = 0;
            }
        }
    }

    private void HookShotCooldowns()
    {
        if (hookOnCooldown)
        {
            hookCooldownTimer += Time.deltaTime;
            if(hookCooldownTimer >= hookCooldown)
            {
                hookOnCooldown = false;
                hookCooldownTimer = 0;
            }
        }
    }
}
