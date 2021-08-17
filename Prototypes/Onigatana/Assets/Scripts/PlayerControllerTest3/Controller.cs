using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Controller : MonoBehaviour
{
    public float friction;

    public float moveSpeed;
    public float moveAcceleration;
    public float moveDecceleration;

    public float jumpSpeed;
    public float gravity = 20f;

    public PlayerMap input;
    ButtonControl jump;

    Vector3 movementDirection;
    Vector3 currentVelocity;

    //Camera
    float rotationX;
    float rotationY;

    public Transform cameraTransform;

    bool jumpQueued = false;

    CharacterController cc;
    
    
    // Start is called before the first frame update
    void Start()
    {
        currentVelocity = Vector3.zero;
        cc = GetComponent<CharacterController>();
        input = new PlayerMap();
        input.Enable();
        jump = (ButtonControl)input.Player.Jump.controls[0];
    }

    // Update is called once per frame
    void Update()
    {
        var mouseVector = input.Player.Mouse.ReadValue<Vector2>();
        var inputDirection = input.Player.Move.ReadValue<Vector2>();
        movementDirection = new Vector3(inputDirection.x, 0, inputDirection.y);

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

        GroundMove();

        cc.Move(currentVelocity * Time.deltaTime);
    }

    void SetMovementDirection()
    {
        var temp = input.Player.Move.ReadValue<Vector2>();
        movementDirection = new Vector3(temp.x, 0, temp.y);
    }

    private void ApplyFriction(float t)
    {
        Vector3 vec = currentVelocity;
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

        if (newspeed < 0)
        {
            newspeed = 0;
        }
        if (newspeed > 0)
        {
            newspeed /= speed;
        }
        currentVelocity.x *= newspeed;
        currentVelocity.z *= newspeed;

    }
    private void QueueJump()
    {
        if (input.Player.Jump.triggered /*jump.wasPressedThisFrame*/ &&
            !jumpQueued)
        {
            jumpQueued = true;
        }

        if (jump.wasReleasedThisFrame)
        {
            jumpQueued = false;
        }
    }

    private void GroundMove()
    {
        Vector3 desiredDirection;

        //DO not apply friction if the player is queueing up the next jump
        if (!jumpQueued)
        {
            ApplyFriction(1.0f);
        }
        else
        {
            ApplyFriction(0);
        }

        SetMovementDirection();

        desiredDirection = movementDirection;
        // Debug.Log("Ground Move");
        desiredDirection = transform.TransformDirection(desiredDirection);
        desiredDirection.Normalize();
        //moveDirectionNormal = desiredDirection;

        var desiredspeed = desiredDirection.magnitude;
        desiredspeed *= moveSpeed;

        Accelerate(desiredDirection, desiredspeed, moveAcceleration);

        //reset the gravity velocity
        currentVelocity.y = -gravity * Time.deltaTime;

        if (jumpQueued)
        {
            currentVelocity.y = jumpSpeed;
            jumpQueued = false;
        }

    }

    private void Accelerate(Vector3 wishdirection, float wishspeed, float accelleration)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(currentVelocity, wishdirection);
        addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
        {
            return;
        }
        accelspeed = accelleration * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }
        currentVelocity.x += accelspeed * wishdirection.x;
        currentVelocity.z += accelspeed * wishdirection.z;

    }
}
