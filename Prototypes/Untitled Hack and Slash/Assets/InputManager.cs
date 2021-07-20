using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputActionMap movementStuff;


    public float movementGravity;

    public float moveSpeed = 10f;
    public float jumpSpeed = 100f;
    public float strafeSpeed = 10f;
    public float gravity = 9.8f;

    //public bool isGrounded;

    public Vector3 verticalVelocity; //current
    Vector3 forwardsVelocity;
    Vector3 strafeVelocity;
    Vector3 currentMoveDirection;

    Vector3 totalVelocity;

    CharacterController cc;
    Rigidbody rb;
    [SerializeField] GroundedHandler gh;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
      
        movementStuff.Enable();
    }

    private void Update()
    {
        //Get forward direction
        Vector3 fDirection = transform.forward;
        fDirection.y = 0;
        fDirection.Normalize();


        //var keyboard = Keyboard.current;
        var inputDirection = movementStuff.FindAction("MoveAction", true).ReadValue<Vector2>();
        var moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        moveDirection.Normalize();

        currentMoveDirection.x = Mathf.MoveTowards(currentMoveDirection.x, moveDirection.x, movementGravity * Time.deltaTime);
        currentMoveDirection.z = Mathf.MoveTowards(currentMoveDirection.z, moveDirection.z, movementGravity * Time.deltaTime);



        Debug.Log("fDirection " + fDirection);

        Debug.Log("moveDirection " + moveDirection);

        forwardsVelocity = fDirection * currentMoveDirection.z;
        Debug.Log("forwardsVelocity " + forwardsVelocity);

        strafeVelocity = currentMoveDirection.x * transform.right;
        Debug.Log("strafeVelocity " + strafeVelocity);
        totalVelocity = (forwardsVelocity + strafeVelocity) * moveSpeed * Time.deltaTime;
        
        

        if (movementStuff.FindAction("JumpAction").triggered && gh.isGrounded)
        {
            verticalVelocity = jumpSpeed * Vector3.up;
            //Mathf.Clamp(verticalVelocity.y, 0, 1);
            gh.isGrounded = false;
        }
        totalVelocity += verticalVelocity;
        //Debug.Log(totalVelocity);
        
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (gh.isGrounded == false)
        {
           verticalVelocity.y -= gravity * Time.deltaTime;
        }

        cc.Move(totalVelocity);

        if (gh.isGrounded)
        {
            verticalVelocity.y = 0;
        }
        
        //verticalVelocity.y -= gravity * Time.deltaTime;
    }

    public void Move(InputAction.CallbackContext context)
    {
        //var moveDirection = 
    }


}


