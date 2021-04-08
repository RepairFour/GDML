using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorial : MonoBehaviour
{
    bool showing = true;
    // Start is called before the first frame update
    void Start()
    {
       Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(showing)
		{
            if(Input.anyKey)
			{
                showing = false;
                gameObject.SetActive(false);
                Time.timeScale = 1;
			}
		}
    }
}
