using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
	public GameObject canvas;
	public static CanvasManager instance;
	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Update()
	{
		if(canvas.activeInHierarchy)
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}
	public void ShowCanvas()
	{
		//canvas.SetActive(true);
		//Cursor.lockState = CursorLockMode.None;
		//Cursor.visible = true;
	}
	public void HideCanvas()
	{
		//canvas.SetActive(false);
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;

	}
}
