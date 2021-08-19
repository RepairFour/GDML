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
    PlayerStats player;

    float timer = 0;
    [SerializeField] float timerMax;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
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
        }
        if(!melee && attackMode && timer > timerMax)
		{
            var bullet = Instantiate(projectile,transform.position,transform.rotation);
            bullet.GetComponent<EnemyProjectile>().dmg = dmgPerHit;
            bullet.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position) * 100);
            timer = 0;
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
