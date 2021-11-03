using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GFollowPlayerLvl1 : GalahadAction
{
    bool performAction = false;
    NavMeshAgent nav;
    Transform player;
    [SerializeField] float followDuration;
    float followDurationTimer = 0;
	public override void Perform()
	{
        performAction = true;
    }

	// Start is called before the first frame update
	void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerStats>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(performAction == true)
		{
            if (nav.enabled)
            {
                nav.destination = player.position;
                followDurationTimer += Time.deltaTime;
                if (followDurationTimer >= followDuration)
                {
                    followDurationTimer = 0;
                    performAction = false;
                    finished = true;
                }
            }

        }
    }
}
