using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLaserSound : MonoBehaviour
{
    [SerializeField] bool triggerSound;
    
    void Update()
    {
        if(triggerSound)
		{
            AudioHandler.instance.PlaySound("LaserBeam",1,false,2);
		}
    }
}
