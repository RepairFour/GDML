using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D playerRb;
    public float speed;
    public float jumpHeight;
    public float maxVelocity;
    bool grounded = true;
    Animator horseRun;
    ParticleSystem airDrag;
    ParticleSystem.EmissionModule emission;
    [SerializeField] AudioSource landing;
    [SerializeField] AudioSource jumping;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        horseRun = GetComponent<Animator>();
        airDrag = GetComponent<ParticleSystem>();
        emission = airDrag.emission;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.D))
		{
            horseRun.speed = 1.5f;        
            emission.rateOverTime = 20;
            if (playerRb.velocity.x < maxVelocity)
            {
                playerRb.AddForce(new Vector2(speed * Time.deltaTime, 0));
            }
			else
			{
                playerRb.velocity = new Vector2(maxVelocity, playerRb.velocity.y);               
                
            }
		}
		if(Input.GetKeyUp(KeyCode.D))
		{
            horseRun.speed = 1;          
            emission.rateOverTime = 10;
        }
        if(Input.GetKeyDown(KeyCode.Space))
		{
            if (grounded)
            {
                playerRb.AddForce(new Vector2(0, jumpHeight));
                grounded = false;
                if (jumping.isPlaying == false)
                {
                    jumping.Play();
                }
            }
		}
        if(Input.GetKey(KeyCode.A))
		{
            horseRun.speed = 0.8f;
            emission.rateOverTime = 5;
            if (playerRb.velocity.x > -10)
            {
                playerRb.AddForce(new Vector2(-1 * (speed * Time.deltaTime), 0));
            }
			else
			{
                playerRb.velocity = new Vector2(-10, playerRb.velocity.y);
            }

        }
        if(Input.GetKeyUp(KeyCode.A))
		{
            horseRun.speed = 1;
            emission.rateOverTime = 10;
        }
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        grounded = true;
        if (landing.isPlaying == false)
        {
            landing.Play();
        }
    }
	private void OnTriggerExit2D(Collider2D collision)
	{
       

    }
}
