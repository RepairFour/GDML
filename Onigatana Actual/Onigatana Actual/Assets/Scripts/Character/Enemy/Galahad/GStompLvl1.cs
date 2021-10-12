using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GStompLvl1 : GalahadAction
{
    bool performAction = false;
    [SerializeField] GameObject stompHitBox;
    [SerializeField] float hitBoxSize;
    [SerializeField] float stompSpeed;
    Vector3 orginalScale;
    NavMeshAgent nav;
	public override void Perform()
	{
        performAction = true;
        stompHitBox.SetActive(true);
        
    }

	// Start is called before the first frame update
	void Start()
    {
        orginalScale = stompHitBox.transform.localScale;
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(performAction)
		{
            nav.destination = transform.position;
            stompHitBox.transform.localScale += new Vector3(Time.deltaTime * stompSpeed, 0, Time.deltaTime * stompSpeed);
            if (stompHitBox.transform.localScale.x > hitBoxSize)
			{
                stompHitBox.SetActive(false);
                stompHitBox.transform.localScale = orginalScale;
                finished = true;
                performAction = false;
            }
		}
    }
}
