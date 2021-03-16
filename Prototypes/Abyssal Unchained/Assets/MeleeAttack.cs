using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    Animator attack;
    [SerializeField] float duration;
    // Start is called before the first frame update
    void Start()
    {
        attack = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
		{
            attack.SetBool("MeleeAttack", true);
            StartCoroutine(Timer(duration));
		}
    }

    IEnumerator Timer(float time)
	{
        yield return new WaitForSeconds(time);
        attack.SetBool("MeleeAttack", false);
    }
}
