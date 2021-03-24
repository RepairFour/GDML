using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : MonoBehaviour
{
    public GameObject enemy;
    public float cooldown;
    float timer = 0;
    public float attackCooldown;
    float attackTimer = 0;
    [SerializeField] GameObject diveEnemy;
    public float dCooldown;
    float dTimer = 0;

    Vector3 spawn;
    [SerializeField] Animator bossAnimator;

    // Start is called before the first frame update
    void Start()
    {
        spawn = new Vector3(transform.position.x - 5, transform.position.y, transform.position.z);
 
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > cooldown)
		{
            timer = 0;
            //Instantiate(enemy, spawn, transform.rotation);
		}

        dTimer += Time.deltaTime;
        if(dTimer > cooldown)
		{
            dTimer = 0;
            //Instantiate(diveEnemy, spawn, transform.rotation);
		}
        attackTimer += Time.deltaTime;
        if(attackTimer > attackCooldown)
		{
            attackTimer = 0;
            Attack1();

        }
    }

    void Attack1()
	{
        bossAnimator.SetTrigger("Attack1");
    }
}
