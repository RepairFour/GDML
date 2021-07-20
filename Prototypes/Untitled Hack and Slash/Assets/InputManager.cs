using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpSpeed = 100f;
    public float strafeSpeed = 10f;
    public float gravity = 9.8f;

    //public bool isGrounded;

    public Vector3 verticalVelocity; //current
    Vector3 forwardsVelocity;
    Vector3 strafeVelocity;

    Vector3 totalVelocity;

    CharacterController cc;
    Rigidbody rb;
    [SerializeField] GroundedHandler gh;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Get forward direction
        Vector3 fDirection = transform.forward;
        fDirection.y = 0;
        fDirection.Normalize();
        

        

        forwardsVelocity = fDirection * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        //forwardsVelocity.y = 0;
        strafeVelocity = transform.right * Input.GetAxis("Horizontal") * strafeSpeed * Time.deltaTime;
        //strafeVelocity.y = 0;

        if (Input.GetKeyDown(KeyCode.Space) && gh.isGrounded)
        {
            verticalVelocity = jumpSpeed * Vector3.up;
            //Mathf.Clamp(verticalVelocity.y, 0, 1);
            gh.isGrounded = false;
        }
        totalVelocity = verticalVelocity + forwardsVelocity + strafeVelocity;
        
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

    
}


