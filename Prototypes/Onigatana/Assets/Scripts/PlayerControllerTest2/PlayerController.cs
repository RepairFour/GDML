using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

struct Command
{
    public float forwardMove;
    public float rightMove;
    public float upMove;
}

public class PlayerController : MonoBehaviour
{

    [Header("Player Setup")]
    public Transform cameraTransform;
    public float viewYoffset = 1.0f;

    [Header("Mouse stuff")]
    public float mouseSensitivityX = 20f;
    public float mouseSensitivityY = 20f;

    [Header("Gravity and Friction")]
    public float gravity = 20f;
    public float friction = 6f;

    [Header("Movement controls")]
   
    public PlayerMap playerMap;

    public float moveSpeed;
    public float moveAccleration;
    public float moveDecceleration;

    public float airAccleration;
    public float airDecceleration;
    public float airControl;

    public float strafeAcceleration;
    public float strafeSpeed;
    public float jumpSpeed;

    private CharacterController cc;

    //Camera rotations
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private Vector3 moveDirectionNormal = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    private float playerTopVelocity = 0.0f;

    private bool playerWishJump = false;

    private Command command;

    private ButtonControl jump;
    private InputAction move;
    private InputAction mouse;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cameraTransform.position = new Vector3(transform.position.x, transform.position.y + viewYoffset, transform.position.z);
        cc = GetComponent<CharacterController>();
        

        //jump = 
        playerMap = new PlayerMap();
        playerMap.Enable();
        jump = (ButtonControl)playerMap.Player.Jump.controls[0];
    }

    // Update is called once per frame
    void Update()
    {
        var mouseVector = playerMap.Player.Mouse.ReadValue<Vector2>();

        rotationX -= mouseVector.y;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().y;
        rotationY += mouseVector.x;//inputMap.FindAction("MouseLocation").ReadValue<Vector2>().x;

        //Debug.Log(rotationX + " " + rotationY);

        if (rotationX < -90)
        {
            rotationX = -90;
        }
        else if(rotationX > 90)
        {
            rotationX = 90;
        }

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        cameraTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        QueueJump();
        //Debug.Log(cc.isGrounded);
        if (cc.isGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }

        cc.Move(playerVelocity * Time.deltaTime);

        Vector3 udp = playerVelocity;
        udp.y = 0.0f;
        if(udp.magnitude > playerTopVelocity)
        {
            playerTopVelocity = udp.magnitude;
        }

        cameraTransform.position = new Vector3(transform.position.x, transform.position.y + viewYoffset, transform.position.z);
    }

    //Sets the direction of movement using the new Unity Input System
    //TODO: I HOPE THIS WORKS
    private void SetMovementDirection()
    {
        var movementVector = playerMap.Player.Move.ReadValue<Vector2>();
        command.forwardMove = movementVector.y;
        command.rightMove = movementVector.x;
    }

    //Queues up jump using the new Unity Input System
    //TODO: I HOPE THIS WORKS
    private void QueueJump()
    {
        
        //Debug.Log(inputMap.FindAction("Jump").ReadValue<float>());


        if (playerMap.Player.Jump.triggered /*jump.wasPressedThisFrame*/ && 
            !playerWishJump)
        {
            playerWishJump = true;
        }

        if (jump.wasReleasedThisFrame)
        {
            playerWishJump = false;
            
        }
    }

    private void AirMove()
    {
        Vector3 desiredDirection;
        float desiredVelocity = airAccleration;
        float accel;

        SetMovementDirection();

        //Calculate the direction the player wants to move 
        desiredDirection = new Vector3(command.rightMove, 0, command.forwardMove);
        //Debug.Log("AirMove");
        //Convert that direction to world space
        desiredDirection = transform.TransformDirection(desiredDirection);

        float desiredSpeed = desiredDirection.magnitude;
        desiredSpeed *= moveSpeed;

        desiredDirection.Normalize();
        moveDirectionNormal = desiredDirection;

        //CPM: Aircontrol
        float desiredSpeed2 = desiredSpeed;
        if(Vector3.Dot(playerVelocity, desiredDirection) < 0)
        {
            accel = airDecceleration;
        }
        else
        {
            accel = airAccleration;
        }

        //If the player is ONLY strafing left or right
        //TODO: NOT SURE IF THIS WILL WORK NOW
        if(command.forwardMove == 0 && command.rightMove != 0)
        {
            if(desiredSpeed > strafeSpeed)
            {
                desiredSpeed = strafeSpeed;
            }
            accel = strafeAcceleration;
        }

        Accelerate(desiredDirection, desiredSpeed, accel);
        if(airControl > 0)
        {
            AirControl(desiredDirection, desiredSpeed2);
        }

        playerVelocity.y -= gravity * Time.deltaTime;
    }

    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        float yspeed;
        float speed;
        float dot;
        float k = 32;

        if(Mathf.Abs(command.forwardMove) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
        {
            return;
        }

        yspeed = playerVelocity.y; // Capture our y velocityy
        playerVelocity.y = 0; // set it to 0

        speed = playerVelocity.magnitude; // we find out how fast we were going
        playerVelocity.Normalize(); //Normalise it for direction vector

        dot = Vector3.Dot(playerVelocity, wishdir); //float between -1 and 1 

        k *= airControl * dot * dot * Time.deltaTime;

        //Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

            playerVelocity.Normalize();
            moveDirectionNormal = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = yspeed;
        playerVelocity.z *= speed;

    }

    private void GroundMove()
    {
        Vector3 desiredDirection;

        //DO not apply friction if the player is queueing up the next jump
        if (!playerWishJump)
        {
            ApplyFriction(1.0f);
        }
        else
        {
            ApplyFriction(0);
        }

        SetMovementDirection();

        desiredDirection = new Vector3(command.rightMove, 0, command.forwardMove);
        // Debug.Log("Ground Move");
        desiredDirection = transform.TransformDirection(desiredDirection);
        desiredDirection.Normalize();
        //moveDirectionNormal = desiredDirection;

        var desiredspeed = desiredDirection.magnitude;
        desiredspeed *= moveSpeed;

        Accelerate(desiredDirection, desiredspeed, moveAccleration);

        //reset the gravity velocity
        playerVelocity.y = -gravity * Time.deltaTime;

        if (playerWishJump)
        {
            playerVelocity.y = jumpSpeed;
            playerWishJump = false;
        }

    }

    private void ApplyFriction(float t)
    {
        Vector3 vec = playerVelocity;
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        if (cc.isGrounded)
        {
            control = speed < moveDecceleration ? moveDecceleration : speed;
            drop = control * friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        //playerFriction = newspeed;

        if(newspeed < 0)
        {
            newspeed = 0;
        }
        if(newspeed > 0)
        {
            newspeed /= speed;
        }
        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;

    }

    private void Accelerate(Vector3 wishdirection, float wishspeed, float accelleration)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(playerVelocity, wishdirection);
        addspeed = wishspeed - currentspeed;
        if(addspeed <= 0)
        {
            return;
        }
        accelspeed = accelleration * Time.deltaTime * wishspeed;
        if(accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }
        playerVelocity.x += accelspeed * wishdirection.x;
        playerVelocity.z += accelspeed * wishdirection.z;

    }

}
