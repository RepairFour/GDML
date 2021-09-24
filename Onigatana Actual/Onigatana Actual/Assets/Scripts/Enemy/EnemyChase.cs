using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    PlayerStats player;
    NavMeshAgent agent;
    WaypointManager wayManager;
    [HideInInspector]
    public int patrolPath;
    List<Transform> path;

    int pathNode = 0;
    bool chasePlayer = false;

    EnemyAttack enemyAttack;
    public float attackDistance;

    //leaping stuff for melee units
    bool leap = false;
    float leapTimer = 0;
    [SerializeField] float leapTimerMax;
    [SerializeField] float leapForce;
    float leapingTimer = 0;
    float agentWalkSpeed;
    float agentAccelleration;
    Vector3 leapVector = new Vector3();
    [SerializeField] float visionDistance = 100;
    [SerializeField] LayerMask layersToIgnore;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        enemyAttack = GetComponent<EnemyAttack>();
        agentWalkSpeed = agent.speed;
        agentAccelleration = agent.acceleration;
        wayManager = FindObjectOfType<WaypointManager>();
        path = new List<Transform>(wayManager.paths[patrolPath].waypoints);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
		{
            player = FindObjectOfType<PlayerStats>();
        }
        CanISeeThePlayer(); // fills out chasePlayer bool 
        if (chasePlayer == false)
        {
            enemyAttack.attackMode = false;
            agent.destination = path[pathNode].position;
			if (Mathf.Abs(transform.position.x - path[pathNode].position.x) < 1 &&
			   Mathf.Abs(transform.position.z - path[pathNode].position.z) < 1)
			{
				++pathNode;
				if (pathNode == path.Count)
				{
					pathNode = 0;
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
        chasePlayer = false;
        // try to see player 
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - transform.position;
        Debug.DrawRay(transform.position, directionToPlayer.normalized * visionDistance, Color.red);
        if (Physics.Raycast(transform.position, directionToPlayer, out hit,visionDistance,~layersToIgnore))
        {
            if (hit.collider.GetComponent<PlayerStats>() != null /*&& Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0*/)
            {
                chasePlayer = true;
            }
        }
    }
}
