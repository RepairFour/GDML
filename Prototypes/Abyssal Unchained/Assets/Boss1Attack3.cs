using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack3 : MonoBehaviour
{
	[SerializeField]LayerMask mask;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<Player>() != null)
		{
            //Debug.DrawLine(FindObjectOfType<CharacterController2D>().transform.position, FindObjectOfType<TestBoss>().transform.position);
            //Vector2 direction = FindObjectOfType<CharacterController2D>().transform.position - FindObjectOfType<TestBoss>().transform.position;
            Vector2 direction = collision.transform.position - GetComponentInParent<TestBoss>().transform.position;
			RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position,direction, 10000, mask);
			if(hit)
			{
 				if(hit.collider.GetComponent<Player>())
				{
					Player.instance.TakeDamage(1);
				}
			} 
			
		}
	}
}
