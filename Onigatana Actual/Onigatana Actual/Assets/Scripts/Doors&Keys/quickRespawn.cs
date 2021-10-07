using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class quickRespawn : MonoBehaviour
{
    public GameObject Player;
    Controller controller;
    public GameObject PlayerPrefab;
    int deathNumber;
    bool sendAnalytics;


    public Vector3 respawnLocation;


    private void Start()
    {
        deathNumber = 0;
        controller = Player.GetComponent<Controller>();
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("triggered 11");
        if (other.gameObject.tag == "Player")
        {
            deathNumber++;
            Debug.Log("triggered 14");

            //controller.SendAnalytics(deathNumber);
      
            Destroy(Player);
            Player = Instantiate(PlayerPrefab, respawnLocation, Quaternion.identity); //respawn


            controller = Player.GetComponent<Controller>();
            
        }
    }
}
