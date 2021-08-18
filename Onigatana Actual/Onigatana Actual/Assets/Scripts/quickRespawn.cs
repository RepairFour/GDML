using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickRespawn : MonoBehaviour
{
    public GameObject Player;
    public GameObject PlayerPrefab;
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("triggered 11");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("triggered 14");
            //Player.GetComponent<Controller>().enabled = false;
            //Player.transform.position = new Vector3(0, 0, 0);
            //Player.GetComponent<Controller>().enabled = true;
            Destroy(Player);
            Player = Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
