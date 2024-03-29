﻿using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    #region Inspector Variables
    [Header("Camera Variables")]
    public float cameraHeight = 1f;
    public float slideCameraHeight = 0.5f;

    [Header("Mouse sensitivity values")]
    public float xSensitivity;
    public float ySensitivity;
    
    public float yMin;
    public float yMax;
    
    [Header ("Character Movement Values")]
    public float maxSpeed;
    public float globalMaxSpeed;
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

    [Header("Slide Variables")]
    public float maxSlideMomentum;
    public float slideDirectionalScalar;
    public float slideHeight;
    public float normalHeight;
    public float slideAccelerate;
    public float slideDeccelerate;
    public float minSlideTime;
    public float slideCooldown;

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
    public Transform hookShotHand;
   
    public float hookCooldown;

    [Header ("Transforms")]
    public Transform cameraTransform;
    public Transform hookShotTransform;
    LineRenderer lr;

    [Header("UI Variables")]
    public Image crossHair;
    #endregion
    
    #region Debugging and Private Variables
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

    [SerializeField] bool slideQueued;
    [SerializeField] bool sliding;
    [SerializeField] float slideMomentum;
    [SerializeField] float slidingTimer;
    [SerializeField] bool maxSlideSpeedHit;
    [SerializeField] bool slideOnCooldown;
    [SerializeField] float slideCooldownTimer;

    [SerializeField] bool hookShoting;
    [SerializeField] bool hookShotFiring;
    [SerializeField] bool hookShotMove;
    [SerializeField] Vector3 hookShotDirection;
    [SerializeField] float hookShotSize;
    [SerializeField] Vector3 hookHitPoint;
    [SerializeField] Vector3 momentum;
    [SerializeField] float hookCooldownTimer;
    [SerializeField] bool hookOnCooldown;
    bool crouch;
    PlayerMap input;

    CharacterController cc;

    private ButtonControl jump;
    private ButtonControl slide;
    private ButtonControl hook;
    #endregion

    #region UnityFunctions
    private void Start()
    {
        input = new PlayerMap();
        input.Enable();
        cc = GetComponent<CharacterController>();
        numberOfDashesCurrent = maxDashAmount;

        jump = (ButtonControl)input.Player.Jump.controls[0];
        hook = (ButtonControl)input.Player.Hook.controls[0];
        slide = (ButtonControl)input.Player.Slide.controls[0];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        cc.gameObject.transform.position = new Vector3(0, normalHeight, 0);

        lr = hookShotTransform.gameObject.GetComponent<LineRenderer>();
        
    }

    private void Update()
    {
        if (grounded)
        {
            groundedTimer += Time.deltaTime;
        }

        var mouseVector = input.Player.Mouse.ReadValue<Vector2>();

        rotationX -= mouseVector.y * Time.deltaTime * xSensitivity;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().y;
        rotationY += mouseVector.x * Time.deltaTime * ySensitivity;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().x;

        //Debug.Log(rotationX + " " + rotationY);

        if (rotationX < yMin)
        {
            rotationX = yMin;
        }
        else if (rotationX > yMax)
        {
            rotationX = yMax;
        }

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        cameraTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        if (!hookOnCooldown)
        {
            CheckGrappleHook();
        }
        CheckGrounded();
        QueueJump();
        QueueSlide();
        CheckDash();
        HandleCooldownFunctions();
        

        if (hookShotFiring)
        {
            FiringHookShot();
        }

        if (grounded)
        {
            GroundMove();
        }
        if (!grounded /*|| queueJump*/)
        {
            AirMove();
        }
        currentVelocity += momentum;

        cc.Move(currentVelocity * Time.deltaTime);

        if (sliding)
        {
            cameraTransform.localPosition = new Vector3(0, slideCameraHeight, 0);
            cc.height = slideHeight;
        }

        if(momentum.magnitude >= 0f)
        {
            
            momentum -= momentum * momentumDrag * Time.deltaTime;
            if (momentum.magnitude < .01f)
            {
                momentum = Vector3.zero;
            }
        }
    }
    #endregion

    #region HookShot Functions
    private void CheckGrappleHook()
    {
        if (Physics.Raycast(transform.position, cameraTransform.forward, out RaycastHit hit, hookShotDistance))
        {
            crossHair.gameObject.SetActive(true);
        }
        else
        {
            crossHair.gameObject.SetActive(false);
        }

        if (input.Player.Hook.triggered)
        {
            CheckHookShotHit();
        }
        
    }

    private void FiringHookShot()
    {
        hookShotTransform.gameObject.SetActive(true);
        var direction = hookHitPoint - hookShotHand.position;
        direction.Normalize();
        hookShotHand.position += hookShotThrowSpeed * direction * Time.deltaTime;
        
        hookShotTransform.LookAt(hookHitPoint);
        hookShotSize = Vector3.Distance(hookShotHand.position, hookShotTransform.position);
        //hookShotTransform.localScale = new Vector3(1, 1, hookShotSize);
        //
        if(hookShotSize >= Vector3.Distance(hookShotTransform.position, hookHitPoint))
        {
            hookShotMove = true;
            hookShotFiring = false;
            //hookShotHand.position = hookHitPoint;
        }
        
        lr.SetPosition(0, hookShotTransform.position);
        lr.SetPosition(1, hookShotHand.position);
        
        if (hook.wasReleasedThisFrame)
        {
            CancelHookShot();
        }
    }
    private void CheckHookShotHit()
    {
        if(Physics.Raycast(transform.position, cameraTransform.forward, out RaycastHit hit, hookShotDistance))
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
        //hookShotTransform.localScale = new Vector3(1, 1, hookShotSize);
        hookShotHand.position = hookShotTransform.position;
        hookShotTransform.gameObject.SetActive(false);
        //lr.positionCount = 0;

    }

    private void CancelHookShotMomentum()
    {
        hookShotDirection.Normalize();
        momentum = hookShotDirection * hookShotSpeed * momentumExtraSpeed;
        momentum.y = 0;
        //momentum.y = hookShotDirection.y * hookShotSpeed/yMomentumSc;
        currentVelocity.y = 0;
        CancelHookShot();
        hookOnCooldown = true;
    }
    private void HookShotMove()
    {
        HookShotDirection();
        currentVelocity = hookShotSpeed * hookShotDirection;
        hookShotTransform.LookAt(hookHitPoint);
        lr.SetPosition(0, hookShotTransform.position);
        lr.SetPosition(1, hookShotHand.position);

        if (Vector3.Distance(hookHitPoint, transform.position) < 2)
        {
            CancelHookShotMomentum();
            //currentVelocity.y = yMomentumScalarValue;
        }
        if (hook.wasReleasedThisFrame)
        {
            CancelHookShotMomentum();
            //currentVelocity.y = 0;
            //momentum.y = 0;
        }
    }
    #endregion

    #region Movement Functions
    void GetMoveDirection()
    {
        inputDirection = input.Player.Move.ReadValue<Vector2>();
        currentMoveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        currentMoveDirection = transform.TransformDirection(currentMoveDirection);
    }
    Vector3 GetSlideMoveDirection()
    {
        
        var inputS = input.Player.Move.ReadValue<Vector2>();
        if (Mathf.Abs(inputS.y) > 0)
        {
            var slideDirection = new Vector3(inputS.x, 0, 0);
            slideDirection = transform.TransformDirection(slideDirection);
            return slideDirection * slideDirectionalScalar;
        }
        return Vector3.zero;
        
    }

    private void AirMove()
    {
        float yspeed = currentVelocity.y;
        GetMoveDirection();

        if (dashing)
        {
            HandleDash();
            CancelHookShot();
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
            HandleVelocity();
            HandleGravity(yspeed);
            return;
        }

        if (Mathf.Abs(inputDirection.y) > 0 || Mathf.Abs(inputDirection.x) > 0)
        {
           
            Accelerate();
            currentVelocity = currentMoveDirection * currentSpeed;
            
        }

        else
        {
            Decellerate();
        }
        HandleGravity(yspeed);
    }
    void GroundMove()
    {
        if (slideQueued)
        {
            sliding = true;
            slideQueued = false;
        }

        if (hookShotMove)
        {
            CancelSlideForHookShot();
            HookShotMove();
            return;
        }

        if (sliding)
        {
            HandleMomentumSpeed();
            
            slidingTimer += Time.deltaTime;
            currentSpeed = currentSpeed + slideMomentum;
            
            if (currentSpeed > globalMaxSpeed)
            {
                currentSpeed = globalMaxSpeed;
            }
            
            HandleSlideVelocity();
            HandleJump();
            return;
        }
        GetMoveDirection();
        
        if (dashing)
        {
            HandleDash();
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

            HandleVelocity();
        }
        else
        {
            lastMoveDirection = currentVelocity;
            lastMoveDirection.Normalize();
            Decellerate();
            currentVelocity = lastMoveDirection * currentSpeed;

        }
        HandleJump();
    }

    void HandleJump()
    {
        if (queueJump)
        {
            currentVelocity.y = jumpSpeed;
            queueJump = false;
            grounded = false;
            groundedTimer = 0;
            CancelSlideForHookShot();
        }
    }

    void HandleDash()
    {
        currentVelocity = dashSpeed * currentMoveDirection;
    }

    void HandleVelocity()
    {
        currentVelocity = currentMoveDirection * currentSpeed;
    }

    void HandleSlideVelocity()
    {
        currentVelocity = (currentMoveDirection + GetSlideMoveDirection()) * currentSpeed;
    }

    void HandleGravity(float speed)
    {
        currentVelocity.y = speed;
        currentVelocity.y -= gravity * Time.deltaTime;
    }
    #endregion

    #region Queue Functions
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
    private void QueueSlide()
    {
        if (slide.wasPressedThisFrame && !slideQueued && !slideOnCooldown)
        {
            if(inputDirection.magnitude > 0)
            {
                slideQueued = true;
            }
        }
        if ((slide.wasReleasedThisFrame && slidingTimer > minSlideTime)
            || (!slide.isPressed && slidingTimer > minSlideTime))
        {
            slideQueued = false;
            slidingTimer = 0f;
            slideMomentum = 0f;
            maxSlideSpeedHit = true;
        }
        
    }
    #endregion

    #region Grounded
    void CheckGrounded()
    {
        
        RaycastHit hit;
        
        Ray ray = new Ray(transform.position, transform.up * -1);
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);


                                          //Half characters height + 0.1
        if (Physics.Raycast(ray, out hit, cc.height/2 + 0.1f, layerMask))
        {
            grounded = true;
            Debug.Log(hit.collider.name);
            
            if(hit.distance < cc.height/2)
            {
                //var characterPosition = transform.position;
                //characterPosition.y = cc.height;
                //transform.position = characterPosition;
                cc.Move(Vector3.up * 2 * Time.deltaTime);
            }
        }
        else
        {
            grounded = false;
            groundedTimer = 0;
        }
    }
    #endregion

    #region Dash Functions
    void CheckDash()
    {
        if (dashing)
        {
            return;
        }
        if (input.Player.Dash.triggered && numberOfDashesCurrent > 0)
        {
            dashing = true;
            //hookShotCancelled = true;
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
    #endregion

    #region Acceleration and Decelleration Functions
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
            
        }
        if (currentSpeed >= maxJumpStrafeSpeed)
        {
            currentSpeed -= strafeJumpDecelleration;
            if (currentSpeed - maxJumpStrafeSpeed <= 0.1)
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
        lastMoveDirection = currentVelocity;
        lastMoveDirection.y = 0;
        lastMoveDirection.Normalize();

        if (currentSpeed > 0)
        {
            currentSpeed -= deccelleration * Time.deltaTime;
            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
            }
        }
        currentVelocity = lastMoveDirection * currentSpeed;

    }
    #endregion

    #region SlideFunctions
    private void HandleMomentumSpeed ()
    {
        if (sliding)
        {
            if (!maxSlideSpeedHit)
            {
                slideMomentum += slideAccelerate * Time.deltaTime;
                if(slideMomentum >= maxSlideMomentum)
                {
                    slideMomentum = maxSlideMomentum;
                    maxSlideSpeedHit = true;
                    slideMomentum = 0;
                }
            }
            else
            {
                currentSpeed -= slideDeccelerate * Time.deltaTime;
                if(currentSpeed <= maxSpeed)
                {
                    CancelSlide();
                }
            }
            
        }
        else
        {
            slideMomentum = 0;
        }
    }

    void CancelSlide()
    {
        currentSpeed = maxSpeed;
        slideMomentum = 0;
        maxSlideSpeedHit = false;
        sliding = false;
        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        slidingTimer = 0f;
        slideOnCooldown = true;
    }
    void CancelSlideForHookShot()
    {
        slideMomentum = 0;
        maxSlideSpeedHit = false;
        sliding = false;
        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        slidingTimer = 0f;
    }

    #endregion

    #region Cooldown Functions
    private void HandleCooldownFunctions()
    {
        DashTimer();
        DashCooldowns();
        HookShotCooldowns();
        SlideCooldown();
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

    private void SlideCooldown()
    {
        if (slideOnCooldown)
        {
            slideCooldownTimer += Time.deltaTime;
            if(slideCooldownTimer >= slideCooldown)
            {
                slideOnCooldown = false;
                slideCooldownTimer = 0;
            }
        }
    }
    #endregion
}
