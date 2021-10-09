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
	[SerializeField] GameObject keyDrop;

	float spawnTimer = 0;
	BoxCollider zone;
	int waveTracker = 0;
	List<GameObject> enemiesInWave = new List<GameObject>();

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
			foreach(var enemy in enemiesInWave)
			{
				if(enemy != null)
				{
					return;
				}
			}
			SpawnNextWave();
		}
		else
		{
			spawnTimer += Time.deltaTime;
			if(spawnTimer >= spawnTimerMax)
			{
				SpawnNextWave();
				spawnTimer = 0;
			}
		}
    }

	void SpawnNextWave()
	{
		if(waveTracker == encounter.Count)
		{
			if(keyDrop != null)
			{
				keyDrop.SetActive(true);
			}
			gameObject.SetActive(false);
			return;
		}
		enemiesInWave.Clear();
		foreach (var enemyType in encounter[waveTracker].spawns)
		{
			for (int i = 0; i < enemyType.spawnNumber; ++i)
			{
				Vector3 spawnPos = transform.position;
				spawnPos.x += Random.Range(-zone.bounds.extents.x, zone.bounds.extents.x);
				spawnPos.y += Random.Range(-zone.bounds.extents.y, zone.bounds.extents.y);
				spawnPos.z += Random.Range(-zone.bounds.extents.z, zone.bounds.extents.z);
				enemiesInWave.Add(Instantiate(enemyType.enemyPrefab, spawnPos, Quaternion.identity));
			}
		}
		++waveTracker;
	}
}
