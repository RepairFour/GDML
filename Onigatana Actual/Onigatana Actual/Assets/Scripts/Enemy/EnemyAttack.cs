using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool melee;
    public int dmgPerHit;
    [HideInInspector]
    public bool attackMode = false;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    PlayerStats player;

    float timer = 0;
    [SerializeField] float timerMax;
    [HideInInspector]
    public float attackDistance;
    bool firstAttack = true;
    [SerializeField][Range(1,3)] float firstAttackMod = 1;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        attackDistance = GetComponent<EnemyChase>().attackDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerStats>();
        }
        if (attackMode)
        {
            timer += Time.deltaTime;
            if(firstAttack)
			{

                firstAttack = false;
                timer -= Random.value * firstAttackMod;
			}
        }
        if(!melee && attackMode && timer > timerMax)
		{
            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
            {
                var bullet = Instantiate(projectile, transform.position, transform.rotation);
                bullet.GetComponent<EnemyProjectile>().dmg = dmgPerHit;
                bullet.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position).normalized * projectileSpeed);
                timer = 0;
            }
        }
    }

	//private void OnCollisionEnter(Collision collision)
	//{
 //       if (melee && timer > timerMax)
 //       {
 //           var player = collision.gameObject.GetComponent<PlayerStats>();
 //           if (player != null)
	//		{
 //               player.Hurt(dmgPerHit);
 //               timer = 0;
 //           }
 //       }
 //   }
	private void OnTriggerEnter(Collider other)
    {
        if (melee && timer > timerMax)
        {
            var player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.Hurt(dmgPerHit);
                timer = 0;
            }
        }
    }
}
