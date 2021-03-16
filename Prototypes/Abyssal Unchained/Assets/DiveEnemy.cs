using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveEnemy : MonoBehaviour
{
    Animator dive;
    // Start is called before the first frame update
    void Start()
    {
        dive = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        LineUpWithCharacter();
    }

    void LineUpWithCharacter()
	{
        if(Mathf.Abs(transform.position.y - Player.instance.transform.position.y) < 0.5)
		{
            dive.SetBool("AbovePlayer", true);

            //transform.position = Player.instance.transform.position
        }
	}
}
