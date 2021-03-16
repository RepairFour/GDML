using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Serialization;

public class GridUpdate : MonoBehaviour
{
    AstarPath pathfinder;
    float cooldown = 0.5f;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GetComponent<AstarPath>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > cooldown)
        {
            timer = 0;
            pathfinder.Scan();
        }
    }
}
