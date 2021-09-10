using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyArea : MonoBehaviour
{
    [SerializeField] GameObject key;
    BoxCollider zone;

	float timer = 0;
	float timerMax = 1;
	List<EnemyStats> enemiesInZone;

	private void Start()
	{
		zone = GetComponent<BoxCollider>();
		enemiesInZone = new List<EnemyStats>();
		//record all the enemies in my bounds
		foreach (var monster in FindObjectsOfType<EnemyStats>())
		{
			if (zone.bounds.Contains(monster.transform.position))
			{
				enemiesInZone.Add(monster);
			}
		}
	}
	// Update is called once per frame
	void Update()
    {
		timer += Time.deltaTime;
		if (timer > timerMax) //this is for performance
		{
			timer = 0;
			foreach (var monster in enemiesInZone)
			{
				if(monster != null) //if one isn't dead
				{
					return;
				}
			}
			//otherwise
			key.SetActive(true);
			Destroy(gameObject);
		}
    }
}
