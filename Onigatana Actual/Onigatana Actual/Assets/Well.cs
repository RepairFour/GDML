using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] LineRenderer ln;
    public Transform lineStart;
    public Transform lineEnd;
    [SerializeField] GameObject wellEnemy;
   
    public void Hurt(int dmg)
	{
        health -= dmg;
        if(health <= 0)
		{
            Destroy(wellEnemy);
            Destroy(gameObject);
		}
	}

    // Update is called once per frame
    void Update()
    {
        ln.SetPosition(0, lineStart.position);
        ln.SetPosition(1, lineEnd.position);
    }
}
