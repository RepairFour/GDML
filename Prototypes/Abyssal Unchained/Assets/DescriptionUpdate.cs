using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionUpdate : MonoBehaviour
{
	private List<Slot> slots;
	private BossNameUpdate nameUpdate;
	private void Start()
	{
		slots = new List<Slot>(GetComponentsInChildren<Slot>());
		nameUpdate = FindObjectOfType<BossNameUpdate>();
	}
	public void UpdateDescriptions()
	{
		foreach(var slot in slots)
		{
			if(slot.GetComponentInChildren<SacrificeInfo>() != null) //if there is a sacrifice under the parent in the hierachy
			{
				slot.GetComponentInChildren<TextMeshProUGUI>().text = slot.GetComponentInChildren<SacrificeInfo>().description;
			}
			else
			{
				slot.GetComponentInChildren<TextMeshProUGUI>().text = "";
			}
		}
		nameUpdate.UpdateName();
	}
}
