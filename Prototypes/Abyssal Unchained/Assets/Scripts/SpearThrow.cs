using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearThrow : MonoBehaviour
{
    public GameObject spear;
	public Vector3 force;
	public float forceMax;
	public float cooldown;
	float timer = 0;
    Rigidbody2D playerRb;

	private void Start()
	{
		playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
    {
		timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && timer > cooldown)
		{
			timer = 0;
            GameObject obj = Instantiate(spear,playerRb.gameObject.transform);
			Vector3 force1 = force * playerRb.velocity;
			if(force1.x < force.x)
			{
				force1.x = force.x;
			}
			else if(force1.x > forceMax)
			{
				force1.x = forceMax;
			}
            obj.GetComponent<Rigidbody2D>().AddForce(force1);            
		}
    }
}
