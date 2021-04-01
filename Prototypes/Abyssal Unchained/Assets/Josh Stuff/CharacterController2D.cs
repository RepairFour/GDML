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
    public bool knockedBack = false;
    public float forwardsVelocityMultiplier;
    public float backwardsVelocityMultiplier;
    public float gravityModifier;

    float currentForwardsVelocityMultiplier;
    float currentBackwardsVelocityMultiplier;

    Animator animator;
    //public Animator weaponAnimation;

    ParticleSystem airDrag;
    ParticleSystem.EmissionModule emission;

    bool startGroundedTimer = false;
    float lastGroundedTimer = 0;
    [SerializeField]float lastGroundedMaxTime;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        grounded = false;
        rb = GetComponent<Rigidbody2D>();
        currentForwardsVelocityMultiplier = forwardsVelocityMultiplier;
        currentBackwardsVelocityMultiplier = backwardsVelocityMultiplier;

        animator = GetComponent<Animator>();
        airDrag = GetComponent<ParticleSystem>();
        emission = airDrag.emission;
    }

 

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
       if(collision.gameObject.CompareTag("LeftBound"))
	   {
            currentBackwardsVelocityMultiplier = 0;
	   }
       else if(collision.gameObject.CompareTag("RightBound"))
	   {
            currentForwardsVelocityMultiplier = 0;
       }

        if (collision.gameObject.CompareTag("Ground"))
        {
            AudioHandler.instance.PlaySound("PlayerLand", 1);
        }

    }

	private void OnTriggerStay2D(Collider2D collision)
	{
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            startGroundedTimer = false;
        }
    }

	private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.gameObject.CompareTag("LeftBound"))
        {
            currentBackwardsVelocityMultiplier = backwardsVelocityMultiplier;
        }
        else if (collision.gameObject.CompareTag("RightBound"))
        {
            currentForwardsVelocityMultiplier = forwardsVelocityMultiplier;
        }     
        else if(collision.CompareTag("Ground"))
		{
               startGroundedTimer = true;
        }
        
    }

	public void Move(float direction)
    {
        if (knockedBack)
        {
            return;
        }
        if (direction == 1)
        {
            velocity.x = currentForwardsVelocityMultiplier;
            animator.SetBool("Running", true);
            emission.rateOverTime = 20;
        }
        if(direction == -1)
        {
            velocity.x = -currentBackwardsVelocityMultiplier;
            animator.SetBool("Backwards", true);
            emission.rateOverTime = 5;
        }
        if(direction == 0)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Backwards", false);
            emission.rateOverTime = 10;
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
        if(startGroundedTimer == true)
		{
            lastGroundedTimer += Time.deltaTime;
            if(lastGroundedTimer < lastGroundedMaxTime)
			{
                if (grounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                    AudioHandler.instance.PlaySound("PlayerJump", 1);
                    grounded = false;
                }
            }
			else
			{
                lastGroundedTimer = 0;
                startGroundedTimer = false;
            }
		}
        else if (grounded)
         {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            AudioHandler.instance.PlaySound("PlayerJump",1);
            //rb.AddForce(Vector3.up * jumpSpeed);
            //Debug.Log("jumping");
            //currentPosition.y = transform.position.y;
            grounded = false;
        }
        
    }

    public void KnockBack(Vector2 force, float duration)
    {
        knockedBack = true;
        StartCoroutine(knockBack(force, duration));
    }

    IEnumerator knockBack(Vector2 force, float duration)
    {
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(force * new Vector2(-1,1), ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        knockedBack = false;

    }


}
