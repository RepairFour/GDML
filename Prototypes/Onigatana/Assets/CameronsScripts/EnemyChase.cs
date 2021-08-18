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
    Rigidbody rb;

    EnemyAttack enemyAttack;
    [SerializeField] float attackDistance;

    //leaping stuff for melee units
    bool leap = false;
    float leapTimer = 0;
    [SerializeField] float leapTimerMax;
    [SerializeField] float leapForce;
    float leapingTimer = 0;
    float agentWalkSpeed;
    float agentAccelleration;
    Vector3 leapVector = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        enemyAttack = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody>();
        agentWalkSpeed = agent.speed;
        agentAccelleration = agent.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        if (chasePlayer == false)
        {
            CanISeeThePlayer();

            //patrol if you cant see the player
            if (!finishedPatrol)
            {
                agent.destination = patrolEnd.position;
                if (Mathf.Abs(transform.position.x - patrolEnd.position.x) < 1
                    && Mathf.Abs(transform.position.z - patrolEnd.position.z) < 1)
                {
                    finishedPatrol = true;
                }

            }
            else
            {
                agent.destination = patrolStart.position;
                if (Mathf.Abs(transform.position.x - patrolStart.position.x) < 1
                    && Mathf.Abs(transform.position.z - patrolStart.position.z) < 1)
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
                leapingTimer -= Time.deltaTime;
                if (!leap)
                {
                    if (leapingTimer < 0)
                    {
                        agent.speed = agentWalkSpeed;
                        agent.acceleration = agentAccelleration;
                        agent.destination = player.transform.position - ((player.transform.position - transform.position).normalized * attackDistance);
                        if (Mathf.Abs(transform.position.x - agent.destination.x) < 1 &&
                           Mathf.Abs(transform.position.z - agent.destination.z) < 1)
                        {
                            leap = true;
                        }
                    }
					else
					{
                        //vector to the player
                        Vector3 toPlayer = player.transform.position - transform.position;
                        //dot product against leap vector
                        if(Vector3.Dot(leapVector, toPlayer) < 0)                        
                        {
                            agent.velocity = Vector3.zero;
                            

                        }
					}
                }
				else
                { 
                    leapTimer += Time.deltaTime;
                    if(leapTimer > leapTimerMax)
					{
                        agent.destination = player.transform.position;                       
                        agent.speed = leapForce;
                        agent.acceleration = leapForce;
                        leap = false;
                        leapTimer = 0;
                        leapingTimer = 1;
                        leapVector = agent.destination - transform.position;
                    }
				}
                
            }
			else
			{
                //the desired distance away from the player to shoot him
                agent.destination = player.transform.position - ((player.transform.position - transform.position).normalized  * attackDistance); 
            }
        }
        
    }

    private void CanISeeThePlayer()
	{
        // try to see player 
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit))
        {
            if (hit.collider.GetComponent<PlayerStats>() != null && Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0.25)
            {
                chasePlayer = true;
            }
        }
    }
}
