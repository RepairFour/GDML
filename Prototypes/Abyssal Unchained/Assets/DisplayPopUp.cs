using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPopUp : MonoBehaviour
{
	[SerializeField] PopUp popUp;

    public void Show(bool answer)
	{
		if (answer == true)
		{
			if(GetComponentsInChildren<SacrificeInfo>() != null && gameObject.transform.childCount > 0)
			{
				popUp.gameObject.SetActive(true);
				popUp.title.text = GetComponentsInChildren<SacrificeInfo>()[0].title; //there is only every 1 child
				popUp.sacrifice.text = GetComponentsInChildren<SacrificeInfo>()[0].sacrifice;
				popUp.description.text = GetComponentsInChildren<SacrificeInfo>()[0].description;
				popUp.flavour.text = GetComponentsInChildren<SacrificeInfo>()[0].flavour;
			}
		}
		else
		{
			popUp.gameObject.SetActive(false);
		}
	}
}
