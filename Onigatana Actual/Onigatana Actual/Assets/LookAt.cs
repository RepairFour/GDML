using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LookAt : MonoBehaviour
{
    NavMeshAgent agent;
	public float speed;
	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update()
    {
        transform.LookAt(agent.destination);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

		//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.destination), speed * Time.deltaTime);
	}
}
