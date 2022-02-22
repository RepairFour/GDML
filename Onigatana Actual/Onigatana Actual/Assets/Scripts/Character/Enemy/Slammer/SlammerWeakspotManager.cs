using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammerWeakspotManager : MonoBehaviour
{
    List<EnemyStats> weakSpots;
    // Start is called before the first frame update
    void Start()
    {
        weakSpots = new List<EnemyStats>(GetComponentsInChildren<EnemyStats>());
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var weakSpot in weakSpots)
		{
            if(weakSpot == null)
			{
                weakSpots.Remove(weakSpot);
                break;
			}
		}
        if(weakSpots.Count == 0)
		{
            Destroy(gameObject);
		}
    }
}
