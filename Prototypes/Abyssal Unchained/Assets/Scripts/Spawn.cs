using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float timeLimit;
    float timer = 0;
	public float spawnHeight;
    Vector3 spawnPos;
	public AudioSource spawnSound;
	public Vector2 spawnVelocity;
	private void Start()
	{
		spawnPos = transform.position;
		spawnPos.y += spawnHeight;
	}

	// Update is called once per frame
	void Update()
    {
		timer += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space) || timer > timeLimit)
		{
			timer = 0;
            FindObjectOfType<PlayerMovement>().gameObject.transform.position = spawnPos;
            gameObject.SetActive(false);
			spawnSound.Play();
			Player.instance.respawning = false;
			Player.instance.GetComponent<Rigidbody2D>().velocity = spawnVelocity;
		}
    }
}
