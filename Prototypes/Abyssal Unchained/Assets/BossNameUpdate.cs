using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BossNameUpdate : MonoBehaviour
{
    [SerializeField]Slot ofFlesh;
    [SerializeField]Slot ofBlood;
    [SerializeField]Slot ofBone;

    TextMeshProUGUI bossName;

	private void Start()
	{
		bossName = GetComponent<TextMeshProUGUI>();
		bossName.text = "";
	}
	public void UpdateName()
	{
		bossName.text = "";
		if (ofBlood.GetComponentInChildren<SacrificeInfo>() != null)
		{
			bossName.text = ofBlood.GetComponentInChildren<SacrificeInfo>().bossNameFix;
		}
		if(ofFlesh.GetComponentInChildren<SacrificeInfo>() != null)
		{
			bossName.text += ofFlesh.GetComponentInChildren<SacrificeInfo>().bossNameFix;
		}
		if (ofBone.GetComponentInChildren<SacrificeInfo>() != null)
		{
			bossName.text += ofBone.GetComponentInChildren<SacrificeInfo>().bossNameFix;
		}
	}
}
