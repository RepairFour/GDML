using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    public Rigidbody2D rb;
    Vector3 currentPosition;
    public float moveSpeed;
    public float jumpSpeed;
   

    Vector3 velocity;
    public bool grounded;
    public float forwardsVelocityMultiplier;
    public float backwardsVelocityMultiplier;
    public float gravityModifier;

    float currentForwardsVelocityMultiplier;
    float currentBackwardsVelocityMultiplier;

    Animator animator;
    public Animator weaponAnimation;


    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        grounded = false;
        rb = GetComponent<Rigidbody2D>();
        currentForwardsVelocityMultiplier = forwardsVelocityMultiplier;
        currentBackwardsVelocityMultiplier = backwardsVelocityMultiplier;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.D))
        //{
        //    velocity.x = currentForwardsVelocityMultiplier;
        //    animator.SetBool("Running", true);

        //}
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //    weaponAnimation.SetBool("Thrust", true);
        //}
        //if (Input.GetKeyUp(KeyCode.LeftControl)){
        //    weaponAnimation.SetBool("Thrust", false);
        //}
        
       
        //if (Input.GetKeyUp(KeyCode.D))
        //{
        //    animator.SetBool("Running", false);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    velocity.x = -currentBackwardsVelocityMultiplier;
        //    animator.SetBool("Backwards", true);
            
        //}
        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    animator.SetBool("Backwards", false);
        //}
        //if (Input.GetKeyDown(KeyCode.Space) && grounded)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

        //    //rb.AddForce(Vector3.up * jumpSpeed);
        //    //Debug.Log("jumping");
        //    //currentPosition.y = transform.position.y;
        //    grounded = false;
        //}
        //Vector3 targetVelocity = new Vector3(velocity.x * moveSpeed, rb.velocity.y, 0);

        //rb.velocity = targetVelocity;
        ////transform.position += targetVelocity * Time.deltaTime;

        //if (!grounded)
        //{
        //    //velocity.y -= rb.gravityScale * Time.deltaTime;
        //}
        ////transform.position = currentPosition;
        //velocity = new Vector3(0, velocity.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.CompareTag("Ground"))
       {
             grounded = true;
       }
    }

    public void Move(float direction)
    {
        if(direction == 1)
        {
            velocity.x = currentForwardsVelocityMultiplier;
            animator.SetBool("Running", true);
        }
        if(direction == -1)
        {
            velocity.x = -currentBackwardsVelocityMultiplier;
            animator.SetBool("Backwards", true);
        }
        if(direction == 0)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Backwards", false);
        }
        //if(direction.y == 1 && grounded)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

        //    //rb.AddForce(Vector3.up * jumpSpeed);
        //    //Debug.Log("jumping");
        //    //currentPosition.y = transform.position.y;
        //    grounded = false;
        //}


        Vector3 targetVelocity = new Vector3(velocity.x * moveSpeed, rb.velocity.y, 0);

        rb.velocity = targetVelocity;
  
        velocity = new Vector3(0, velocity.y, 0);
    }
    public void Jump()
    {
        if (grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

            //rb.AddForce(Vector3.up * jumpSpeed);
            //Debug.Log("jumping");
            //currentPosition.y = transform.position.y;
            grounded = false;
        }
        
    }


}
