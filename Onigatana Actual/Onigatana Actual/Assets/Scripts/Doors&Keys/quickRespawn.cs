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
            AudioHandler.instance.PlaySound("PlayerDeath");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            GameManager.instance.gameOverCanvas.ShowCanvas();
        }
    }
    
}
