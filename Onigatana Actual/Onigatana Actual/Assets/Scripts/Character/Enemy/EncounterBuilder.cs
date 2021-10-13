using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EncounterBuilder : MonoBehaviour
{
	[System.Serializable]
	struct Wave
	{
		[System.Serializable]
		public struct Spawn
		{
			public GameObject enemyPrefab;
			public int spawnNumber;
		}
		public List<Spawn> spawns;
	}

	[SerializeField] List<Wave> encounter = new List<Wave>();
	[SerializeField] bool spawnNextOnWipe;
	[SerializeField] float spawnTimerMax;
	[SerializeField] bool spawnTimedEnemiesOnWipe;
	[SerializeField] int wipeFlexibility;
	[SerializeField] GameObject keyDrop;


	float spawnTimer = 0;
	BoxCollider zone;
	int waveTracker = 0;
	List<GameObject> enemiesInWave = new List<GameObject>();
	List<GameObject> enemiesSpawned = new List<GameObject>();

	private void Start()
	{
		zone = GetComponent<BoxCollider>();
		zone.isTrigger = true;
	}
	// Update is called once per frame
	void Update()
    {
		if(spawnNextOnWipe)
		{
			if(!IsWaveAlive())
			{
				SpawnNextWave();
			}			
		}
		else
		{
			spawnTimer += Time.deltaTime;
			if (spawnTimedEnemiesOnWipe)
			{
				if (spawnTimer >= spawnTimerMax || !IsAnyWaveAlive())
				{
					SpawnNextWave();
					spawnTimer = 0;
				}
			}
			else
			{
				if (spawnTimer >= spawnTimerMax)
				{
					SpawnNextWave();
					spawnTimer = 0;
				}
			}
		}
		if (waveTracker == encounter.Count && !IsAnyWaveAlive())
		{
			if (keyDrop != null)
			{
				keyDrop.SetActive(true);
			}
			gameObject.SetActive(false);
			return;
		}

	}

	void SpawnNextWave()
	{
		if(waveTracker == encounter.Count)
		{
			return;
		}
		enemiesInWave.Clear();
		if (waveTracker != 0)
		{
			AudioHandler.instance.PlaySound("WaveSpawn", 1, true, 2);
		}
		foreach (var enemyType in encounter[waveTracker].spawns)
		{
			for (int i = 0; i < enemyType.spawnNumber; ++i)
			{
				Vector3 spawnPos = transform.position;
				spawnPos.x += Random.Range(-zone.bounds.extents.x, zone.bounds.extents.x);
				spawnPos.y += Random.Range(-zone.bounds.extents.y, zone.bounds.extents.y);
				spawnPos.z += Random.Range(-zone.bounds.extents.z, zone.bounds.extents.z);
				GameObject enemy = Instantiate(enemyType.enemyPrefab, spawnPos, Quaternion.identity);
				enemiesInWave.Add(enemy);
				enemiesSpawned.Add(enemy);
			}
		}
		++waveTracker;
	}

	bool IsWaveAlive()
	{
		int numberAlive = 0;
		foreach (var enemy in enemiesInWave)
		{
			if (enemy != null)
			{
				++numberAlive;				
			}
		}
		if(numberAlive > wipeFlexibility)
		{
			return true;
		}
		return false;
	}
	
	bool IsAnyWaveAlive()
	{
		int numberAlive = 0;
		foreach (var enemy in enemiesSpawned)
		{
			if (enemy != null)
			{
				++numberAlive;
			}
		}
		if (numberAlive > wipeFlexibility)
		{
			return true;
		}
		return false;
	}
}
