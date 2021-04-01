using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] bool triggerSound;
    [SerializeField] string audioClipName;
    
    void Update()
    {
        if(triggerSound)
		{
            AudioHandler.instance.PlaySound(audioClipName, 1,false,2);
		}
    }
}
