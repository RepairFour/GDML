using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    PlayerInputManager input;
    public GroundedHandler gh;

    public float moveSpeed;
    public float jumpSpeed;

    public float gravityScale;

    Vector3 forwardDirection;

    Vector3 lateralVelocity;
    Vector3 jumpVelocity;

    Rigidbody rb;
   
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInputManager>();
    }

    private void Update()
    {
        forwardDirection = transform.forward;
        if (input.movementMap.FindAction("Jump").triggered && gh.isGrounded)
        {

            jumpVelocity.y = jumpSpeed;
            gh.isGrounded = false;
        }

        lateralVelocity = input.getMoveDirection * moveSpeed;

        var forwardVelocity = forwardDirection * lateralVelocity.z;

        forwardVelocity.y = 0;

        var strafeVelocity = transform.right * lateralVelocity.x;

        rb.velocity = forwardVelocity + strafeVelocity + jumpVelocity;

        if (!gh.isGrounded)
        {
            jumpVelocity.y -= gravityScale * Time.deltaTime;
        }
        else
        {
            jumpVelocity.y = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        
    }
}
