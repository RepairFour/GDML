using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PlayerAttack : MonoBehaviour
{
    PlayerMap inputs;
    [Header("Main Hitbox")]
    [SerializeField]Vector3 halfExtents;
    [SerializeField] float distance;
    [SerializeField] LayerMask layerhit;
    [SerializeField] int dmg;

    [Header("Secondary Hitbox")]
    [SerializeField] Vector3 halfExtentsSecondary;
    [SerializeField] float distanceSecondary;
    [SerializeField] LayerMask layerhitSecondary;
    [SerializeField] int dmgSecondary;

    [Header("Misc")]
    [SerializeField] bool showHitBox;
    [SerializeField] Animator weaponSlash;
    [SerializeField][Min(0)] float attackCD;
    [SerializeField] LayerMask layerToIgnore;

    List<EnemyStats> enemiesHit;
    PlayerStats playerStats;

    //[SerializeField] float hitBoxRotation;
    float attackCDTimer;
    // Start is called before the first frame update
    void Start()
    {
        inputs = new PlayerMap();
        inputs.Enable();
        attackCDTimer = attackCD;
        enemiesHit = new List<EnemyStats>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (showHitBox)
        {
            ExtDebug.DrawBoxCastBox(transform.position, halfExtents, gameObject.transform.rotation, gameObject.transform.forward, distance, Color.blue);
            ExtDebug.DrawBoxCastBox(transform.position, halfExtentsSecondary, gameObject.transform.rotation, gameObject.transform.forward, distanceSecondary, Color.red);            
        }

        if (Application.isPlaying)
        {
            if (inputs.Player.Attack.triggered && weaponSlash.GetBool("Attack") == false && attackCDTimer > attackCD == true)
            {
                gameObject.transform.rotation = Camera.main.transform.rotation;
                Debug.Log("Attacking");
                weaponSlash.SetTrigger("Attack");
                var n = Random.Range(1, 3);
                weaponSlash.SetInteger("Attack#", n);
                attackCDTimer = 0;
                RaycastHit[] hits = Physics.BoxCastAll(transform.position, halfExtents, gameObject.transform.forward, gameObject.transform.rotation, distance, layerhit);
                RaycastHit[] hitsSecondary = Physics.BoxCastAll(transform.position, halfExtentsSecondary, gameObject.transform.forward, gameObject.transform.rotation, distanceSecondary, layerhitSecondary);

                //hit the enemies hitting the secondary hitbox first
                enemiesHit.Clear();
                HitEnemies(hitsSecondary, dmgSecondary);
                HitEnemies(hits, dmg);

            }
            else
            {
                attackCDTimer += Time.deltaTime;
            }
        }
    }

    private void HitEnemies(RaycastHit[] hits , int dmg)
	{
        if (hits != null && hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                EnemyStats enemy = hit.collider.gameObject.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    RaycastHit ray;
                    if(Physics.Raycast(transform.position, enemy.transform.position - transform.position, out ray, 10000 , ~layerToIgnore)) // to stop hitting through walls
					{
                        if(ray.collider.GetComponent<EnemyStats>())
						{
                            bool alreadyHit = false;
                            foreach (var e in enemiesHit)
                            {
                                if (enemy.gameObject == e.gameObject)//another hitbox has it this enemy already
                                {
                                    alreadyHit = true;
                                }
                            }
                            if (!alreadyHit)
                            {
                                enemy.Hurt(dmg);
                                enemiesHit.Add(enemy);
                                playerStats.FillBloodMeter(10);
                            }
                        }
					}
                    
                }
            }
        }
    }

	#region BoxCastDraw
	public static class ExtDebug
    {
        //Draws just the box at where it is currently hitting.
        public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
        {
            origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
            DrawBox(origin, halfExtents, orientation, color);
        }

        //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
        public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
        {
            direction.Normalize();
            Box bottomBox = new Box(origin, halfExtents, orientation);
            Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

            Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
            Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
            Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
            Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
            Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
            Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
            Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
            Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

            DrawBox(bottomBox, color);
            DrawBox(topBox, color);
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
        {
            DrawBox(new Box(origin, halfExtents, orientation), color);
        }
        public static void DrawBox(Box box, Color color)
        {
            Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
            Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
            Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
            Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

            Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
            Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
            Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
            Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

            Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
            Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
            Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
            Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
        }

        public struct Box
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }
            public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
            public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
            public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
            public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

            public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
            public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
            public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
            public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
            public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
            public Vector3 backTopRight { get { return localBackTopRight + origin; } }
            public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
            public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

            public Vector3 origin { get; private set; }

            public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }
            public Box(Vector3 origin, Vector3 halfExtents)
            {
                this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }


            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }

        //This should work for all cast types
        static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
        {
            return origin + (direction.normalized * hitInfoDistance);
        }

        static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
    }

	#endregion
}
