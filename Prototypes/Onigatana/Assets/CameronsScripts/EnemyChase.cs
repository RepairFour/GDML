using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    PlayerStats player;
    NavMeshAgent agent;
    [SerializeField] Transform patrolStart;
    [SerializeField] Transform patrolEnd;
    bool finishedPatrol = false;
    bool chasePlayer = false;

    EnemyAttack enemyAttack;
    [SerializeField] float rangedDesiredDistance;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (chasePlayer == false)
        {
            CanISeeThePlayer(transform.forward);
            CanISeeThePlayer(transform.right);
            CanISeeThePlayer(-transform.right);

            //patrol if you cant see the player
            if (!finishedPatrol)
            {
                agent.destination = patrolEnd.position;
                if (Mathf.Abs(transform.position.x - patrolEnd.position.x) < 0.1f
                    && Mathf.Abs(transform.position.z - patrolEnd.position.z) < 0.1f)
                {
                    finishedPatrol = true;
                }

            }
            else
            {
                agent.destination = patrolStart.position;
                if (Mathf.Abs(transform.position.x - patrolStart.position.x) < 0.1f
                    && Mathf.Abs(transform.position.z - patrolStart.position.z) < 0.1f)
                {
                    finishedPatrol = false;
                }
            }
        }
		else // chase the player
		{
            enemyAttack.attackMode = true;
            if (enemyAttack.melee)
            {
                agent.destination = player.transform.position;
                
            }
			else
			{
                //the desired distance away from the player to shoot him
                agent.destination = player.transform.position - ((player.transform.position - transform.position).normalized  * rangedDesiredDistance); 
            }
        }
        
    }

    private void CanISeeThePlayer(Vector3 direction)
	{
        // try to see player 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            if (hit.collider.GetComponent<PlayerStats>() != null)
            {
                chasePlayer = true;
            }
        }
    }
}
