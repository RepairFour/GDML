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
        if(galahadActions[actionCounter].finished || activate == true)
		{
            activate = false;
            ++actionCounter;
            if(actionCounter == galahadActions.Count)
			{
                actionCounter = 0;
                foreach(var action in galahadActions)
				{
                    action.finished = false;
				}
			}
            galahadActions[actionCounter].Perform();
		}
    }
}
