using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;

public class MainCharacterController : MonoBehaviour
{
    #region Inspector Variables
    [Header("Character Variables")]
    public float normalHeight; //Character Controller heights
    public float slideHeight;
    [Space]

    [Header("Animation Variables")]
    public Animator animator; //Character Animations
    public Animator leanAnimator; //Camera Animations
    [Space]

    [Header("Camera Variables")]
    public float cameraHeight = 1f;
    public float slideCameraHeight = 0.5f;
    public Transform cameraTransform; //Highest parent of main camera
    [Space]

    [Header("Mouse Variables")]
    public float xSensitivity;
    public float ySensitivity;
    public float yMin; //Locks the rotation up and down
    public float yMax;
    [Space]

    [Header("HUD Variables")]
    public ParticleSystem animeLines;
    public Image crossHair;
    public Slider dash1;
    public Slider dash2;
    [Space]

    [Header("Movement Variables")]
    public float walkingMaxSpeed; //Max speed when using WASD
    public float globalMaxSpeed; //Cap on all modifiers
    [Space]

    public float deccelleration;
    public float accelleration;
    public float airControlModifier; //Reduces the players control in the air 
    [Space]

    public float directionChangeSpeed; //Speed player is reduced to when moving in the oppisite direction8
    [Space]

    [Header("Strafe Movement Variables")]
    public float maxJumpStrafeSpeed; //Max speed at which player can strafe jump`
    public float strafeAcceleration; //How quickly player accelerates with strafejump 
    public float strafeJumpDeccelleration; //How quickly strafe jump speed falls off once player has stopped strafe jumping
    [Space]

    [Header("Jump Variables")]
    public int maxNumberOfJumps;
    public float jumpSpeed;
    public float minAirTimeForDoubleJump;
    public float minJumpingTime; //This stops the grounded check for X seconds
    [Space]

    [Header("Grounded Variables")]
    public float timeGroundedOffset; //Time player has to be grounded before they are considered to be actually grounded
    public LayerMask layerMask; //Layer mask that determines what the controller considers as ground
    [Space]

    [Header("Gravity Variables")]
    public float gravity;
    [Space]

    [Header("Slide Variables")]
    [Range (0, 1)] public float slideDirectionalScalar; //Determines how much left and right movement the player has in slide 
    public float slideDrag; 
    public float slideDeccelerate;
    public float maxSlideMomentum; 
    public float minSlideTime;
    public float slideCooldown;
    public float momentumDrag; //How quickly momentum drops off 
    [Space]

    [Header("Dash Variables")]
    public float dashSpeed;
    public float dashTime;
    public int maxDashAmount;
    public float dashCooldown;
    public bool dashNoMove;

    [Space]
    [Header("HookShot Variables")]
    public GrappleTargetting grappleSystem; //Targetting
    [Space]
    public LineRenderer lr; //Draws the grapple line
    
    [Space]
    public Transform hookShotTransform; //Where the line is drawn from 
    public Transform hookShotHand; //This may be Depreceated 

    [Space]
    public float hookShotThrowSpeed; //How fast the hookshot reaches its destination
    public float momentumExtraSpeed = 7f; //How much momentum is added at the end of the hookshot
    public float hookShotSpeed; //How quickly the player travels while hookshoting
    public float hookCooldown;
    [Space]
    public bool floatAfterHookShot = false; //If there is a small gravity cancel at the end of hookshot movement

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

    Vector3 currentMomentum; //Gets added to current velocity in some cases 

    float currentSlideMomentum; //Gets added to current speed in some cases

    float groundedTimer; //How long you have been grounded for
    bool airControl; //If air control is activated or not 
    
    RaycastHit[] hit; // Array used to store hit data from the grounded ray cast

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
    bool slideMomentumExpended;
    bool slideOnCooldown;
    float slidingTimer;
    float slideCooldownTimer;

    #endregion

    #region Input
    PlayerMap input;
    private ButtonControl jumpButton;
    private ButtonControl slideButton;
    #endregion

    #region DashVariables
    //bool dashNoMove;
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
    float leanIncrement = 0.05f; //Camera animation variable
    #endregion

    #region Blink Variables
    /*Blink is techinically a dash due to the way 
     * the character controller works */
    bool blinkStikeActivated = false;
    public bool isBlinkStrikeActivated { get => blinkStikeActivated; }
    
    float blinkStoppingDistance; //How far away from the positionToTeleport the controller will stop
    Vector3 positionToTeleport; //Intial target area of blink/teleport
    float blinkSpeed;
    #endregion

    #region Charge Attack Variables
    bool chargingStrongAttack;
    float chargeSlowDown; //Value from 0 - 1 that determines how much slower the player goes while chargin and attack
    #endregion

    #region HookShot Variables
    GameObject enemyStruck;
    
    Vector3 hookShotDirection;
    Vector3 hookHitPoint;
    Vector3 momentum;

    bool hookShotFiring;
    //bool hookShotMove;
    bool hookOnCooldown;

    float hookShotSize; //How far hookshot has travelled
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

        jumpButton = (ButtonControl)input.Player.Jump.controls[0];         //Captures the controls for various button clicks
        slideButton = (ButtonControl)input.Player.Slide.controls[0];       //Captures the controls for various button clicks
        

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

        // Rotation of the player and the camera
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

        //Check inputs this frame
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
            //TODO: Needs to be done as a lerp or animation instead 
            //Set slide heights
            cameraTransform.localPosition = new Vector3(0, slideCameraHeight, 0);
            cc.height = slideHeight;
        }
        
        if (currentMomentum.magnitude >= 0f)
        {
            //Apply drag to momentum
            currentMomentum -= currentMomentum * momentumDrag * Time.deltaTime;
            if (currentMomentum.magnitude < .01f)
            {
                currentMomentum = Vector3.zero;
            }
        }

        RunAnimations();

    }
    #endregion

    #region Movement Functions

    #region Direction Functions
    void UpdateMoveDirection()
    {
        //If we have input
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
        if (Mathf.Abs(inputS.y) > 0)//If the player is holding either w or s
        {
            //Cache the A and D movement 
            var slideDirection = new Vector3(inputS.x, 0, 0);
            //Turn that into a direction in world space
            slideDirection = transform.TransformDirection(slideDirection);
            /*Scale that vector by a number set in the inspector,
             * This determines how much of an affect the slideDirection will 
             * have on the current move direction*/
            return slideDirection * slideDirectionalScalar;
        }
        return Vector3.zero;
    }

    private void UpdateHookShotDirection()
    {
        hookShotDirection = (hookHitPoint - transform.position).normalized;
    }

    #endregion

    #region Ground/Air
    void GroundMove()
    {
        //if (currentMoveDirection.magnitude == 0 && movementState == MovementState.NORMAL)
        //{
        //    Deccellerate();
        //    currentMomentum = Vector3.zero;
        //    currentSlideMomentum = 0;
        //}
        float yspeed = currentVelocity.y; //Capture the y velocity for later
        currentVelocity.y = 0; //Negate gravity

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
                HandleGravity(yspeed); //Increases gravity so player sticks to ground
                HandleJump(); //Allows us to jump out of slide
                return;

            case MovementState.HOOKSHOT:
                CancelSlideForHookShot();
                HookShotMove();
                return;
                
            case MovementState.NORMAL:
                UpdateMoveDirection();
                if (Mathf.Abs(inputDirection.x) > 0.01f || Mathf.Abs(inputDirection.y) > 0.01f)
                {
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

                HandleGravity(0); //Basic level of gravity applied
                HandleJump();
                break;
        }
    }

    void AirMove()
    {


        float yspeed = currentVelocity.y;
        UpdateMoveDirection();

        //This needs to be revisited if we ever add controller support
        //Need to look at possibly doing this as dot product calculation
        if (((lastInputDirection.x + inputDirection.x == 0) && Mathf.Abs(inputDirection.x) > 0)
            || ((lastInputDirection.y + inputDirection.y == 0) && Mathf.Abs(inputDirection.y) > 0))
        {
            airControl = true;
        }

        switch (movementState)
        {
            case MovementState.NORMAL:
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
                }

                else if (Mathf.Abs(inputDirection.y) > 0 || Mathf.Abs(inputDirection.x) > 0)
                {
                    Accelerate();
                    AddAirControl();
                    HandleVelocity();
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
                break;
               
            case MovementState.HOOKSHOT:
                {
                    HookShotMove();
                }
                break;
        }
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
            //Slows direction change while in the airborne
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
            if (!slideMomentumExpended)
            {
                currentSlideMomentum -= slideDrag * Time.deltaTime;
                if (currentSlideMomentum <= 0)
                {
                    currentSlideMomentum = 0;
                    slideMomentumExpended = true;
                    //CancelSlide();
                }
            }
            else //After slide momentum is used slow player down to walking speed
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
        if (currentMomentum.magnitude > 0) //Reset Momentum
        {
            currentMomentum = Vector3.zero;
        }
        currentVelocity = dashSpeed * currentMoveDirection; //Apply dash
        animeLines.gameObject.SetActive(true);
    }
    #endregion

    #region BlinkFunctions

    void HandleBlinkStrike(Vector3 positionToBlink)
    {
        TeleportToPosition(positionToBlink);
    }

    private void TeleportToPosition(Vector3 positionToTeleport)
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
            currentVelocity *= chargeSlowDown;
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
        UpdateHookShotDirection();
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

        //Allows the user to exceed maxWalkingSpeed if they are strafe jumping otherwise cap
        //there speed at the max walking speed
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
        slideMomentumExpended = false;
        movementState = MovementState.NORMAL;
        cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
        cc.height = normalHeight;
        
        slideOnCooldown = true;
        //slideTriggerSet = false;
    }

    private void CancelHookShot()
    {
        movementState = MovementState.NORMAL;
        hookShotFiring = false;
        hookShotSize = 0;
        currentVelocity.y = 0;
        hookShotHand.position = hookShotTransform.position;
        hookShotTransform.gameObject.SetActive(false);
        enemyStruck = null;
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
        slideMomentumExpended = false;
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
                /*This is if the player walks off of an object 
                 * it is the amount of time they have to input a jump
                 * before they forfiet the double jump */
                if (timeInAir >= minAirTimeForDoubleJump)
                {
                    jumpNumber = 1;
                }
            }
        }

        if (jumpButton.wasReleasedThisFrame)
        {
            queueJump = false;
        }
    }
    private void QueueSlide()
    {
        if (slideButton.wasPressedThisFrame && !queueSlide && !slideOnCooldown)
        {
            if (currentSpeed > 0)
            {
                queueSlide = true;
                currentSlideMomentum = maxSlideMomentum;
            }
        }

        if (slidingTimer > minSlideTime && slideButton.wasReleasedThisFrame)
        {
            queueSlide = false;
            slidingTimer = 0f;
            currentSlideMomentum = 0f;
            slideMomentumExpended = true;
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
            currentNumberOfDashes--;
            animeLines.gameObject.SetActive(true);
            //animator.SetTrigger("Dash");
        }
        else if (input.Player.Dash.triggered && currentNumberOfDashes > 0 && inputDirection.magnitude == 0)
        {
            movementState = MovementState.DASHING;
            dashNoMove = true;
            //dashNoMoveDirection = transform.forward;
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
            UpdateHookShotDirection();
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
