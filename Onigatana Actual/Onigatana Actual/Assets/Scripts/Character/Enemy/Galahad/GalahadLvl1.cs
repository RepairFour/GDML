using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalahadLvl1 : MonoBehaviour
{
    [SerializeField] List<GalahadAction> galahadActions;
    int actionCounter = 0;
    bool activate = false;

    // Start is called before the first frame update
    void Start()
    {
        activate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (actionCounter == 0)
        {
            if (activate == true)
            {
                PerformAction();
            }
        }
		else if(galahadActions[actionCounter - 1].finished)
		{
            PerformAction();
		}
    }

    void PerformAction()
	{
        activate = false;
        if (actionCounter != 0)
        {
            galahadActions[actionCounter - 1].finished = false;
        }
        galahadActions[actionCounter].Perform();
        ++actionCounter;
        if (actionCounter == galahadActions.Count)
        {
            galahadActions[actionCounter-1].finished = false;
            actionCounter = 0;
            activate = true;
        }
    }
}
