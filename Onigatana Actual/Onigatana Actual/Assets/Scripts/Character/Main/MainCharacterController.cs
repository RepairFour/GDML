using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;

public class MainCharacterController : MonoBehaviour
{
    #region Inspector Variables
    [Header("Character Variables")]
    public float normalHeight;
    public float slideHeight;
    [Space]

    [Header("Animation Variables")]
    public Animator animator;
    public Animator leanAnimator;
    [Space]

    [Header("Camera Variables")]
    public float cameraHeight = 1f;
    public float slideCameraHeight = 0.5f;
    public Transform cameraTransform;
    [Space]

    [Header("Mouse sensitivity values")]
    public float xSensitivity;
    public float ySensitivity;
    public float yMin;
    public float yMax;
    [Space]

    [Header("UI Variables")]
    public ParticleSystem animeLines;
    public Image crossHair;
    public Slider dash1;
    public Slider dash2;
    [Space]

    [Header("Movement Variables")]
    public float walkingMaxSpeed;
    public float globalMaxSpeed;
    [Space]

    public float deccelleration;
    public float accelleration;
    public float airControlModifier;
    [Space]

    public float directionChangeSpeed;
    [Space]
    
    [Header("Strafe Movement Variables")]
    public float maxJumpStrafeSpeed;
    public float strafeAcceleration;
    public float strafeJumpDeccelleration;
    [Space]
    
    [Header("Jump Variables")]
    public int maxNumberOfJumps;
    public float jumpSpeed;
    public float minAirTimeForDoubleJump;
    public float minJumpingTime;
    [Space]
    
    [Header("Grounded Variables")]
    public float timeGroundedOffset;
    public LayerMask layerMask;
    [Space]

    [Header("Gravity Variables")]
    public float gravity;
    [Space]

    [Header("Slide Variables")]
    public float slideDirectionalScalar;
    public float slideAccelerate;
    public float slideDeccelerate;
    public float maxSlideMomentum;
    public float minSlideTime;
    public float slideCooldown;
    public float momentumDrag;
    [Space]

    [Header("Dash Variables")]
    public float dashSpeed;
    public float dashTime;
    public int maxDashAmount;
    public float dashCooldown;

    [Space]
    [Header("HookShot Variables")]
    public GrappleTargetting grappleSystem;
    [Space]
    public LineRenderer lr;

    [Space]
    public Transform hookShotTransform;
    public Transform hookShotHand;

    [Space]
    public float hookShotThrowSpeed;
    public float momentumExtraSpeed = 7f;
    public float hookShotSpeed;
    public float hookCooldown;
    [Space]
    public bool floatAfterHookShot = false;

    #endregion

    #region PrivateVariables
    CharacterController cc;
    
    float rotationX;
    float rotationY;


    Vector3 inputDirection;
    Vector3 lastInputDirection;
    Vector3 currentMoveDirection;
    Vector3 lastMoveDirection;

    Vector3 currentVelocity;
    float currentSpeed;

    Vector3 currentMomentum;

    float currentSlideMomentum;

    float groundedTimer;
    bool airControl;
    
    RaycastHit[] hit;

    #region Jump Variables
    float jumpingTimer;
    int jumpNumber;
    float timeInAir;
    #endregion

    #region QueueInputBooleans
    bool queueJump;
    bool queueSlide;
    #endregion

    #region Slide Variables
    bool maxSlideSpeedHit;
    bool slideOnCooldown;
    float slidingTimer;
    float slideCooldownTimer;

    #endregion

    #region Input
    PlayerMap input;
    private ButtonControl jump;
    private ButtonControl slide;
    private ButtonControl hook;
    private ButtonControl blinkStrike;
    #endregion

    #region DashVariables
    bool dashNoMove;
    Vector3 dashNoMoveDirection;
    int currentNumberOfDashes;
    float dashingTimer;
    float dashCooldownTimer;
    #endregion

    #region PlayerMovementState
    enum MovementState { NORMAL, DASHING, SLIDING, HOOKSHOT}
    MovementState movementState;
    enum GroundedState { GROUNDED, INAIR}
    GroundedState groundedState;
    #endregion
    
    #region AnimationVariables
    private bool slideTriggerSet = false;
    float leanIncrement = 0.05f;
    #endregion

    #region Blink Variables
    bool blinkStikeActivated = false;
    float blinkStoppingDistance;
    public bool isBlinkStrikeActivated { get => blinkStikeActivated; }
    Vector3 positionToTeleport;
    float blinkSpeed;

    #endregion

    #region Charge Attack Variables
    bool chargingStrongAttack;
    float chargeSlowDown;
    #endregion

    #region HookShot Variables
    GameObject enemyStruck;
    
    Vector3 hookShotDirection;
    Vector3 hookHitPoint;
    Vector3 momentum;

    bool hookShotFiring;
    //bool hookShotMove;
    bool hookOnCooldown;

    float hookShotSize;
    float hookCooldownTimer;
    
    #endregion

    #endregion

    #region UnityFunctions
    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.instance.input;
        cc = GetComponent<CharacterController>();
        currentNumberOfDashes = maxDashAmount;

        jump = (ButtonControl)input.Player.Jump.controls[0];         //Captures the controls for various button clicks
        hook = (ButtonControl)input.Player.Hook.controls[0];         //Captures the controls for various button clicks
        slide = (ButtonControl)input.Player.Slide.controls[0];       //Captures the controls for various button clicks
        blinkStrike = (ButtonControl)input.Player.Melee.controls[0]; //Captures the controls for various button clicks

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        movementState = MovementState.NORMAL;
        groundedState = GroundedState.INAIR;
    }

    // Update is called once per frame
    void Update()
    {
        if (blinkStikeActivated)
        {
            HandleBlinkStrike(positionToTeleport);
        }

        CheckGrounded();

        switch (groundedState)
        {
            case GroundedState.GROUNDED:
                groundedTimer += Time.deltaTime;
                break;
            case GroundedState.INAIR:
                jumpingTimer += Time.deltaTime;
                break;
        }

        var mouseVector = input.Player.Mouse.ReadValue<Vector2>();

        rotationX -= mouseVector.y * Time.deltaTime * xSensitivity;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().y;
        rotationY += mouseVector.x * Time.deltaTime * ySensitivity;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().x;

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

        QueueHookShot();
        QueueJump();
        QueueSlide();
        QueueDash();
        HandleCooldowns();

        if (hookShotFiring)
        {
            FiringHookShot();
        }


        switch (groundedState)
        {
            case GroundedState.GROUNDED:
                GroundMove();
                break;
            case GroundedState.INAIR:
                AirMove();
                break;
        }
        currentVelocity += currentMomentum;

        cc.Move(currentVelocity * Time.deltaTime);

        if(movementState == MovementState.SLIDING)
        {
            cameraTransform.localPosition = new Vector3(0, slideCameraHeight, 0);
            cc.height = slideHeight;
        }
        
        if (currentMomentum.magnitude >= 0f)
        {

            currentMomentum -= currentMomentum * momentumDrag * Time.deltaTime;
            if (currentMomentum.magnitude < .01f)
            {
                currentMomentum = Vector3.zero;
            }
        }

        RunAnimations();

    }
    #endregion

    #region Movement Functions8

    #region Direction Functions
    void GetMoveDirection()
    {
        //IF we have input
        if (Mathf.Abs(inputDirection.x) > 0 || Mathf.Abs(inputDirection.y) > 0)
        {
            lastInputDirection = inputDirection; 
        }
        inputDirection = input.Player.Move.ReadValue<Vector2>();
        currentMoveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        //Transform the direction to world space 
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

    private void HookShotDirection()
    {
        hookShotDirection = (hookHitPoint - transform.position).normalized;
    }

    #endregion

    #region Ground/Air
    void GroundMove()
    {
        if (currentMoveDirection.magnitude == 0 && movementState == MovementState.NORMAL)
        {
            Deccellerate();
            currentMomentum = Vector3.zero;
            currentSlideMomentum = 0;
        }
        float yspeed = currentVelocity.y;
        currentVelocity.y = 0;

        if (queueSlide)
        {
            movementState = MovementState.SLIDING;
            queueSlide = false;
        }

        switch (movementState)
        {
            case MovementState.DASHING:
                HandleDash();
                if (movementState == MovementState.HOOKSHOT)
                {
                    CancelHookShotMomentumWallKick();
                }
                return;
                
            case MovementState.SLIDING:
               
                currentSpeed = walkingMaxSpeed + currentSlideMomentum;
                HandleSlideVelocity();
                HandleMomentumSpeed();
                HandleGravity(yspeed);
                HandleJump();
                return;

            case MovementState.HOOKSHOT:
                CancelSlideForHookShot();
                HookShotMove();
                return;
                
            case MovementState.NORMAL:
                GetMoveDirection();
                if (Mathf.Abs(inputDirection.x) > 0.01f || Mathf.Abs(inputDirection.y) > 0.01f)
                {
                    //if (Mathf.Abs(inputDirection.x) > 0 && Mathf.Abs(inputDirection.y) > 0)
                    //{
                    //    StrafeAccelerate();
                    //}
                    Accelerate();
                    HandleVelocity();
                }

                else
                {
                    lastMoveDirection = currentVelocity;
                    lastMoveDirection.Normalize();
                    Deccellerate();
                    currentVelocity = lastMoveDirection * currentSpeed;
                }

                HandleChargingAttack();

                HandleGravity(0);
                HandleJump();
                break;
        }
    }

    void AirMove()
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

        /* Input is read in on the x and y axis and stored in the variable inputDirection
         * it is converted to x and z and then stored in moveDirection which is then transform
         * into the objects world space axis. Input direction is used here to make sure the 
         * player is strafing in the air. If they are they go faster. This emulates strafe jumping 
         * to a certain extent */
        if (((lastInputDirection.x + inputDirection.x == 0) && Mathf.Abs(inputDirection.x) > 0)
            || ((lastInputDirection.y + inputDirection.y == 0) && Mathf.Abs(inputDirection.y) > 0))
        {
            Debug.Log(lastInputDirection + " " + inputDirection);
            Debug.Log("Activate Air Control");
            airControl = true;
        }

        switch (movementState)
        {
            case MovementState.NORMAL:
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
                break;
            case MovementState.DASHING:
                HandleDash();
                if (movementState == MovementState.HOOKSHOT)
                {
                    CancelHookShotMomentumWallKick();
                }
                return;
               
            case MovementState.HOOKSHOT:
                {
                    HookShotMove();
                }
                return;
        }

        

        //if (hookShotMove)
        //{
        //    HookShotMove();
        //    return;
        //}
        //if (enemyStruck != null && !hookShotFiring)
        //{
        //    HookShotWhip();
        //}
    }

    #endregion

    #region Velocity & Gravity Functions
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

    void AddAirControl()
    {
        if (airControl)
        {
            currentSpeed *= airControlModifier;
            airControl = false;
        }
    }
    #endregion

    #region Jump Functions
    void HandleJump()
    {
        if (queueJump && jumpNumber < maxNumberOfJumps)
        {
            currentVelocity.y = jumpSpeed;
            
            queueJump = false;
            groundedState = GroundedState.INAIR;
            groundedTimer = 0;
            //CancelSlideForHookShot();
            //CancelSlide();
            //animator.SetTrigger("Jump");

            jumpNumber++;
        }
    }

    #endregion
    
    #region SlideFunctions
    private void HandleMomentumSpeed()
    {
        if (movementState == MovementState.SLIDING)
        {
            if (!maxSlideSpeedHit)
            {
                currentSlideMomentum -= slideAccelerate * Time.deltaTime;
                //if(slideMomentum >= maxSlideMomentum)
                //{
                //    slideMomentum = maxSlideMomentum;
                //    maxSlideSpeedHit = true;
                //    slideMomentum = 0;
                //}
                if (currentSlideMomentum <= 0)
                {
                    currentSlideMomentum = 0;
                    maxSlideSpeedHit = true;
                    CancelSlide();
                    //slideMomentum = 0;
                }
            }
            else
            {
                currentSpeed -= slideDeccelerate * Time.deltaTime;
                if (currentSpeed <= walkingMaxSpeed)
                {
                    CancelSlide();
                }
            }

        }
        else
        {
            currentSlideMomentum = 0;
        }
    }
    #endregion

    #region DashFunctions
    void HandleDash()
    {
        if (currentMomentum.magnitude > 0)
        {
            currentMomentum = Vector3.zero;
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


    #endregion

    #region BlinkFunctions

    public void HandleBlinkStrike(Vector3 positionToBlink)
    {
        TeleportToPosition(positionToBlink);
    }

    public void TeleportToPosition(Vector3 positionToTeleport)
    {
        //foreach(Collider c in cc.)
        var currentPosition = transform.position;
        var direction = positionToTeleport - currentPosition;
        direction.Normalize();
        Physics.IgnoreLayerCollision(7, 0, true);
        cc.Move(direction * blinkSpeed * Time.deltaTime);
        if (Vector3.Distance(currentPosition, positionToTeleport) < blinkStoppingDistance)
        {
            Physics.IgnoreLayerCollision(7, 0, false);
            //cc.detectCollisions = true;
            blinkStikeActivated = false;
            Debug.Log("Function Called");
        }

    }

    public void BlinkToPosition(Vector3 positionToBlink, float speed, float stoppingDistance)
    {
        blinkStikeActivated = true;
        positionToTeleport = positionToBlink;
        blinkSpeed = speed;
        blinkStoppingDistance = stoppingDistance;
    }


    #endregion

    #region Charge Attack Functions
    public void ToggleChargingAttack(float slowdown, bool toggle)
    {
        chargingStrongAttack = toggle;
        chargeSlowDown = slowdown;
    }

    void HandleChargingAttack()
    {
        if (chargingStrongAttack)
        {
            currentVelocity /= chargeSlowDown;
        }
    }

    #endregion

    #region HookShot Functions
    
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
        if (hookShotSize >= Vector3.Distance(hookShotTransform.position, hookHitPoint))
        {
            movementState = MovementState.HOOKSHOT;
            //hookShotMove = true;
            hookShotFiring = false;
            if (enemyStruck != null)
            {
                movementState = MovementState.NORMAL;
            }
            //hookShotHand.position = hookHitPoint;
        }

        lr.SetPosition(0, hookShotTransform.position);
        lr.SetPosition(1, hookShotHand.position);

    }

    private void HookShotMove()
    {
        HookShotDirection();
        currentVelocity = hookShotSpeed * hookShotDirection;
        hookShotTransform.LookAt(hookHitPoint);
        lr.SetPosition(0, hookShotTransform.position);
        lr.SetPosition(1, hookShotHand.position);

        if (Vector3.Distance(hookHitPoint, transform.position) < 5)
        {
            if (queueJump)
            {
                CancelHookShot();
                grappleSystem.currentTargettedObject.GetComponent<Outline>().OutlineWidth = 0f;
                grappleSystem.currentTargettedObject = null;
                grappleSystem.StartGrapple = false;
                //CancelHookShotMomentumWallKick();
                queueJump = false;
            }
            else
            {
                CancelHookShot();
                grappleSystem.currentTargettedObject.GetComponent<Outline>().OutlineWidth = 0f;
                grappleSystem.currentTargettedObject = null;
                grappleSystem.StartGrapple = false;

            }
        }
    }
    #endregion

    #region Accelleration/Deccelleration Functions
    void Accelerate()
    {
        //If they change direction to the exact oppisite direction 
        //their speed gets set to directionChangeSpeed
        // they will then continue to accellerate
        if (Mathf.Abs(inputDirection.x) == 1)
        {
            if (lastInputDirection.x + inputDirection.x == 0)
            {
                currentSpeed = directionChangeSpeed;
            }

        }
        if (Mathf.Abs(inputDirection.y) == 1)
        {
            if (lastInputDirection.y + inputDirection.y == 0)
            {
                currentSpeed = directionChangeSpeed;
            }
        }


        //if (Vector3.Dot(lastInputDirection, inputDirection) < 1)
        //{
        //    currentSpeed = 10;
        //}

        if (currentSpeed < walkingMaxSpeed)
        {
            currentSpeed += accelleration * Time.deltaTime;
        }

        if (currentSpeed >= walkingMaxSpeed && groundedTimer >= timeGroundedOffset)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkingMaxSpeed, strafeJumpDeccelleration * Time.deltaTime);
            if (Mathf.Abs(walkingMaxSpeed - currentSpeed) < 0.01)
            {
                currentSpeed = walkingMaxSpeed;
            }
        }
    }

    void Deccellerate()
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

    void JumpStrafeAccelerate()
    {
        if (currentSpeed < maxJumpStrafeSpeed)
        {
            currentSpeed += strafeAcceleration * Time.deltaTime;

        }
        if (currentSpeed >= maxJumpStrafeSpeed)
        {
            currentSpeed -= strafeJumpDeccelleration;
            if (currentSpeed - maxJumpStrafeSpeed <= 0.1)
            {
                currentSpeed = maxJumpStrafeSpeed;
            }
        }
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


    #endregion

    #region CancelFunctions
    void CancelSlide()
    {
        currentSpeed = walkingMaxSpeed;
        currentSlideMomentum = 0;
        maxSlideSpeedHit = false;
        movementState = MovementState.NORMAL;
        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        
        slideOnCooldown = true;
        slideTriggerSet = false;
    }

    private void CancelHookShot()
    {
        movementState = MovementState.NORMAL;
        //hookShotMove = false;
        hookShotFiring = false;
        hookShotSize = 0;
        currentVelocity.y = 0;
        //hookShotTransform.localScale = new Vector3(1, 1, hookShotSize);
        hookShotHand.position = hookShotTransform.position;
        hookShotTransform.gameObject.SetActive(false);
        enemyStruck = null;
        //lr.positionCount = 0;
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
    
    void CancelSlideForHookShot()
    {
        currentSlideMomentum = 0;
        maxSlideSpeedHit = false;
        movementState = MovementState.HOOKSHOT;
        slideTriggerSet = false;
        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        slidingTimer = 0f;
    }

    #endregion

    #endregion

    #region Input Functions

    private void QueueJump()
    {
        if (input.Player.Jump.triggered /*jump.wasPressedThisFrame*/ &&
            !queueJump)
        {
            queueJump = true;
            if (groundedState != GroundedState.GROUNDED && jumpNumber != maxNumberOfJumps)
            {
                if (timeInAir >= minAirTimeForDoubleJump)
                {
                    jumpNumber = 1;
                }
            }
        }

        if (jump.wasReleasedThisFrame)
        {
            queueJump = false;
        }
    }
    private void QueueSlide()
    {
        if (slide.wasPressedThisFrame && !queueSlide && !slideOnCooldown)
        {
            if (currentSpeed > 0)
            {
                queueSlide = true;
                currentSlideMomentum = maxSlideMomentum;
            }
        }

        if (slidingTimer > minSlideTime)
        {
            queueSlide = false;
            slidingTimer = 0f;
            currentSlideMomentum = 0f;
            maxSlideSpeedHit = true;
        }

    }

    void QueueDash()
    {
        if (movementState == MovementState.DASHING)
        {
            return;
        }
        if (input.Player.Dash.triggered && currentNumberOfDashes > 0 && inputDirection.magnitude > 0)
        {
            movementState = MovementState.DASHING;
            dashNoMove = false;
            //hookShotCancelled = true;
            currentNumberOfDashes--;
            animeLines.gameObject.SetActive(true);
            //animator.SetTrigger("Dash");
        }
        else if (input.Player.Dash.triggered && currentNumberOfDashes > 0 && inputDirection.magnitude == 0)
        {
            movementState = MovementState.DASHING;
            dashNoMove = true;
            dashNoMoveDirection = transform.forward;
            currentNumberOfDashes--;
            animeLines.gameObject.SetActive(true);
            //animator.SetTrigger("Dash");
        }

    }

    void QueueHookShot()
    {
        if (grappleSystem.StartGrapple == true)
        {
            hookHitPoint = grappleSystem.currentTargettedObject.transform.position;
            //Debug.Log(hit.collider.name);
            hookShotFiring = true;
            HookShotDirection();
            hookShotSize = 0f;
            hookShotTransform.LookAt(hookHitPoint);
        }
    }


    #endregion

    #region CooldownFunctions

    void DashTimer()
    {
        if (movementState != MovementState.DASHING)
        {
            return;
        }
        dashingTimer += Time.deltaTime;
        if (dashingTimer >= dashTime)
        {
            movementState = MovementState.NORMAL;
            dashingTimer = 0f;
            animeLines.gameObject.SetActive(false);
        }
    }
    
    private void DashCooldowns()
    {
        if (currentNumberOfDashes < maxDashAmount)
        {
            dashCooldownTimer += Time.deltaTime;
            if (dashCooldownTimer >= dashCooldown)
            {
                currentNumberOfDashes++;
                dashCooldownTimer = 0;
            }
        }

        if (currentNumberOfDashes < maxDashAmount)
        {
            dash1.gameObject.SetActive(true);
            dash2.gameObject.SetActive(true);
            if (currentNumberOfDashes < 1)
            {
                dash1.value = dashCooldownTimer / dashCooldown;
                dash2.value = 0;
            }
            if (currentNumberOfDashes >= 1 && currentNumberOfDashes < 2)
            {
                dash1.value = 1;
                dash2.value = dashCooldownTimer / dashCooldown;
            }
        }
        if (currentNumberOfDashes == maxDashAmount)
        {
            dash1.value = 1;
            dash2.value = 1;
        }

    }

    private void SlideCooldown()
    {
        if (slideOnCooldown)
        {
            slideCooldownTimer += Time.deltaTime;
            if (slideCooldownTimer >= slideCooldown)
            {
                slideOnCooldown = false;
                slideCooldownTimer = 0;
            }
        }
    }

    private void HookShotCooldowns()
    {
        if (hookOnCooldown)
        {
            hookCooldownTimer += Time.deltaTime;
            if (hookCooldownTimer >= hookCooldown)
            {
                hookOnCooldown = false;
                hookCooldownTimer = 0;
            }
        }
        //HookShotFloatCooldown();
    }

    private void HandleCooldowns()
    {
        DashTimer();
        DashCooldowns();
        HookShotCooldowns();
        SlideCooldown();
    }

    #endregion

    #region Grounded Functions
    void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, transform.up * -1);

        hit = Physics.SphereCastAll(ray, 0.5f * cc.height / normalHeight, cc.height / 2 + 0.1f, layerMask);

        if (hit.Length > 0)
        {
            if (jumpingTimer >= minJumpingTime)
            {
                foreach (RaycastHit c in hit)
                {
                    groundedState = GroundedState.GROUNDED;
                    //Debug.Log(hit.Length);
                    jumpingTimer = 0;
                    jumpNumber = 0;
                    //animator.SetTrigger("HitGround");
                    //animator.SetBool("Falling", false);
                    timeInAir = 0;
                }
            }

        }
        else
        {
            if (movementState != MovementState.SLIDING)
            {
                groundedState = GroundedState.INAIR;
                
                groundedTimer = 0;
                timeInAir += Time.deltaTime;
            }
            //animator.SetBool("Falling", true);
        }

    }


    #endregion

    #region AnimationFunctions
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
            if (inputDirection.x > 0)
            {
                leanAnimator.SetFloat("RightLean", leanAnimator.GetFloat("RightLean") + leanIncrement);
            }
            else if (inputDirection.x < 0)
            {
                leanAnimator.SetFloat("RightLean", leanAnimator.GetFloat("RightLean") - leanIncrement);
            }
            else
            {
                if (leanAnimator.GetFloat("RightLean") > 0)
                {
                    leanAnimator.SetFloat("RightLean", leanAnimator.GetFloat("RightLean") - leanIncrement);
                }
                else if (leanAnimator.GetFloat("RightLean") < 0)
                {
                    leanAnimator.SetFloat("RightLean", leanAnimator.GetFloat("RightLean") + leanIncrement);
                }
                if (Mathf.Abs(leanAnimator.GetFloat("RightLean")) <= leanIncrement)
                {
                    leanAnimator.SetFloat("RightLean", 0);
                }
            }
            if (leanAnimator.GetFloat("RightLean") > 1)
            {
                leanAnimator.SetFloat("RightLean", 1);
            }
            else if (leanAnimator.GetFloat("RightLean") < -1)
            {
                leanAnimator.SetFloat("RightLean", -1);
            }
        }
        else
        {
            if (leanAnimator.GetFloat("RightLean") > 0)
            {
                leanAnimator.SetFloat("RightLean", leanAnimator.GetFloat("RightLean") - leanIncrement);
            }
            else if (leanAnimator.GetFloat("RightLean") < 0)
            {
                leanAnimator.SetFloat("RightLean", leanAnimator.GetFloat("RightLean") + leanIncrement);
            }
            if (Mathf.Abs(leanAnimator.GetFloat("RightLean")) <= leanIncrement)
            {
                leanAnimator.SetFloat("RightLean", 0);
            }
            // animator.SetFloat("Moving", 0);
        }
    }
    void HandleSlideAnimation()
    {
        if (movementState == MovementState.SLIDING && !slideTriggerSet)
        {
            //animator.SetTrigger("Sliding");
            slideTriggerSet = true;
        }

    }
    #endregion
}
