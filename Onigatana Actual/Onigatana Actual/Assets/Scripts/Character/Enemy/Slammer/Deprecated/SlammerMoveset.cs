using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammerMoveset : MonoBehaviour
{
    [Header("Spin Var")]
    public bool spin = false;
    [SerializeField] SlammerMove spinMove;

    [Header("Raise Lava Vars")]
    public bool raiseLava = false;
    [SerializeField] SlammerMove lavaMove;

    [Header("Quick Spin Vars")]
    public bool quickSpin = false;
    [SerializeField] SlammerMove quickSpinMove;

    [Header("Arc Swipe")]
    public bool arcSwipe;
    [SerializeField] SlammerMove arcSwipeMove;



    [Header("Misc Vars")]
    [SerializeField] float turnSpeed;

    bool performingAction = false;
    GameObject player;
    Vector3 playerLastPos;
    // Start is called before the first frame update
    void Start()
    {        
        player = FindObjectOfType<PlayerStats>().gameObject;
        playerLastPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    { 
        if(spin)
		{
            performingAction = true;
            if(!spinMove.Activate())
			{
                spin = false;
			}
        }
        else if(raiseLava)
		{
            performingAction = true;
            if(!lavaMove.Activate())
			{
                raiseLava = false;
			}
        }
        else if(quickSpin)
		{
            performingAction = true;
            if(!quickSpinMove.Activate())
			{
                quickSpin = false;
			}

        }
        else if(arcSwipe)
		{
            performingAction = true;
            if (!arcSwipeMove.Activate())
			{
                arcSwipe = false;
            }
        }
		else
		{
            performingAction = false;
        }

        //Base movement turning logic
        if (!performingAction)
        {
            Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            var dir = position - transform.position;
            dir.Normalize();
            var rotGoal = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed * Time.deltaTime);
        }
	}


    
}
