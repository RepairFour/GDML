using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float moveSpeed = 5;
    public float clampAmount;
    public float timeLimit;
    Vector2 startPosition;
    float clampNeg;
    float clampPos;
    float timer = 0;
	public float spawnHeight;
    Vector3 spawnPos;
	public AudioSource spawnSound;
	public Vector2 spawnVelocity;
	private void Start()
	{
		spawnPos = transform.position;
		spawnPos.y += spawnHeight;

        startPosition = transform.position;

        clampNeg = startPosition.x - clampAmount;
        clampPos = startPosition.x + clampAmount;
    }

	// Update is called once per frame
	void Update()
    {
		timer += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space) || timer > timeLimit)
		{
			timer = 0;
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnPos;
            //FindObjectOfType<PlayerMovement>().gameObject.transform.position = spawnPos;
            gameObject.SetActive(false);
			spawnSound.Play();
			Player.instance.respawning = false;
			Player.instance.GetComponent<Rigidbody2D>().velocity = spawnVelocity;
		}

        var move = Input.GetAxis("Horizontal");
        var position = transform.position;

        position.x += move * moveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, clampNeg, clampPos);

        transform.position = position;
        spawnPos = transform.position;
        
    }
}
