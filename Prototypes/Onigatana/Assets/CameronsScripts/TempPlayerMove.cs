using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerMove : MonoBehaviour
{
	Rigidbody rb;
	public float speed;
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}
	private void Update()
	{
		if(Input.GetKey(KeyCode.W))
		{
			rb.AddForce(new Vector3(0, 0, 1)* speed);
		}
		if (Input.GetKey(KeyCode.A))
		{
			rb.AddForce(new Vector3(1, 0, 0)* speed);
		}
		if (Input.GetKey(KeyCode.S))
		{
			rb.AddForce(new Vector3(0, 0, -1)* speed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			rb.AddForce(new Vector3(-1, 0, 0)* speed);
		}
	}
}
