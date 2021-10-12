using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GStompLvl1 : GalahadAction
{
    bool performAction = false;
    [SerializeField] GameObject stompHitBox;
	public override void Perform()
	{
        performAction = true;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(performAction)
		{

		}
    }
}
