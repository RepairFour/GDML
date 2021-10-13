using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class quickRespawn : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.GetComponent<PlayerStats>())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
