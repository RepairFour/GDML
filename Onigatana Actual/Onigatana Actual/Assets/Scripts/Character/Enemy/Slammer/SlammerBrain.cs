using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammerBrain : MonoBehaviour
{
	[SerializeField] List<SlammerMove> moveset;
	[SerializeField] float timeBetweenMoves;
	[SerializeField] float basicTurnSpeed;
	Timer timer;
	int moveIndex = -1;
	bool performMove = false;
	GameObject player;
	Vector3 playerLastPos;

	private void Start()
	{
		timer = new Timer(timeBetweenMoves);
		player = FindObjectOfType<PlayerStats>().gameObject;
		playerLastPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
	}
	// Update is called once per frame
	void Update()
    {
		if (!performMove)
		{
			BasicMovement();
			timer.UpdateTimer();
			if (timer.IsFinished())
			{
				timer.ResetTimer();
				performMove = true;
				moveIndex++;
				if(moveIndex == moveset.Count)
				{
					moveIndex = 0;
				}
			}
		}
		else
		{
			performMove = moveset[moveIndex].Activate();
		}
	}


	void BasicMovement()
	{
		Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
		var dir = position - transform.position;
		dir.Normalize();
		var rotGoal = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, basicTurnSpeed * Time.deltaTime);
	}
}
