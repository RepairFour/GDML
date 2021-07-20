using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbStat : MonoBehaviour
{
    public int health = 5;
    public EnemyStats mainBody;

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        mainBody.TakeDamage(dmg);
    }
}
