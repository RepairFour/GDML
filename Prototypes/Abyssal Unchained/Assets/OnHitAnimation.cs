using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitAnimation : MonoBehaviour
{
	enum Animations
	{
		None,
		Attack1,
		Attack2,
		Attack3,
		Attack4
    }
    [SerializeField] Animations animationToPlay;
	Animator bossAni;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Switch to tag "Boss"
		if (collision.GetComponent<TestBoss>() != null)
		{
			switch (animationToPlay)
			{
				case Animations.Attack1:
					bossAni.SetTrigger("Attack1");
					break;
				case Animations.Attack2:
					bossAni.SetTrigger("Attack2");
					break;
				case Animations.Attack3:
					bossAni.SetTrigger("Attack3");
					break;
				case Animations.Attack4:
					bossAni.SetTrigger("Attack4");
					break;
			}
			gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		//Switch to tag "Boss"
		bossAni = FindObjectOfType<TestBoss>().GetComponent<Animator>();
	}
}
