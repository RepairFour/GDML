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
    [SerializeField] float aknowledgementTimerMax;
    float aknowledgementTimer = 0;
    Vector3 playerPos;

    NavMeshAgent NavMeshAgent;
    [Tooltip("The distance this can run before needing a break")]
    [SerializeField]float runawayDistance;
    [Tooltip("How long of a break this needs after expending its runaway distance")]
    [SerializeField] float runawayCooldown;
    [HideInInspector]
    public float runawayTimer = 0;
    float runawayDistanceMax;

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
        NavMeshAgent = GetComponent<NavMeshAgent>();
        runawayDistanceMax = runawayDistance;
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
            Debug.DrawLine(transform.position, agent.destination, Color.green);
        }
        else // get to optimal attack range
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
                aknowledgementTimer += Time.deltaTime;
                runawayTimer -= Time.deltaTime;
                if (aknowledgementTimer > aknowledgementTimerMax && runawayTimer < 0)
                {
                    runawayDistance -= Vector3.Distance(player.transform.position, playerPos);
                    playerPos = player.transform.position;
                    aknowledgementTimer = 0;                 
                    //if distance < 0 
                    if(runawayDistance <= 0)
					{
                        runawayTimer = runawayCooldown;
                        runawayDistance = runawayDistanceMax;
                        FindPath(transform.position);
                    }
                    
                }
                if (runawayTimer < 0)
                {
                    //the desired distance away from the player to shoot hit
                    FindPath(playerPos - ((playerPos - transform.position).normalized * attackDistance));
                }
            }
            Debug.DrawLine(transform.position, agent.destination, Color.green);
        }
        
    }

    void FindPath(Vector3 desiredDestination)
	{
        agent.SetDestination(desiredDestination);
        if(agent.pathStatus != NavMeshPathStatus.PathComplete)
		{
            agent.destination = Quaternion.Euler(0, 45, 0) * agent.destination;
        }
       
    }

    private void CanISeeThePlayer()
	{
        chasePlayer = false;
        // try to see player 
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - transform.position;
        //Debug.DrawRay(transform.position, directionToPlayer.normalized * visionDistance, Color.red);
        if (Physics.Raycast(transform.position, directionToPlayer, out hit,visionDistance,~layersToIgnore))
        {
            if (hit.collider.GetComponent<PlayerStats>() != null /*&& Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0*/)
            {
                chasePlayer = true;
            }
        }
    }
}
