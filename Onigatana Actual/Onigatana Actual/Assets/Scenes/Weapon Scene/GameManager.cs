using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MainCharacterController playerController;

    public PlayerStats playerStats;
    public BloodFuryState bloodFuryState;
    public CanvasManager gameOverCanvas;
    public GameObject playerAimArea;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
