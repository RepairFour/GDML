using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] int damage;
    bool triggered = false;

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        if(duration <= 0)
		{
            Destroy(gameObject);
		}
    }

	private void OnTriggerEnter(Collider other)
	{
        if (!triggered)
        {
            var player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.Hurt(damage);
                triggered = true;
            }
        }
	}
}
