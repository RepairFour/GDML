using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class Controller : MonoBehaviour
{
    #region Inspector Variables
    [Header("Camera Variables")]
    public float cameraHeight = 1f;
    public float slideCameraHeight = 0.5f;
    [Space]
    [Header("Mouse sensitivity values")]
    public float xSensitivity;
    public float ySensitivity;
    
    public float yMin;
    public float yMax;
    [Space]
    [Header ("Character Movement Values")]
    public float maxSpeed;
    public float globalMaxSpeed;
    public float maxJumpStrafeSpeed;
    public float maxStrafeSpeed;
    public float accelleration;
    public float deccelleration;
    public float strafeAcceleration;
    public float strafeJumpDecelleration;
    public float airControlModifier;
    public float speedDropDirectionChange;
    [Space]
    [Header ("Dash Variables")]
    public float dashSpeed;
    public float dashTime;
    public int maxDashAmount;
    public float dashCooldown;
    [Space]
    [Header("Jump Variables")]
    public float jumpSpeed;
    public float gravity;
    public float groundedTime;
    public int maxJumpNumber = 2;
    public LayerMask layerMask;

    [Space]
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
    public float hookShotFloatTime = 0.25f;
    public LayerMask hookShotMask;
    public Transform hookShotHand;

   
    public float hookCooldown;
    [Space]
    [Header("Blink Strike Variable")]
    public float blinkStrikeRange;
    public float blinkAngleModifier = 0.7f;

    [Header ("Transforms")]
    public Transform cameraTransform;
    public Transform hookShotTransform;
    LineRenderer lr;

    [Header("UI Variables")]
    public ParticleSystem animeLines;
    public Image crossHair;
    public Slider dash1;
    public Slider dash2;


    [Header("Testing Enemy Marks Variables")]
    public Mark markedEnemy;
    #endregion

    #region Animation Variables
    [Header("Animation Variables")]
    public Animator animator;
    private bool slideTriggerSet = false;
    #endregion

    #region Debugging and Private Variables
    float rotationX;
    float rotationY;

    float groundedTimer;

    [Header("Debugging Variables")]
    [SerializeField] bool queueJump;
    [SerializeField] float jumpingTimer;
    [SerializeField] float minJumpingTimer = 0.01f;
    [SerializeField] int jumpNumber = 0;

    [SerializeField] Vector3 currentMoveDirection;
    [SerializeField] Vector3 lastMoveDirection;
    [SerializeField] Vector3 inputDirection;
    [SerializeField] Vector3 lastInputDirection;
    [SerializeField] bool airControl;

    [SerializeField] Vector3 currentVelocity;
    [SerializeField] float currentSpeed;
    [SerializeField] bool grounded;

    [SerializeField] bool dashing;
    [SerializeField] float dashingTimer;
    [SerializeField] float dashCooldownTimer;
    [SerializeField] int numberOfDashesCurrent;
    [SerializeField] bool dashNoMove;
    [SerializeField] Vector3 dashNoMoveDirection;

    [SerializeField] bool slideQueued;
    [SerializeField] bool sliding;
    [SerializeField] float slideMomentum;
    [SerializeField] float slidingTimer;
    [SerializeField] bool maxSlideSpeedHit;
    [SerializeField] bool slideOnCooldown;
    [SerializeField] float slideCooldownTimer;

    [SerializeField] bool floatAfterHookShot = false;
    [SerializeField] float floatAfterHookShotTimer = 0f;
    [SerializeField] bool hookShoting;
    [SerializeField] bool hookShotFiring;
    [SerializeField] bool hookShotMove;
    [SerializeField] Vector3 hookShotDirection;
    [SerializeField] float hookShotSize;
    [SerializeField] Vector3 hookHitPoint;
    [SerializeField] Vector3 momentum;
    [SerializeField] float hookCooldownTimer;
    [SerializeField] bool hookOnCooldown;

    [SerializeField] bool blinkStrikeActivated = false;
    

    [SerializeField] RaycastHit[] hit;
    bool crouch;
    PlayerMap input;

    CharacterController cc;

    private ButtonControl jump;
    private ButtonControl slide;
    private ButtonControl hook;
    private ButtonControl blinkStrike;
    #endregion

    #region UnityAnalytics
    [Header ("Unity Analytics")]
    [SerializeField] int jumps;
    [SerializeField] float timeSpentInHookShot;
    [SerializeField] float timeSpentDashing;
    [SerializeField] float timeSpentSliding;
    [SerializeField] float timeSpentInAir;
    [SerializeField] float timeSpentOnGround;
    [SerializeField] float timeSpentAlive;
    [SerializeField] int deathNumber;
    public bool sendAnalytics = false;
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
        blinkStrike = (ButtonControl)input.Player.Melee.controls[0];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        //cc.gameObject.transform.position = new Vector3(0, normalHeight, 0);

        lr = hookShotTransform.gameObject.GetComponent<LineRenderer>();

        timeSpentInHookShot = 0f;
        timeSpentDashing = 0f;
        timeSpentSliding = 0f;
        timeSpentInAir = 0f;
        timeSpentOnGround = 0f;
        jumps = 0;
        deathNumber = 0;
    }
    private void Update()
    {
        //Debug.Log(transform.position);
        
        if (blinkStrikeActivated)
        {
            HandleBlinkStrike();
            return;
        }
        timeSpentAlive += Time.deltaTime;
        CheckGrounded();
        if (grounded)
        {
            groundedTimer += Time.deltaTime;
        }
        else
        {
            jumpingTimer += Time.deltaTime;
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

        HandleBlinkStrikeInput();

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
            timeSpentOnGround += Time.deltaTime;
            GroundMove();
        }
        if (!grounded /*|| queueJump*/)
        {
            timeSpentInAir += Time.deltaTime;
            AirMove();
        }
       
        currentVelocity += momentum;

        cc.Move(currentVelocity * Time.deltaTime);
        

        if (sliding)
        {
            timeSpentSliding += Time.deltaTime;
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
        
        RunAnimations();

        
    }
    #endregion
    
    #region Animations
    void RunAnimations()
    {
        HandleWalkAnimation();
        HandleSlideAnimation();
    }

    void HandleWalkAnimation()
    {
        if (currentSpeed > 0)
        {
           // animator.SetFloat("Moving", currentSpeed / maxSpeed);
        }
        else
        {
           // animator.SetFloat("Moving", 0);
        }
    }
    void HandleSlideAnimation()
    {
        if (sliding && !slideTriggerSet)
        {
            //animator.SetTrigger("Sliding");
            slideTriggerSet = true;
        }
        
    }

    #endregion

    #region HookShot Functions
    private void CheckGrappleHook()
    {
        if (Physics.Raycast(hookShotHand.transform.position, cameraTransform.forward, out RaycastHit hit, hookShotDistance, hookShotMask))
        {
            if (hit.collider.gameObject.GetComponent<ObjectProperties>() != null)
            {
                if (hit.collider.gameObject.GetComponent<ObjectProperties>().grapplable)
                {
                    crossHair.gameObject.SetActive(true);
                }
                else
                {
                    crossHair.gameObject.SetActive(false);
                }
            }

            else
            {
                crossHair.gameObject.SetActive(false);
            }
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
        if(Physics.Raycast(hookShotHand.transform.position, cameraTransform.forward, out RaycastHit hit, hookShotDistance))
        {

            if (hit.collider.gameObject.GetComponent<ObjectProperties>() != null)
            {
                if (hit.collider.gameObject.GetComponent<ObjectProperties>().grapplable)
                {
                    hookHitPoint = hit.point;
                    Debug.Log(hit.collider.name);
                    hookShotFiring = true;
                    HookShotDirection();
                    hookShotSize = 0f;
                    hookShotTransform.LookAt(hookHitPoint);
                    
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
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
        floatAfterHookShot = true;
    }
    private void CancelHookShotMomentumWallKick()
    {
        hookShotDirection.Normalize();
        momentum = cameraTransform.forward * hookShotSpeed * momentumExtraSpeed;
        momentum.y = 0;
        //momentum.y = hookShotDirection.y * hookShotSpeed/yMomentumSc;
        currentVelocity.y = 0;
        CancelHookShot();
        hookOnCooldown = true;
        floatAfterHookShot = true;
    }
    private void HookShotMove()
    {
        timeSpentInHookShot += Time.deltaTime;
        HookShotDirection();
        currentVelocity = hookShotSpeed * hookShotDirection;
        hookShotTransform.LookAt(hookHitPoint);
        lr.SetPosition(0, hookShotTransform.position);
        lr.SetPosition(1, hookShotHand.position);

        if (Vector3.Distance(hookHitPoint, transform.position) < 2)
        {
            if (queueJump)
            {
                CancelHookShotMomentumWallKick();
                queueJump = false;
            }
            else{
                CancelHookShot();
            }
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
        if (Mathf.Abs(inputDirection.x) > 0 || Mathf.Abs(inputDirection.y) > 0)
        {
            lastInputDirection = inputDirection;
        }
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
        
        //if (currentMoveDirection.magnitude == 0)
        //{
        //    AirDecellerate();
        //    //momentumExtraSpeed = 0;
        //    momentum = Vector3.zero;
        //    slideMomentum = 0;
        //}

        float yspeed = currentVelocity.y;
        GetMoveDirection();
        
        if (((lastInputDirection.x + inputDirection.x == 0) && Mathf.Abs(inputDirection.x) > 0) 
            || ((lastInputDirection.y + inputDirection.y == 0) && Mathf.Abs(inputDirection.y) > 0))
        {
            Debug.Log(lastInputDirection + " " + inputDirection);
            Debug.Log("Activate Air Control");
            airControl = true;
        }

        if (dashing)
        {
            timeSpentDashing += Time.deltaTime;
            HandleDash();
            if (hookShotMove)
            {
                CancelHookShotMomentumWallKick();
            }
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
            AddAirControl();
            HandleVelocity();
            HandleGravity(yspeed);
            HandleJump();
            return;
        }

        if (Mathf.Abs(inputDirection.y) > 0 || Mathf.Abs(inputDirection.x) > 0)
        {
           
            Accelerate();
            AddAirControl();
            currentVelocity = currentMoveDirection * currentSpeed;
            //currentVelocity.y = 0;
            
        }
        else
        {
            AirDecellerate();
        }
        HandleGravity(yspeed);
        HandleJump();
    }
    void GroundMove()
    {
        if (currentMoveDirection.magnitude == 0 && !sliding)
        {
            Decellerate();
            //momentumExtraSpeed = 0;
            momentum = Vector3.zero;
            slideMomentum = 0;
        }
        float yspeed = currentVelocity.y;
        currentVelocity.y = 0;

        if (slideQueued)
        {
            sliding = true;
            slideQueued = false;
        }

        
        if (dashing)
        {
            timeSpentDashing += Time.deltaTime;
            HandleDash();
            if (hookShotMove)
            {
                CancelHookShotMomentumWallKick();
            }
            return;
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
            HandleGravity(yspeed);
            HandleJump();
            return;
        }
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

            HandleVelocity();
        }
        else
        {
            lastMoveDirection = currentVelocity;
            lastMoveDirection.Normalize();
            Decellerate();
            currentVelocity = lastMoveDirection * currentSpeed;

        }

        
        HandleGravity(0);
        HandleJump();
        //HandleGravity(0);
    }

    void HandleJump()
    {
        if (queueJump && jumpNumber < maxJumpNumber)
        {
            currentVelocity.y = jumpSpeed;
            jumpingTimer = 0;
            queueJump = false;
            grounded = false;
            groundedTimer = 0;
            CancelSlideForHookShot();
            jumps++;
            //animator.SetTrigger("Jump");
            
            jumpNumber++;
        }
    }

    void HandleDash()
    {
        if(momentum.magnitude > 0)
        {
            momentum = Vector3.zero;
        }
        
        if (dashNoMove)
        {
            currentVelocity = dashSpeed * dashNoMoveDirection;
        }
        else
        {
            currentVelocity = dashSpeed * currentMoveDirection;
        }
        animeLines.gameObject.SetActive(true);
        
    }

    void HandleVelocity()
    {
        currentVelocity = currentMoveDirection * currentSpeed;
    }
    void AddAirControl()
    {
        if (airControl)
        {
            currentSpeed *= airControlModifier;
            airControl = false;
        }
    }

    void HandleSlideVelocity()
    {
        currentVelocity = (currentMoveDirection + GetSlideMoveDirection()) * currentSpeed;
    }

    void HandleGravity(float speed)
    {
        if (!floatAfterHookShot)
        {
            currentVelocity.y = speed;
            currentVelocity.y -= gravity * Time.deltaTime;
        }
    }
    #endregion

    #region Queue Functions
    private void QueueJump()
    {
        if (input.Player.Jump.triggered /*jump.wasPressedThisFrame*/ &&
            !queueJump)
        {
            queueJump = true;
            if (!grounded && jumpNumber != maxJumpNumber)
            {
                jumpNumber = 1;
            }
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
            if(currentSpeed > 0)
            {
                slideQueued = true;
            }
        }
        
        if (slidingTimer > minSlideTime)
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
        Ray ray = new Ray(transform.position, transform.up * -1);

        hit = Physics.SphereCastAll(ray, 0.1f, cc.height / 2 + 0.1f, layerMask);

        if (hit.Length > 0 || sliding)
        {
            if (jumpingTimer >= minJumpingTimer)
            {
                foreach (RaycastHit c in hit)
                {
                    grounded = true;
                    //Debug.Log(hit.Length);
                    jumpingTimer = 0;
                    jumpNumber = 0;
                    //animator.SetTrigger("HitGround");
                    //animator.SetBool("Falling", false);
                }
            }
            
        }
        else
        {
            grounded = false;
            groundedTimer = 0;
            //animator.SetBool("Falling", true);
        }
        
        

        RaycastHit gHit;
        if (Physics.Raycast(ray, out gHit, cc.height / 2, layerMask))
        {
            if (gHit.distance < cc.height / 2)
            {
                cc.Move(Vector3.up * 2 * Time.deltaTime);
            }
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
        if (input.Player.Dash.triggered && numberOfDashesCurrent > 0 && inputDirection.magnitude > 0)
        {
            dashing = true;
            dashNoMove = false;
            //hookShotCancelled = true;
            numberOfDashesCurrent--;
            animeLines.gameObject.SetActive(true);
            //animator.SetTrigger("Dash");
        }
        else if(input.Player.Dash.triggered && numberOfDashesCurrent > 0 && inputDirection.magnitude == 0)
        {
            dashing = true;
            dashNoMove = true;
            dashNoMoveDirection = transform.forward;
            numberOfDashesCurrent--;
            animeLines.gameObject.SetActive(true);
            //animator.SetTrigger("Dash");
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
            animeLines.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Acceleration and Decelleration Functions
    void Accelerate()
    {
        if (Mathf.Abs(inputDirection.x) == 1)
        {
            if (lastInputDirection.x + inputDirection.x == 0)
            {
                currentSpeed = speedDropDirectionChange;
            }

        }
        if (Mathf.Abs(inputDirection.y) == 1)
        {
            if (lastInputDirection.y + inputDirection.y == 0)
            {
                currentSpeed = speedDropDirectionChange;
            }
        }
        //if (Vector3.Dot(lastInputDirection, inputDirection) < 1)
        //{
        //    currentSpeed = 10;
        //}

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

    private void AirDecellerate()
    {
        lastMoveDirection = currentVelocity;
        float yspeed = lastMoveDirection.y;
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
        currentVelocity.y = yspeed;
    }

    private void ChangeDirectionDecellerate()
    {

        
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
        slideTriggerSet = false;
    }
    void CancelSlideForHookShot()
    {
        slideMomentum = 0;
        maxSlideSpeedHit = false;
        sliding = false;
        slideTriggerSet = false;
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
    private void HookShotFloatCooldown()
    {
        if (floatAfterHookShot)
        {
            floatAfterHookShotTimer += Time.deltaTime;
            if(floatAfterHookShotTimer >= hookShotFloatTime)
            {
                floatAfterHookShotTimer = 0f;
                floatAfterHookShot = false;
            }
        }
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
        
        if(numberOfDashesCurrent < maxDashAmount)
        {
            dash1.gameObject.SetActive(true);
            dash2.gameObject.SetActive(true);
            if(numberOfDashesCurrent < 1)
            {
                dash1.value = dashCooldownTimer / dashCooldown;
                dash2.value = 0;
            }
            if(numberOfDashesCurrent >= 1 && numberOfDashesCurrent < 2)
            {
                dash1.value = 1;
                dash2.value = dashCooldownTimer / dashCooldown;
            }
        }
        if (numberOfDashesCurrent == maxDashAmount)
        {
            dash1.value = 1;
            dash2.value = 1;
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
        HookShotFloatCooldown();
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

    #region BlinkFunctions
    public void TeleportToPosition(Vector3 positionToTeleport)
    {
        //foreach(Collider c in cc.)
        var currentPosition = transform.position;
        var direction = positionToTeleport - currentPosition;
        direction.Normalize();
        Physics.IgnoreLayerCollision(7, 0, true);
        cc.Move(direction * 1000 * Time.deltaTime);

        if(Vector3.Distance(currentPosition, positionToTeleport) < 10)
        {
            Physics.IgnoreLayerCollision(7, 0, false);
            //cc.detectCollisions = true;
            blinkStrikeActivated = false;
            markedEnemy.BlinkMarkApplied();
            Debug.Log("Function Called");
        }


        //gameObject.transform.position = positionToTeleport;
    }
    public void HandleBlinkStrikeInput()
    {
        if (blinkStrike.wasPressedThisFrame && markedEnemy != null && InRangeForBlink())
        {
            blinkStrikeActivated = true;
        }
    }

    public void SetMark(Mark mark)
    {
        ChangeMark();
        markedEnemy = mark;
    }
    public void ChangeMark()
    {
       
    }
    public bool InRangeForBlink()
    {
        var distanceToEnemy = Vector3.Distance(markedEnemy.gameObject.transform.position, gameObject.transform.position);

        if(distanceToEnemy <= blinkStrikeRange)
        {
            float dot = Vector3.Dot(transform.forward, (markedEnemy.gameObject.transform.position - transform.position).normalized);
            if (dot >= blinkAngleModifier)
            {
                return true;
            }
        }
        
        return false;
    }

    public void HandleBlinkStrike()
    {
        TeleportToPosition(markedEnemy.transform.position);
    }

    #endregion

    #region UnityAnalytics Functions
    public void SendAnalytics(int deathNumber)
    {
        if (sendAnalytics)
        {
            Debug.Log("Analytics Sent");
            this.deathNumber = deathNumber;
            AnalyticsEvent.Custom("Times for Playthrough - Player Died", new Dictionary<string, object>
            {
                {"Number of Jumps", jumps },
                {"Time spent on ground", timeSpentOnGround},
                {"Time spent in air", timeSpentInAir },
                {"Time spent sliding", timeSpentSliding},
                {"Time spent dashing", timeSpentDashing},
                {"Time spent in hookshot", timeSpentInHookShot},
                {"Time spent alive", timeSpentAlive},
                {"Death number",  this.deathNumber}
            });
        }
    }
    private void OnApplicationQuit()
    { 
        if (sendAnalytics) {
            Debug.Log("Analytics Sent");
            AnalyticsEvent.Custom("Times for Playthrough - Player Quit", new Dictionary<string, object>
            {
                {"Number of Jumps", jumps},
                {"Time spent on ground", timeSpentOnGround},
                {"Time spent in air", timeSpentInAir },
                {"Time spent sliding", timeSpentSliding},
                {"Time spent dashing", timeSpentDashing},
                {"Time spent in hookshot", timeSpentInHookShot},
                {"Time spent alive", timeSpentAlive},
                {"Death number", this.deathNumber }
            });
        }
    }

#endregion

}
