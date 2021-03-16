using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

	public Canvas GameOver;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	public void GameOverScreen(bool answer)
	{
		GameOver.gameObject.SetActive(answer);
		Time.timeScale = 0;
	}

	public void PlayAgainButton()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Time.timeScale = 1;
	}
	public void ExitButton()
	{
		Application.Quit();
	}
}
