using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HUDCon : MonoBehaviour
{
    public static HUDCon instance;
    [SerializeField] Slider hpBar;
    [SerializeField] Slider bloodBar;
    [SerializeField] TextMeshProUGUI killCount;
	PlayerStats playerStats;
	BloodFuryState bloodFury;
	float playerMaxHp;
	float playerMaxBlood;
	int killsRecorded = 0;

	[SerializeField] List<Transform> keySlots;
	int slotsFilled = 0;

	private void Awake()
	{
		if(instance == null)
		{
            instance = this;
		}
		else
		{
            Destroy(gameObject);
		}
	}
	public void Initialise(float bloodMax)
	{
		playerStats = FindObjectOfType<PlayerStats>();
		bloodFury = playerStats.GetComponent<BloodFuryState>();
		playerMaxHp = playerStats.health;
		playerMaxBlood = bloodMax;
		UpdateHpBar();
		UpdateBloodBar();
	}

	public void UpdateHpBar()
	{
		hpBar.value = playerStats.health / playerMaxHp;
	}

	public void UpdateBloodBar()
	{
		bloodBar.value = (float)bloodFury.currentBlood / playerMaxBlood;
	}

	public void UpdateKillCount()
	{
		++killsRecorded;
		killCount.text = $"Souls Vanquished: {killsRecorded}";
	}

	//////////////////////Key Stuff/////////////////////
	
	public bool FillSlot(GameObject key)
	{
		if (slotsFilled < keySlots.Count)
		{
			key.transform.parent = keySlots[slotsFilled];
			key.transform.localPosition = Vector3.zero;
			key.transform.localRotation = Quaternion.identity;
			key.transform.localScale = new Vector3(100, 100, 1);
			++slotsFilled;
			return true;
		}
		return false;
	}
}
