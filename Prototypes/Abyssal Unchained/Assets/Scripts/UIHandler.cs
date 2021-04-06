using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
	public static UIHandler instance;
	[SerializeField] Animator playerHeartAni;
	[SerializeField] TextMeshProUGUI playerHealthText;
	public Canvas GameOver;
	[SerializeField] Slider bossHealthSlider;
	[SerializeField] GameObject WinScreen;
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
	private void Start()
	{
		playerHealthText.text = Player.instance.CurrentHealth().ToString();
		bossHealthSlider.maxValue = FindObjectOfType<TestBoss>().GetComponent<EnemyStats>().CurrentHealth();
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

	public void ReducePlayerHealthText()
	{
		playerHeartAni.SetTrigger("TakenDamage");
		playerHealthText.text = Player.instance.CurrentHealth().ToString();
	}

	public void ShowWinScreen(bool answer)
	{
		WinScreen.SetActive(answer);
		Time.timeScale = 0;
	}
}