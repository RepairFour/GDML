using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
	public Spawn playerSpawner;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player")){
			playerSpawner.gameObject.SetActive(true);
			Player.instance.TakeTrueDamage(1);
			Player.instance.respawning = true;
		}
	}
}
