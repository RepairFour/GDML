using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
	#region Variables
	PlayerStats player;
    NavMeshAgent agent;
    WaypointManager wayManager;
    [HideInInspector]
    public int patrolPath;
    List<Transform> path;

    int pathNode = 0;
    bool chasePlayer = false;

    EnemyAttack enemyAttack;
    public float basicAttackDistance;

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

    [SerializeField] float loseInterestTimerMax;
    float loseInterestTimer = 0;
    bool wasChasing = false;

    Vector3 lastLocation = Vector3.zero;
    bool relocate = false;
    Vector3 relocatePos;
    [SerializeField]float relocateTimerMax;
    float relocateTimer = 0;
    float standingStillTimerMax = 0.8f;
    float standingStillTimer = 0;

    [SerializeField] Transform debugDestination;
    float debugTimer = 0;
    float debugTimerMax = 1;
	#endregion

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
        if (debugDestination == null)
        {
            CanISeeThePlayer(); // fills out chasePlayer bool 
            if (chasePlayer == false)
            {
                if (wasChasing)
                {
                    loseInterestTimer += Time.deltaTime;
                    if (loseInterestTimer >= loseInterestTimerMax)
                    {
                        wasChasing = false;
                        loseInterestTimer = 0;
                    }
                    ChasePlayer();
                    Debug.Log("Chasing player");
                }
                else
                {
                    FollowPatrol();
                    Debug.Log("Following patrol");
                }
            }
            else // get to optimal attack range
            {
                wasChasing = true;
                enemyAttack.attackMode = true;
                ChasePlayer();
                Debug.Log("Chasing player 2");
            }
        }
		else
		{
            debugTimer += Time.deltaTime;
            if (debugTimer >= debugTimerMax)
            {
                debugTimer = 0;
                FindPath(debugDestination.position);
            }
		}
    }

    void FindPath(Vector3 desiredDestination)
	{
        if (Vector3.Distance(transform.position, lastLocation) < 0.7f &&
            Vector3.Distance(transform.position, desiredDestination) > 1f &&
            standingStillTimer > (standingStillTimerMax / 2) &&
            relocate == false)
        {
            if (enemyAttack.type != EnemyAttack.EnemyType.SHIELD_COMBATANT)
            {
                agent.SetDestination((Quaternion.Euler(0, Random.Range(100, 260), 0) * desiredDestination).normalized * basicAttackDistance);
                relocatePos = agent.destination;
                relocate = true;
                Debug.Log("Relocate to " + agent.destination);
            }
        }

        if (!relocate)
		{
			agent.destination = desiredDestination;
		}
		else
		{
            relocateTimer += Time.deltaTime;
            if (relocateTimer >= relocateTimerMax)
            {
                relocateTimer = 0;
                relocate = false;                
            }            
        }
        
        standingStillTimer += Time.deltaTime;
        if(standingStillTimer >= standingStillTimerMax)
		{
            lastLocation = transform.position;
            standingStillTimer = 0;
        }

    }

    private void CanISeeThePlayer()
	{
        chasePlayer = false;
        // try to see player 
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit,visionDistance,~layersToIgnore))
        {
            if (hit.collider.GetComponent<PlayerStats>() != null /*&& Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0*/)
            {
                chasePlayer = true;
            }
        }
    }

    private void ChasePlayer()
	{
        if (enemyAttack.type == EnemyAttack.EnemyType.MELEE_FODDER)
        {
            leapingTimer -= Time.deltaTime;
            if (!leap)
            {
                if (leapingTimer < 0)
                {
                    agent.speed = agentWalkSpeed;
                    agent.acceleration = agentAccelleration;
                    agent.destination = player.transform.position - ((player.transform.position - transform.position).normalized * basicAttackDistance);
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
                    if (Vector3.Dot(leapVector, toPlayer) < 0)
                    {
                        agent.velocity = Vector3.zero;


                    }
                }
            }
            else
            {
                leapTimer += Time.deltaTime;
                if (leapTimer > leapTimerMax)
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
        else if(enemyAttack.type == EnemyAttack.EnemyType.RANGED_FODDER)
        {
            DistanceAndAknowledgementTracker();
            if (runawayTimer < 0)
            {
                //the desired distance away from the player to shoot hit
                FindPath(playerPos + ((transform.position - playerPos).normalized * basicAttackDistance));
            }
        }
        else if(enemyAttack.type == EnemyAttack.EnemyType.SHIELD_COMBATANT)
		{
            DistanceAndAknowledgementTracker();
            if (runawayTimer < 0) //if I can move
            {
                //the desired distance away from the player to shoot hit
                FindPath(transform.position + ((playerPos - transform.position)* 0.5f));
            }
        }
        else if (enemyAttack.type == EnemyAttack.EnemyType.JUMPER_COMBATANT)
        {
            DistanceAndAknowledgementTracker();
            if (runawayTimer < 0) //if I can move
            {
                //the desired distance away from the player to shoot hit
                FindPath(transform.position + ((playerPos - transform.position) * 0.5f));
            }
        }
        Debug.DrawLine(transform.position, agent.destination, Color.green);
    }
    private void DistanceAndAknowledgementTracker()
	{
        aknowledgementTimer += Time.deltaTime;
        runawayTimer -= Time.deltaTime;
        if (aknowledgementTimer > aknowledgementTimerMax && runawayTimer < 0)
        {
            runawayDistance -= Vector3.Distance(player.transform.position, playerPos);
            playerPos = player.transform.position;
            aknowledgementTimer = 0;
            if (runawayDistance <= 0)
            {
                runawayTimer = runawayCooldown;
                runawayDistance = runawayDistanceMax;
                FindPath(transform.position);
                Debug.Log("Wating");
            }

        }
    }
    private void FollowPatrol()
    {
        enemyAttack.attackMode = false;
        if (agent.enabled && agent.isOnNavMesh)
        {
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
        Debug.DrawLine(transform.position, agent.destination, Color.green);
    }
}
